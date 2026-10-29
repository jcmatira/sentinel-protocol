#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Gameplay.EditorTools
{
    /// <summary>
    /// Optional helper: puts a GameModeManager GameObject inside MainScene and saves it, so the
    /// gameplay mode is visible and configurable in the Hierarchy.
    /// The mode also works at runtime without this step (see GameModeBootstrap).
    /// </summary>
    public static class GameModeSceneInstaller
    {
        const string k_MainScenePath = "Assets/FPS/Scenes/MainScene.unity";

        [MenuItem("Tools/Game Mode/Install Game Mode Into MainScene")]
        public static void InstallIntoMainScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (activeScene.path != k_MainScenePath)
            {
                EditorUtility.DisplayDialog(
                    "Game Mode",
                    "Open " + k_MainScenePath + " first, then run this command again.",
                    "OK");
                return;
            }

            if (Object.FindAnyObjectByType<GameModeManager>() != null)
            {
                EditorUtility.DisplayDialog("Game Mode", "MainScene already contains a Game Mode Manager.", "OK");
                return;
            }

            GameObject manager = new GameObject("Game Mode Manager");
            Undo.RegisterCreatedObjectUndo(manager, "Add Game Mode Manager");
            manager.AddComponent<GameModeManager>();
            Selection.activeGameObject = manager;

            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);

            Debug.Log("[Game Mode] Game Mode Manager added to MainScene and saved.");
        }
    }
}
#endif
