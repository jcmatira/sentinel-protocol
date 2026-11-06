using Unity.FPS.Gameplay;
using Unity.Netcode;
using UnityEngine;

namespace Unity.FPS.UI
{
    /// <summary>
    /// Finds the player that belongs to this machine.
    ///
    /// The stock FPS Microgame HUD uses FindAnyObjectByType and therefore binds to the
    /// level's single-player Player. In a network session that copy is stripped, so every
    /// HUD element (crosshair, health, ammo, compass...) must instead bind to the local
    /// NetworkPlayer. The local player may spawn a frame or two after the scene loads, so
    /// HUD scripts retry this lookup until it succeeds.
    /// </summary>
    public static class LocalPlayerLocator
    {
        public static PlayerCharacterController Find()
        {
            PlayerCharacterController[] players = Object.FindObjectsByType<PlayerCharacterController>();

            bool networked = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;

            for (int i = 0; i < players.Length; i++)
            {
                PlayerCharacterController player = players[i];
                if (player == null)
                    continue;

                NetworkObject networkObject = player.GetComponent<NetworkObject>();

                if (networked)
                {
                    // Only the player we own is ours. Remote proxies belong to other machines.
                    if (networkObject != null && networkObject.IsSpawned && networkObject.IsOwner)
                        return player;

                    continue;
                }

                // Single-player / menu fallback.
                if (networkObject == null && player.gameObject.activeInHierarchy)
                    return player;
            }

            return null;
        }
    }
}
