#if UNITY_EDITOR
using Unity.FPS.Multiplayer;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;

namespace Unity.FPS.EditorTools
{
    /// <summary>
    /// Creates the networked version of the Microgame player prefab.
    ///
    /// The prefab lives in a Resources folder so the runtime NetworkSession can register it
    /// with the NetworkManager without editing any scene or prefab list by hand.
    /// </summary>
    public static class MultiplayerSetup
    {
        const string k_SourcePrefab = "Assets/FPS/Prefabs/Player.prefab";
        const string k_ResourcesRoot = "Assets/FPS/Resources";
        const string k_TargetFolder = "Assets/FPS/Resources/Network";
        const string k_TargetPrefab = "Assets/FPS/Resources/Network/Player_Networked.prefab";

        [MenuItem("Tools/Multiplayer/Setup Network Player Prefab")]
        public static void SetupPlayerPrefab()
        {
            if (!AssetDatabase.IsValidFolder(k_ResourcesRoot))
                AssetDatabase.CreateFolder("Assets/FPS", "Resources");

            if (!AssetDatabase.IsValidFolder(k_TargetFolder))
                AssetDatabase.CreateFolder(k_ResourcesRoot, "Network");

            if (AssetDatabase.LoadAssetAtPath<GameObject>(k_TargetPrefab) == null)
            {
                if (!AssetDatabase.CopyAsset(k_SourcePrefab, k_TargetPrefab))
                {
                    Debug.LogError("[Multiplayer] Could not copy " + k_SourcePrefab + " to " + k_TargetPrefab);
                    return;
                }
            }

            GameObject contents = PrefabUtility.LoadPrefabContents(k_TargetPrefab);
            bool changed = false;

            if (contents.GetComponent<NetworkObject>() == null)
            {
                contents.AddComponent<NetworkObject>();
                changed = true;
            }

            NetworkTransform networkTransform = contents.GetComponent<NetworkTransform>();
            if (networkTransform == null)
            {
                networkTransform = contents.AddComponent<NetworkTransform>();
                changed = true;
            }

            networkTransform.AuthorityMode = NetworkTransform.AuthorityModes.Owner;
            networkTransform.Interpolate = true;
            networkTransform.InLocalSpace = false;

            if (contents.GetComponent<NetworkPlayer>() == null)
            {
                contents.AddComponent<NetworkPlayer>();
                changed = true;
            }

            PrefabUtility.SaveAsPrefabAsset(contents, k_TargetPrefab);
            PrefabUtility.UnloadPrefabContents(contents);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(changed
                ? "[Multiplayer] Network player prefab created at " + k_TargetPrefab
                : "[Multiplayer] Network player prefab already up to date at " + k_TargetPrefab);

            Debug.Log("[Multiplayer] Phase 0 ready. Run the game from IntroMenu and use the LAN MULTIPLAYER panel.");
        }
    }
}
#endif
