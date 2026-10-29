using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// Makes the gameplay mode work out of the box: whenever a scene containing the Microgame
    /// (a GameFlowManager + the player) is loaded and no <see cref="GameModeManager"/> exists,
    /// one is created automatically.
    ///
    /// IMPORTANT: RuntimeInitializeOnLoadMethod(AfterSceneLoad) only fires once for the very
    /// first scene of the application. In the Editor the first scene is usually MainScene, but
    /// in a build the first scene is IntroMenu - so we subscribe to SceneManager.sceneLoaded
    /// instead, which fires for every scene load (including the first one).
    ///
    /// If you prefer to see / tweak the mode in the Hierarchy, use
    /// Tools > Game Mode > Install Game Mode Into MainScene.
    /// </summary>
    public static class GameModeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) => InstallIfMissing(scene.name);

        static void InstallIfMissing(string sceneName)
        {
            // Only the gameplay scene has a GameFlowManager; menu / win / lose scenes are ignored.
            if (Object.FindAnyObjectByType<GameFlowManager>() == null)
                return;

            if (Object.FindAnyObjectByType<GameModeManager>() != null)
                return;

            if (Object.FindAnyObjectByType<PlayerCharacterController>() == null)
                return;

            GameObject manager = new GameObject("Game Mode Manager");
            GameModeManager gameMode = manager.AddComponent<GameModeManager>();
            gameMode.ApplyLevelDefaults(sceneName);
        }
    }
}
