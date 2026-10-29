#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Unity.FPS.AI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.EditorTools
{
    /// <summary>
    /// Turns the two prefab-built levels that ship with the FPS Microgame into a small campaign:
    ///
    ///   IntroMenu -> LEVEL 1  FOUNDRY      (MainScene)
    ///             -> LEVEL 2  OBSERVATORY  (SecondaryScene)
    ///             -> WinScene
    ///
    /// The level geometry, props, enemies and pickups in both scenes are the Microgame's own
    /// prefabs (Room/Section/Dungeon kit, Enemy_*, Pickup_*, GameManager, Player...).
    /// This tool only adds / configures the GameModeManager component on each scene and
    /// registers SecondaryScene in the Build Settings.
    /// </summary>
    public static class TwoLevelSetup
    {
        const string k_Level1Path = "Assets/FPS/Scenes/MainScene.unity";
        const string k_Level2Path = "Assets/FPS/Scenes/SecondaryScene.unity";

        /// <summary>
        /// Keeps SecondaryScene in the Build Settings so the Level 1 -> Level 2 transition
        /// works even if the setup command below was not run yet.
        /// </summary>
        [InitializeOnLoadMethod]
        static void EnsureLevelScenesInBuildSettings()
        {
            EditorApplication.delayCall += () =>
            {
                List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
                if (scenes.Any(s => s.path == k_Level2Path))
                    return;

                scenes.Add(new EditorBuildSettingsScene(k_Level2Path, true));
                EditorBuildSettings.scenes = scenes.ToArray();
                Debug.Log("[Levels] Added " + k_Level2Path + " to the Build Settings.");
            };
        }

        [MenuItem("Tools/Levels/Setup Two-Level Campaign")]
        public static void Setup()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            bool level1Ok = ConfigureLevel(k_Level1Path, 1, "FOUNDRY", 150f, "SecondaryScene", false, true);
            bool level2Ok = ConfigureLevel(k_Level2Path, 2, "OBSERVATORY", 180f, string.Empty, true, true);

            if (!level1Ok || !level2Ok)
            {
                EditorUtility.DisplayDialog("Levels",
                    "One of the levels could not be configured. Check the Console for details.", "OK");
                return;
            }

            AddSceneToBuildSettings(k_Level2Path);
            AssetDatabase.SaveAssets();

            // leave the user on Level 1
            EditorSceneManager.OpenScene(k_Level1Path, OpenSceneMode.Single);

            Debug.Log("[Levels] Campaign ready: LEVEL 1 FOUNDRY (MainScene) -> LEVEL 2 OBSERVATORY (SecondaryScene) -> WinScene.");
        }

        static bool ConfigureLevel(string path, int number, string levelName, float timeLimit,
            string nextLevel, bool carryScoreIn, bool carryScoreOut)
        {
            Scene scene;
            try
            {
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            }
            catch (System.Exception e)
            {
                Debug.LogError("[Levels] Could not open " + path + ": " + e.Message);
                return false;
            }

            GameModeManager manager = Object.FindAnyObjectByType<GameModeManager>();
            if (manager == null)
            {
                GameObject go = new GameObject("Game Mode Manager");
                Undo.RegisterCreatedObjectUndo(go, "Create Game Mode Manager");
                manager = go.AddComponent<GameModeManager>();
            }

            manager.LevelNumber = number;
            manager.LevelName = levelName;
            manager.TimeLimit = timeLimit;
            manager.NextLevelSceneName = nextLevel;
            manager.CarryScoreFromPreviousLevel = carryScoreIn;
            manager.CarryScoreToNextLevel = carryScoreOut;

            EditorUtility.SetDirty(manager);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            int enemies = Object.FindObjectsByType<EnemyController>().Length;
            int pickups = Object.FindObjectsByType<Pickup>().Length;
            bool hasPlayer = Object.FindAnyObjectByType<PlayerCharacterController>() != null;
            bool hasFlow = Object.FindAnyObjectByType<GameFlowManager>() != null;

            Debug.Log("[Levels] " + levelName + ": Player=" + hasPlayer + ", GameFlow=" + hasFlow +
                      ", Bots=" + enemies + ", Pickups=" + pickups + ", Next=" +
                      (string.IsNullOrEmpty(nextLevel) ? "WinScene" : nextLevel));

            if (!hasPlayer || !hasFlow || enemies == 0)
            {
                Debug.LogWarning("[Levels] " + levelName +
                                 " is missing a player, a GameFlowManager or enemies, so it is not playable.");
                return false;
            }

            return true;
        }

        static void AddSceneToBuildSettings(string path)
        {
            List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();

            if (scenes.Any(s => s.path == path))
                return;

            scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[Levels] Added " + path + " to the Build Settings.");
        }
    }
}
#endif
