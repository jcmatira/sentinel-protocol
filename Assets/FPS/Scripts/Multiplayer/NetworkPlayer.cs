using Unity.FPS.Gameplay;
using Unity.Netcode;
using UnityEngine;

namespace Unity.FPS.Multiplayer
{
    /// <summary>
    /// Makes the FPS Microgame player work as a networked player:
    /// only the owning client keeps the camera, audio listener, input, movement and weapons.
    /// Remote copies are visual proxies driven by <see cref="Unity.Netcode.Components.NetworkTransform"/>.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayer : NetworkBehaviour
    {
        Camera[] m_PlayerCameras;
        AudioListener[] m_AudioListeners;
        PlayerInputHandler m_InputHandler;
        PlayerCharacterController m_CharacterController;
        PlayerWeaponsManager m_WeaponsManager;

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

            if (isLocalPlayer)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
