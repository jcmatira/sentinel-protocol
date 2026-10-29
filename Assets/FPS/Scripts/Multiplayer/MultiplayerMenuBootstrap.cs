using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Multiplayer
{
    /// <summary>
    /// Adds the LAN host/join panel to the IntroMenu automatically.
    /// The panel is additive: the existing single-player START button still works.
    /// </summary>
    public static class MultiplayerMenuBootstrap
    {
        const string k_MenuSceneName = "IntroMenu";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name != k_MenuSceneName)
                return;

            if (Object.FindAnyObjectByType<HostJoinUI>() != null)
                return;

            GameObject go = new GameObject("Multiplayer Menu");
            go.AddComponent<HostJoinUI>();
        }
    }
}
