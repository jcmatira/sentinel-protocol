#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Gameplay.EditorTools
{
    /// <summary>
    /// Optional helper: puts a GameModeManager GameObject inside Level01_Foundry and saves it, so the
    /// gameplay mode is visible and configurable in the Hierarchy.
    /// The mode also works at runtime without this step (see GameModeBootstrap).
    /// </summary>
    public static class GameModeSceneInstaller
    {
        const string k_Level1Path = "Assets/FPS/Scenes/Level01_Foundry.unity";

        [MenuItem("Tools/Game Mode/Install Game Mode Into Level 1")]
        public static void InstallIntoLevel1()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (activeScene.path != k_Level1Path)
            {
                EditorUtility.DisplayDialog(
                    "Game Mode",
                    "Open " + k_Level1Path + " first, then run this command again.",
                    "OK");
                return;
            }

            if (Object.FindAnyObjectByType<GameModeManager>() != null)
            {
                EditorUtility.DisplayDialog("Game Mode", "Level01_Foundry already contains a Game Mode Manager.", "OK");
                return;
            }

            GameObject manager = new GameObject("Game Mode Manager");
            Undo.RegisterCreatedObjectUndo(manager, "Add Game Mode Manager");
            manager.AddComponent<GameModeManager>();
            Selection.activeGameObject = manager;

            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);

            Debug.Log("[Game Mode] Game Mode Manager added to Level01_Foundry and saved.");
        }
    }
}
#endif
