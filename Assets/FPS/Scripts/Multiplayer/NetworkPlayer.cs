using Unity.FPS.Gameplay;
using Unity.Netcode;
using UnityEngine;

namespace Unity.FPS.Multiplayer
{
    /// <summary>
    /// Makes the FPS Microgame player work as a networked player:
    /// only the owning client keeps the camera, audio listener, input, movement and weapons.
    /// Remote copies are visual proxies driven by <see cref="Unity.Netcode.Components.NetworkTransform"/>.
    ///
    /// The Microgame player is first-person only, so it has no third-person body mesh.
    /// Remote copies therefore get a simple robot avatar built from primitives, otherwise
    /// other players would only see the ShadowProjector blob.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayer : NetworkBehaviour
    {
        Camera[] m_PlayerCameras;
        AudioListener[] m_AudioListeners;
        PlayerInputHandler m_InputHandler;
        PlayerCharacterController m_CharacterController;
        PlayerWeaponsManager m_WeaponsManager;
        GameObject m_RemoteAvatar;

        static Material s_BodyMaterial;
        static Material s_AccentMaterial;

        void Awake()
        {
            m_PlayerCameras = GetComponentsInChildren<Camera>(true);
            m_AudioListeners = GetComponentsInChildren<AudioListener>(true);
            m_InputHandler = GetComponent<PlayerInputHandler>();
            m_CharacterController = GetComponent<PlayerCharacterController>();
            m_WeaponsManager = GetComponent<PlayerWeaponsManager>();
        }

        public override void OnNetworkSpawn()
        {
            bool isLocalPlayer = IsOwner;

            for (int i = 0; i < m_PlayerCameras.Length; i++)
            {
                if (m_PlayerCameras[i] != null)
                    m_PlayerCameras[i].gameObject.SetActive(isLocalPlayer);
            }

            for (int i = 0; i < m_AudioListeners.Length; i++)
            {
                if (m_AudioListeners[i] != null)
                    m_AudioListeners[i].enabled = isLocalPlayer;
            }

            if (m_InputHandler != null)
                m_InputHandler.enabled = isLocalPlayer;

            if (m_CharacterController != null)
                m_CharacterController.enabled = isLocalPlayer;

            if (m_WeaponsManager != null)
                m_WeaponsManager.enabled = isLocalPlayer;

            CharacterController unityCharacterController = GetComponent<CharacterController>();
            if (unityCharacterController != null)
                unityCharacterController.enabled = isLocalPlayer;

            // The stock player only has a projected blob shadow. Give remote copies a
            // visible body so teammates can actually see each other.
            if (isLocalPlayer)
            {
                HideRemoteAvatar();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                ShowRemoteAvatar();
            }
        }

        void HideRemoteAvatar()
        {
            if (m_RemoteAvatar != null)
                m_RemoteAvatar.SetActive(false);
        }

        void ShowRemoteAvatar()
        {
            if (m_RemoteAvatar == null)
                m_RemoteAvatar = BuildRemoteAvatar();

            m_RemoteAvatar.SetActive(true);
        }

        GameObject BuildRemoteAvatar()
        {
            GameObject root = new GameObject("Remote Avatar");
            root.layer = gameObject.layer;
            root.transform.SetParent(transform, false);

            Material body = GetBodyMaterial();
            Material accent = GetAccentMaterial();

            // Torso
            CreatePart(PrimitiveType.Capsule, "Torso", root.transform,
                new Vector3(0f, 0.95f, 0f), new Vector3(0.62f, 0.9f, 0.5f), body);

            // Head and visor
            CreatePart(PrimitiveType.Sphere, "Head", root.transform,
                new Vector3(0f, 1.92f, 0f), Vector3.one * 0.42f, body);
            CreatePart(PrimitiveType.Cube, "Visor", root.transform,
                new Vector3(0f, 1.94f, 0.18f), new Vector3(0.3f, 0.1f, 0.08f), accent);

            // Shoulders and arms
            CreatePart(PrimitiveType.Cube, "Shoulder L", root.transform,
                new Vector3(-0.42f, 1.55f, 0f), new Vector3(0.22f, 0.18f, 0.34f), body);
            CreatePart(PrimitiveType.Cube, "Shoulder R", root.transform,
                new Vector3(0.42f, 1.55f, 0f), new Vector3(0.22f, 0.18f, 0.34f), body);
            CreatePart(PrimitiveType.Capsule, "Arm L", root.transform,
                new Vector3(-0.44f, 1.15f, 0f), new Vector3(0.16f, 0.4f, 0.16f), body);
            CreatePart(PrimitiveType.Capsule, "Arm R", root.transform,
                new Vector3(0.44f, 1.15f, 0f), new Vector3(0.16f, 0.4f, 0.16f), body);

            // Chest light makes the avatar readable in dark levels.
            CreatePart(PrimitiveType.Cube, "Chest Light", root.transform,
                new Vector3(0f, 1.45f, 0.24f), new Vector3(0.2f, 0.08f, 0.04f), accent);

            // The blob shadow exists only because the Microgame player has no body.
            // Projector is deprecated in Unity 6, so find it by type name instead.
            Behaviour[] behaviours = GetComponentsInChildren<Behaviour>(true);
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i].GetType().Name == "Projector")
                    behaviours[i].enabled = false;
            }

            return root;
        }

        static void CreatePart(PrimitiveType type, string name, Transform parent, Vector3 localPosition,
            Vector3 localScale, Material material)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = name;
            part.layer = parent.gameObject.layer;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = Quaternion.identity;
            part.transform.localScale = localScale;

            Collider collider = part.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;
        }

        static Material GetBodyMaterial()
        {
            if (s_BodyMaterial == null)
            {
                s_BodyMaterial = CreateMaterial("Universal Render Pipeline/Lit",
                    new Color(0.16f, 0.19f, 0.24f));
            }

            return s_BodyMaterial;
        }

        static Material GetAccentMaterial()
        {
            if (s_AccentMaterial == null)
            {
                s_AccentMaterial = CreateMaterial("Universal Render Pipeline/Unlit",
                    new Color(0.25f, 0.92f, 1f));
            }

            return s_AccentMaterial;
        }

        static Material CreateMaterial(string shaderName, Color color)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader == null)
                shader = Shader.Find("Standard");

            Material material = new Material(shader);
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color"))
                material.SetColor("_Color", color);
            return material;
        }
    }
}
