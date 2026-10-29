using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Collections;
using Unity.FPS.AI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
        bool m_SceneEventsHooked;

        const string k_EnemySnapshotMessage = "Sentinel_EnemySnapshot";
        const string k_EnemyKillMessage = "Sentinel_EnemyKill";
        const string k_GameStateMessage = "Sentinel_GameState";

        readonly List<EnemyController> m_Enemies = new List<EnemyController>();
        bool[] m_EnemyDead = Array.Empty<bool>();
        GameModeManager m_GameMode;
        string m_InitializedSceneName;
        string m_PendingSceneInit;
        float m_NextEnemySnapshotTime;
        float m_NextGameStateTime;
        bool m_ClientKillListenerRegistered;

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
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnUnitySceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
            UnregisterMessageHandlers();

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
                // Register the prefab for manual spawning after a mission scene loads.
                // PlayerPrefab is intentionally left null so no avatar spawns in the menu.
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

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnUnitySceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnUnitySceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
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
            if (started)
            {
                HookSceneEvents();
                RegisterMessageHandlers();
            }
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
            if (started)
            {
                HookSceneEvents();
                RegisterMessageHandlers();
            }
            return started;
        }

        public void Shutdown()
        {
            if (Manager != null && Manager.IsListening)
                Manager.Shutdown();

            UnregisterMessageHandlers();
            m_ApprovedClients = 0;
            m_SceneEventsHooked = false;
            SetStatus("Offline");
        }

        /// <summary>
        /// Loads a mission scene through NGO. Only the host may call this; clients follow automatically.
        /// </summary>
        public bool LoadMissionScene(string sceneName)
        {
            if (Manager == null || !Manager.IsListening)
            {
                SetStatus("Start a session first");
                return false;
            }

            if (!Manager.IsServer)
            {
                SetStatus("Only the host can start the mission");
                return false;
            }

            if (Manager.SceneManager == null)
            {
                SetStatus("Scene manager is not ready");
                return false;
            }

            Manager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            return true;
        }

        void HookSceneEvents()
        {
            if (Manager == null || Manager.SceneManager == null)
                return;

            Manager.SceneManager.OnLoadEventCompleted -= OnSceneLoadCompleted;
            Manager.SceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;
            m_SceneEventsHooked = true;
        }

        void Update()
        {
            // StartHost/StartClient normally create the scene manager synchronously,
            // but hooking here also covers a delayed initialization.
            if (!m_SceneEventsHooked && Manager != null && Manager.IsListening && Manager.SceneManager != null)
                HookSceneEvents();

            if (!string.IsNullOrEmpty(m_PendingSceneInit) && Manager != null && Manager.IsListening)
                InitializeSceneState(m_PendingSceneInit);

            if (Manager == null || !Manager.IsListening || !Manager.IsServer)
                return;

            if (string.IsNullOrEmpty(m_InitializedSceneName))
                return;

            if (Time.time >= m_NextGameStateTime)
            {
                SendGameState();
                m_NextGameStateTime = Time.time + 0.25f;
            }

            if (Time.time >= m_NextEnemySnapshotTime)
            {
                SendEnemySnapshot();
                m_NextEnemySnapshotTime = Time.time + 0.1f;
            }
        }

        void OnUnitySceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (Manager == null || !Manager.IsListening)
                return;

            // Clients: make sure the level's single-player Player is disabled even if
            // the NGO load event is processed later.
            if (!Manager.IsServer && scene.name != "IntroMenu")
                DisableLocalScenePlayer();

            InitializeSceneState(scene.name);
        }

        void OnSceneLoadCompleted(string sceneName, LoadSceneMode loadMode,
            List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            if (Manager == null || !Manager.IsListening)
                return;

            if (Manager.IsServer)
                ServerSpawnPlayers();
            else
                DisableLocalScenePlayer();

            InitializeSceneState(sceneName);
        }

        void ServerSpawnPlayers()
        {
            GameObject prefab = Resources.Load<GameObject>(PlayerPrefabResourcePath);
            if (prefab == null)
            {
                Debug.LogError("[Network] Cannot spawn players: no prefab at Resources/" +
                               PlayerPrefabResourcePath + ". Run Tools > Multiplayer > Setup Network Player Prefab.");
                return;
            }

            Vector3 origin;
            Quaternion rotation;
            FindScenePlayer(out origin, out rotation);

            // The level ships with a single-player Player prefab instance. In a network
            // session that copy is replaced by one spawned NetworkObject per client.
            DisableLocalScenePlayer();

            int index = 0;
            foreach (ulong clientId in Manager.ConnectedClientsIds)
            {
                Vector3 offset = new Vector3((index % 2) * 1.5f, 0f, (index / 2) * 1.5f);
                GameObject instance = Instantiate(prefab, origin + offset, rotation);
                NetworkObject networkObject = instance.GetComponent<NetworkObject>();

                if (networkObject == null)
                {
                    Debug.LogError("[Network] The network player prefab has no NetworkObject component.");
                    Destroy(instance);
                    continue;
                }

                networkObject.SpawnWithOwnership(clientId, true);
                index++;
            }
        }

        PlayerCharacterController FindScenePlayer(out Vector3 position, out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;

            PlayerCharacterController[] players = FindObjectsByType<PlayerCharacterController>();
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<NetworkObject>() == null)
                {
                    position = players[i].transform.position;
                    rotation = players[i].transform.rotation;
                    return players[i];
                }
            }

            return null;
        }

        void DisableLocalScenePlayer()
        {
            // Keep the level's single-player Player GameObject alive (the HUD holds
            // references to it) but strip its controls, camera, audio and visuals.
            // The spawned NetworkPlayers replace it visually.
            PlayerCharacterController[] players = FindObjectsByType<PlayerCharacterController>();
            for (int i = 0; i < players.Length; i++)
            {
                PlayerCharacterController player = players[i];
                if (player.GetComponent<NetworkObject>() != null)
                    continue;

                player.enabled = false;

                PlayerInputHandler input = player.GetComponent<PlayerInputHandler>();
                if (input != null) input.enabled = false;

                PlayerWeaponsManager weapons = player.GetComponent<PlayerWeaponsManager>();
                if (weapons != null) weapons.enabled = false;

                CharacterController controller = player.GetComponent<CharacterController>();
                if (controller != null) controller.enabled = false;

                Camera[] cameras = player.GetComponentsInChildren<Camera>(true);
                for (int c = 0; c < cameras.Length; c++)
                    cameras[c].gameObject.SetActive(false);

                AudioListener[] listeners = player.GetComponentsInChildren<AudioListener>(true);
                for (int l = 0; l < listeners.Length; l++)
                    listeners[l].enabled = false;

                Renderer[] renderers = player.GetComponentsInChildren<Renderer>(true);
                for (int r = 0; r < renderers.Length; r++)
                    renderers[r].enabled = false;
            }
        }

        void RegisterMessageHandlers()
        {
            if (Manager == null || Manager.CustomMessagingManager == null)
                return;

            Manager.CustomMessagingManager.UnregisterNamedMessageHandler(k_EnemySnapshotMessage);
            Manager.CustomMessagingManager.UnregisterNamedMessageHandler(k_EnemyKillMessage);
            Manager.CustomMessagingManager.UnregisterNamedMessageHandler(k_GameStateMessage);

            if (Manager.IsServer)
            {
                Manager.CustomMessagingManager.RegisterNamedMessageHandler(k_EnemyKillMessage, OnEnemyKillClaim);
            }
            else
            {
                Manager.CustomMessagingManager.RegisterNamedMessageHandler(k_EnemySnapshotMessage, OnEnemySnapshot);
                Manager.CustomMessagingManager.RegisterNamedMessageHandler(k_GameStateMessage, OnGameState);

                EventManager.RemoveListener<EnemyKillEvent>(OnClientEnemyKilled);
                EventManager.AddListener<EnemyKillEvent>(OnClientEnemyKilled);
                m_ClientKillListenerRegistered = true;
            }
        }

        void UnregisterMessageHandlers()
        {
            if (Manager != null && Manager.CustomMessagingManager != null)
            {
                Manager.CustomMessagingManager.UnregisterNamedMessageHandler(k_EnemySnapshotMessage);
                Manager.CustomMessagingManager.UnregisterNamedMessageHandler(k_EnemyKillMessage);
                Manager.CustomMessagingManager.UnregisterNamedMessageHandler(k_GameStateMessage);
            }

            if (m_ClientKillListenerRegistered)
            {
                EventManager.RemoveListener<EnemyKillEvent>(OnClientEnemyKilled);
                m_ClientKillListenerRegistered = false;
            }
        }

        void OnSceneUnloaded(Scene scene)
        {
            m_InitializedSceneName = null;
            m_PendingSceneInit = null;
            m_GameMode = null;
            m_Enemies.Clear();
            m_EnemyDead = Array.Empty<bool>();
        }

        void InitializeSceneState(string sceneName)
        {
            if (Manager == null || !Manager.IsListening || string.IsNullOrEmpty(sceneName))
                return;

            if (sceneName == "IntroMenu" || sceneName == "WinScene" || sceneName == "LoseScene")
                return;

            if (m_InitializedSceneName == sceneName)
                return;

            GameModeManager gameMode = FindAnyObjectByType<GameModeManager>();
            if (gameMode == null)
            {
                m_PendingSceneInit = sceneName;
                return;
            }

            m_PendingSceneInit = null;
            m_InitializedSceneName = sceneName;
            m_GameMode = gameMode;
            m_GameMode.SetAuthoritative(Manager.IsServer);

            BuildEnemyList();

            if (!Manager.IsServer)
            {
                DestroyObjectiveManager();
                ConfigureClientEnemies();
            }

            m_NextEnemySnapshotTime = Time.time;
            m_NextGameStateTime = Time.time;
        }

        void BuildEnemyList()
        {
            m_Enemies.Clear();
            EnemyController[] enemies = FindObjectsByType<EnemyController>();
            Array.Sort(enemies, CompareEnemies);
            m_Enemies.AddRange(enemies);
            m_EnemyDead = new bool[m_Enemies.Count];
        }

        static int CompareEnemies(EnemyController a, EnemyController b)
        {
            int byName = string.CompareOrdinal(a.gameObject.name, b.gameObject.name);
            if (byName != 0)
                return byName;

            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        }

        void DestroyObjectiveManager()
        {
            ObjectiveManager manager = FindAnyObjectByType<ObjectiveManager>();
            if (manager != null)
                Destroy(manager);
        }

        void ConfigureClientEnemies()
        {
            for (int i = 0; i < m_Enemies.Count; i++)
            {
                EnemyController enemy = m_Enemies[i];
                if (enemy == null)
                    continue;

                enemy.enabled = false;
                DisableComponents<EnemyMobile>(enemy.gameObject);
                DisableComponents<EnemyTurret>(enemy.gameObject);
                DisableComponents<NavigationModule>(enemy.gameObject);
                DisableComponents<DetectionModule>(enemy.gameObject);
                DisableComponents<NavMeshAgent>(enemy.gameObject);
                DisableComponents<WeaponController>(enemy.gameObject);
            }
        }

        static void DisableComponents<T>(GameObject root) where T : Behaviour
        {
            T[] components = root.GetComponentsInChildren<T>(true);
            for (int i = 0; i < components.Length; i++)
                components[i].enabled = false;
        }

        void OnClientEnemyKilled(EnemyKillEvent evt)
        {
            if (Manager == null || !Manager.IsListening || Manager.IsServer || evt.Enemy == null)
                return;

            int index = -1;
            for (int i = 0; i < m_Enemies.Count; i++)
            {
                if (m_Enemies[i] != null && m_Enemies[i].gameObject == evt.Enemy)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0 || m_EnemyDead[index])
                return;

            m_EnemyDead[index] = true;
            SendKillClaim(index);
        }

        void SendKillClaim(int index)
        {
            if (Manager == null || Manager.CustomMessagingManager == null)
                return;

            using (FastBufferWriter writer = new FastBufferWriter(8, Allocator.Temp))
            {
                writer.WriteValueSafe(index);
                Manager.CustomMessagingManager.SendNamedMessage(k_EnemyKillMessage,
                    NetworkManager.ServerClientId, writer);
            }
        }

        void OnEnemyKillClaim(ulong sender, FastBufferReader reader)
        {
            if (Manager == null || !Manager.IsServer)
                return;

            reader.ReadValueSafe(out int index);
            if (index < 0 || index >= m_Enemies.Count)
                return;

            EnemyController enemy = m_Enemies[index];
            if (enemy == null)
                return;

            Health health = enemy.GetComponent<Health>();
            if (health != null && health.CurrentHealth > 0f)
                health.Kill();
        }

        void SendEnemySnapshot()
        {
            if (Manager == null || !Manager.IsServer || Manager.CustomMessagingManager == null)
                return;

            if (m_Enemies.Count == 0)
                return;

            using (FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp))
            {
                writer.WriteValueSafe(m_Enemies.Count);

                for (int i = 0; i < m_Enemies.Count; i++)
                {
                    EnemyController enemy = m_Enemies[i];
                    Health health = enemy != null ? enemy.GetComponent<Health>() : null;
                    bool alive = enemy != null && (health == null || health.CurrentHealth > 0f);
                    Vector3 position = alive ? enemy.transform.position : Vector3.zero;
                    Quaternion rotation = alive ? enemy.transform.rotation : Quaternion.identity;

                    writer.WriteValueSafe(position);
                    writer.WriteValueSafe(rotation);
                    writer.WriteValueSafe(alive);
                }

                Manager.CustomMessagingManager.SendNamedMessageToAll(k_EnemySnapshotMessage, writer,
                    NetworkDelivery.Unreliable);
            }
        }

        void OnEnemySnapshot(ulong sender, FastBufferReader reader)
        {
            if (Manager == null || Manager.IsServer)
                return;

            reader.ReadValueSafe(out int count);
            for (int i = 0; i < count; i++)
            {
                reader.ReadValueSafe(out Vector3 position);
                reader.ReadValueSafe(out Quaternion rotation);
                reader.ReadValueSafe(out bool alive);

                if (i >= m_Enemies.Count)
                    continue;

                EnemyController enemy = m_Enemies[i];
                if (enemy == null)
                    continue;

                if (!alive || m_EnemyDead[i])
                {
                    if (enemy.gameObject.activeSelf)
                        enemy.gameObject.SetActive(false);
                    continue;
                }

                enemy.transform.SetPositionAndRotation(position, rotation);
            }
        }

        void SendGameState()
        {
            if (Manager == null || !Manager.IsServer || m_GameMode == null ||
                Manager.CustomMessagingManager == null)
                return;

            using (FastBufferWriter writer = new FastBufferWriter(64, Allocator.Temp))
            {
                writer.WriteValueSafe(m_GameMode.TimeRemaining);
                writer.WriteValueSafe(m_GameMode.Kills);
                writer.WriteValueSafe(m_GameMode.Score);
                Manager.CustomMessagingManager.SendNamedMessageToAll(k_GameStateMessage, writer,
                    NetworkDelivery.UnreliableSequenced);
            }
        }

        void OnGameState(ulong sender, FastBufferReader reader)
        {
            if (Manager == null || Manager.IsServer)
                return;

            reader.ReadValueSafe(out float timeRemaining);
            reader.ReadValueSafe(out int kills);
            reader.ReadValueSafe(out int score);

            if (m_GameMode == null)
                m_GameMode = FindAnyObjectByType<GameModeManager>();

            if (m_GameMode != null)
                m_GameMode.ApplyNetworkState(timeRemaining, kills, score);
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
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                return "127.0.0.1";

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
