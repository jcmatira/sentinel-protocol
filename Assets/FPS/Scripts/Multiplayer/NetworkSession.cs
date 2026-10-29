using System;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Unity.FPS.Multiplayer
{
    /// <summary>
    /// Owns the Unity Netcode for GameObjects <see cref="NetworkManager"/> for the whole game.
    /// It is created on demand (when the player hosts or joins) and survives scene loads.
    ///
    /// Phase 0: host / join over LAN, connection approval, player limit.
    /// Phase 1: player prefab spawning and per-player camera/input isolation.
    /// </summary>
    [DisallowMultipleComponent]
    public class NetworkSession : MonoBehaviour
    {
        public const ushort DefaultPort = 7777;
        public const int MaxPlayers = 4;

        /// <summary>Prefab path relative to a Resources folder.</summary>
        public const string PlayerPrefabResourcePath = "Network/Player_Networked";

        public static NetworkSession Instance { get; private set; }

        public NetworkManager Manager { get; private set; }
        public UnityTransport Transport { get; private set; }

        /// <summary>Raised whenever the status message or player count changes.</summary>
        public event Action Changed;

        public bool IsOnline => Manager != null && Manager.IsListening;
        public bool IsHost => Manager != null && Manager.IsHost;
        public bool IsClient => Manager != null && Manager.IsClient;
        public int PlayerCount => Manager != null ? Manager.ConnectedClientsIds.Count : 0;
        public string StatusMessage { get; private set; } = "Offline";

        [Tooltip("Phase 0: leave off so no avatar spawns in the menu. Phase 1 spawns players when the mission loads.")]
        public bool SpawnPlayerAvatars = false;

        int m_ApprovedClients;

        public static NetworkSession EnsureExists()
        {
            if (Instance != null)
                return Instance;

            GameObject go = new GameObject("Network Session");
            return go.AddComponent<NetworkSession>();
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            try
            {
                CreateManager();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("[Network] Could not create the NetworkManager. Host/join is unavailable.");
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            if (Manager != null)
                Destroy(Manager.gameObject);
        }

        void CreateManager()
        {
            // Built inactive so the NetworkConfig is fully set before NetworkManager.Awake runs.
            // The NetworkManager must be a root object: NGO does not support nested managers.
            GameObject go = new GameObject("Network Manager");
            go.SetActive(false);

            Manager = go.AddComponent<NetworkManager>();
            Transport = go.AddComponent<UnityTransport>();

            // A NetworkManager added at runtime does not get a serialized NetworkConfig,
            // so create one before touching any configuration.
            if (Manager.NetworkConfig == null)
                Manager.NetworkConfig = new NetworkConfig();

            GameObject playerPrefab = Resources.Load<GameObject>(PlayerPrefabResourcePath);
            if (playerPrefab != null)
            {
                Manager.NetworkConfig.PlayerPrefab = playerPrefab;
                Manager.NetworkConfig.Prefabs.Add(new NetworkPrefab { Prefab = playerPrefab });
            }
            else
            {
                Debug.LogWarning("[Network] No network player prefab at Resources/" + PlayerPrefabResourcePath +
                                 ". Run Tools > Multiplayer > Setup Network Player Prefab. " +
                                 "Players will connect without avatars until then.");
            }

            Manager.NetworkConfig.NetworkTransport = Transport;
            Manager.NetworkConfig.ConnectionApproval = true;
            Manager.NetworkConfig.EnableSceneManagement = true;

            Manager.ConnectionApprovalCallback = ApprovalCheck;
            Manager.OnClientConnectedCallback += OnClientConnected;
            Manager.OnClientDisconnectCallback += OnClientDisconnected;
            Manager.OnTransportFailure += OnTransportFailure;

            go.SetActive(true);
            DontDestroyOnLoad(go);
        }

        void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            // The host also counts toward MaxPlayers, so remote clients are capped at MaxPlayers - 1.
            if (m_ApprovedClients >= MaxPlayers - 1)
            {
                response.Approved = false;
                response.Reason = "Server is full";
                return;
            }

            m_ApprovedClients++;
            response.Approved = true;
            response.CreatePlayerObject = SpawnPlayerAvatars && Manager.NetworkConfig.PlayerPrefab != null;
            response.Pending = false;
            response.Position = Vector3.zero;
            response.Rotation = Quaternion.identity;
        }

        void OnClientConnected(ulong clientId) => SetStatus(BuildStatus());

        void OnClientDisconnected(ulong clientId)
        {
            if (Manager != null && clientId != Manager.LocalClientId)
                m_ApprovedClients = Mathf.Max(0, m_ApprovedClients - 1);

            if (Manager != null && Manager.IsListening)
                SetStatus(BuildStatus());
            else
                SetStatus(Manager != null && Manager.IsHost ? "Offline" : "Disconnected");
        }

        void OnTransportFailure() => SetStatus("Network transport failure");

        public bool StartHost(ushort port = DefaultPort)
        {
            if (Manager == null)
                return false;

            if (Manager.IsListening)
                Shutdown();

            // Bind the listen socket to all interfaces so LAN clients can reach the host.
            Transport.SetConnectionData("0.0.0.0", port, "0.0.0.0");

            bool started = Manager.StartHost();
            SetStatus(started ? BuildStatus() : "Failed to host");
            return started;
        }

        public bool StartClient(string address, ushort port = DefaultPort)
        {
            if (Manager == null)
                return false;

            if (string.IsNullOrWhiteSpace(address))
            {
                SetStatus("Enter the host IP address");
                return false;
            }

            if (Manager.IsListening)
                Shutdown();

            address = address.Trim();
            Transport.SetConnectionData(address, port);

            bool started = Manager.StartClient();
            SetStatus(started ? "Connecting to " + address + ":" + port + "..." : "Failed to start client");
            return started;
        }

        public void Shutdown()
        {
            if (Manager != null && Manager.IsListening)
                Manager.Shutdown();

            m_ApprovedClients = 0;
            SetStatus("Offline");
        }

        string BuildStatus()
        {
            if (Manager == null || !Manager.IsListening)
                return "Offline";

            string role = Manager.IsHost ? "Hosting" : "Connected";
            return role + " - " + Manager.ConnectedClientsIds.Count + "/" + MaxPlayers + " players";
        }

        void SetStatus(string message)
        {
            StatusMessage = message;
            Changed?.Invoke();
        }

        /// <summary>Best-effort LAN IPv4 address to show to the host.</summary>
        public static string GetLocalIPv4()
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                for (int i = 0; i < addresses.Length; i++)
                {
                    if (addresses[i].AddressFamily == AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(addresses[i]))
                        return addresses[i].ToString();
                }
            }
            catch (Exception)
            {
                // Fall through to loopback.
            }

            return "127.0.0.1";
        }
    }
}
