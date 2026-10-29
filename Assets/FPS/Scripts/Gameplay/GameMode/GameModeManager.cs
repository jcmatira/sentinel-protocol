using Unity.FPS.AI;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.AI;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// "Blackout Protocol" - a small, self-contained gameplay mode for the FPS Microgame.
    ///
    /// This script also drives the two-level campaign:
    ///   Level 1 (Level01_Foundry / FOUNDRY)  -> Level 2 (Level02_Observatory / OBSERVATORY) -> WinScene
    ///
    /// RULES
    ///  * Destroy every hostile bot in the level.
    ///  * You have a limited amount of time (TimeLimit). The clock counts down while playing.
    ///  * Every kill is worth ScorePerKill points and buys TimeBonusPerKill extra seconds.
    ///  * Sprint is disabled while the mode is active (heavy combat gear).
    ///  * You have one life: no respawn.
    ///
    /// VICTORY
    ///  * Every hostile bot is destroyed before the clock reaches 00:00.
    ///  * Leftover seconds are converted to bonus points.
    ///
    /// DEFEAT
    ///  * The player dies, OR
    ///  * The clock reaches 00:00.
    ///
    /// The script reuses the Microgame's own events (EnemyKillEvent / AllObjectivesCompletedEvent /
    /// PlayerDeathEvent) so the victory screen, the defeat screen and the fade-out still come from
    /// the stock GameFlowManager.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("FPS/Game Mode/Game Mode Manager")]
    public class GameModeManager : MonoBehaviour
    {
        public const string ScorePrefsKey = "BlackoutProtocol.Score";
        public enum Outcome
        {
            InProgress,
            Victory,
            Defeat
        }

        public enum EndReason
        {
            None,
            AllBotsDestroyed,
            TimeExpired,
            PlayerKilled
        }

        [Header("Mission")] [Tooltip("Name shown at the top of the mission HUD")]
        public string MissionName = "BLACKOUT PROTOCOL";

        [Tooltip("Objective the player must complete to win")]
        public string ObjectiveText = "Destroy every hostile bot before the clock runs out";

        [Tooltip("Seconds the player has to finish the mission")]
        public float TimeLimit = 150f;

        [Header("Level")] [Tooltip("Level number shown on the HUD and the intro banner")]
        public int LevelNumber = 1;

        [Tooltip("Level name shown on the HUD and the intro banner")]
        public string LevelName = "FOUNDRY";

        [Tooltip("Scene loaded when this level is won. Leave empty to use the stock WinScene.")]
        public string NextLevelSceneName = "";

        [Tooltip("Start with the score saved by the previous level")]
        public bool CarryScoreFromPreviousLevel = false;

        [Tooltip("Save the score so the next level can continue it")]
        public bool CarryScoreToNextLevel = true;

        [Header("Scoring")] [Tooltip("Points awarded for every enemy killed")]
        public int ScorePerKill = 100;

        [Tooltip("Extra seconds added to the clock for every enemy killed")]
        public float TimeBonusPerKill = 5f;

        [Tooltip("Points awarded for every second left on the clock when winning")]
        public int ScorePerRemainingSecond = 10;

        [Header("Rules")] [Tooltip("Rule: the player cannot sprint while this mode is active")]
        public bool DisableSprint = true;

        [Tooltip("Rule: the jetpack stays offline for the whole mission")]
        public bool DisableJetpack = true;

        [Header("References (auto-found when empty)")] [Tooltip("Player in the scene")]
        public PlayerCharacterController Player;

        [Tooltip("Enemy manager used to display how many bots are left")]
        public EnemyManager Enemies;

        GameModeHUD m_Hud;

        public float TimeRemaining { get; private set; }
        public int Kills { get; private set; }
        public int Score { get; private set; }
        public Outcome CurrentOutcome { get; private set; } = Outcome.InProgress;
        public EndReason Reason { get; private set; } = EndReason.None;

        public bool IsRunning => CurrentOutcome == Outcome.InProgress;

        /// <summary>Text shown as the level heading on the HUD and intro banner.</summary>
        public string LevelLabel => "LEVEL " + LevelNumber + " · " + LevelName;

        public int TotalEnemies => Enemies != null ? Enemies.NumberOfEnemiesTotal : 0;
        public int EnemiesRemaining => Enemies != null ? Enemies.NumberOfEnemiesRemaining : 0;

        /// <summary>Rules displayed on the HUD. Kept in one place so the on-screen text always matches the code.</summary>
        public string RulesText
        {
            get
            {
                string killRule = ScorePerKill + " pts & +" + TimeBonusPerKill.ToString("0.#") + "s per kill";
                string restrictions = DisableSprint ? "no sprint" : "sprint allowed";

                return "RULES  " + killRule + "  ·  " + restrictions + "\n" +
                       "WIN  all bots down     LOSE  death or 00:00";
            }
        }

        void Awake()
        {
            if (TimeLimit <= 0f)
                TimeLimit = 150f;

            if (Player == null)
                Player = FindAnyObjectByType<PlayerCharacterController>();

            if (Enemies == null)
                Enemies = FindAnyObjectByType<EnemyManager>();

            TimeRemaining = TimeLimit;

            ApplyActionRules();
        }

        /// <summary>
        /// Fallback configuration used when the mode auto-installs itself (no GameModeManager
        /// saved in the scene). It keeps the two levels playable even if the Level Setup tool
        /// was not run, and it never overrides values saved in the Inspector.
        /// </summary>
        public void ApplyLevelDefaults(string sceneName)
        {
            switch (sceneName)
            {
                case "Level01_Foundry":
                    LevelNumber = 1;
                    LevelName = "FOUNDRY";
                    TimeLimit = 150f;
                    NextLevelSceneName = "Level02_Observatory";
                    CarryScoreFromPreviousLevel = false;
                    CarryScoreToNextLevel = true;
                    break;

                case "Level02_Observatory":
                    LevelNumber = 2;
                    LevelName = "OBSERVATORY";
                    TimeLimit = 180f;
                    NextLevelSceneName = string.Empty;
                    CarryScoreFromPreviousLevel = true;
                    CarryScoreToNextLevel = true;
                    break;
            }
        }

        void Start()
        {
            // TimeRemaining is set here (not Awake) so configuration applied between
            // AddComponent and Start - e.g. ApplyLevelDefaults - is respected.
            TimeRemaining = TimeLimit;

            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
            EventManager.AddListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);

            ApplyLevelFlow();

            m_Hud = GameModeHUD.Create(this);

            // The Microgame's NavMeshSurface can add its data a frame after the enemies'
            // NavMeshAgents initialize in a build, which logs "Failed to create agent".
            // Re-enabling those agents once the navmesh exists fixes them.
            StartCoroutine(RecoverNavMeshAgents());
        }

        void ApplyLevelFlow()
        {
            if (CarryScoreFromPreviousLevel)
                Score = PlayerPrefs.GetInt(ScorePrefsKey, 0);

            if (string.IsNullOrEmpty(NextLevelSceneName))
                return;

            GameFlowManager flow = FindAnyObjectByType<GameFlowManager>();
            if (flow != null)
                flow.WinSceneName = NextLevelSceneName;
            else
                Debug.LogWarning("[Game Mode] No GameFlowManager found; cannot route to " + NextLevelSceneName);
        }

        System.Collections.IEnumerator RecoverNavMeshAgents()
        {
            yield return null;

            NavMeshAgent[] agents = FindObjectsByType<NavMeshAgent>();
            for (int i = 0; i < agents.Length; i++)
            {
                if (!agents[i].enabled || !agents[i].isOnNavMesh)
                {
                    agents[i].enabled = false;
                    agents[i].enabled = true;
                }
            }
        }

        void Update()
        {
            if (!IsRunning)
                return;

            TimeRemaining -= Time.deltaTime;

            if (TimeRemaining <= 0f)
            {
                TimeRemaining = 0f;
                Defeat(EndReason.TimeExpired);
            }
        }

        void OnEnemyKilled(EnemyKillEvent evt)
        {
            if (!IsRunning)
                return;

            Kills++;
            Score += ScorePerKill;
            TimeRemaining += TimeBonusPerKill;

            // small feedback toast, reusing the Microgame notification system
            DisplayMessageEvent message = Events.DisplayMessageEvent;
            message.Message = "+" + ScorePerKill + " pts  ·  +" + TimeBonusPerKill.ToString("0.#") + "s";
            message.DelayBeforeDisplay = 0f;
            EventManager.Broadcast(message);
        }

        void OnAllObjectivesCompleted(AllObjectivesCompletedEvent evt)
        {
            if (!IsRunning)
                return;

            // the stock "Eliminate all the enemies" objective was completed -> that is our victory condition
            Score += Mathf.RoundToInt(TimeRemaining) * ScorePerRemainingSecond;
            CurrentOutcome = Outcome.Victory;
            Reason = EndReason.AllBotsDestroyed;

            if (CarryScoreToNextLevel)
            {
                PlayerPrefs.SetInt(ScorePrefsKey, Score);
                PlayerPrefs.Save();
            }

            if (m_Hud != null)
                m_Hud.ShowEnd(true, "All hostile bots destroyed", Score);
        }

        void OnPlayerDeath(PlayerDeathEvent evt) => Defeat(EndReason.PlayerKilled);

        void Defeat(EndReason reason)
        {
            if (!IsRunning)
                return;

            CurrentOutcome = Outcome.Defeat;
            Reason = reason;

            if (m_Hud != null)
            {
                string subtitle = reason == EndReason.TimeExpired
                    ? "The clock ran out."
                    : "You were eliminated.";
                m_Hud.ShowEnd(false, subtitle, Score);
            }

            if (reason == EndReason.TimeExpired)
            {
                // The stock GameFlowManager already knows how to lose: it listens for this event,
                // fades to black and loads LoseScene. Reuse it instead of duplicating scene loading.
                EventManager.Broadcast(Events.PlayerDeathEvent);
            }
        }

        void ApplyActionRules()
        {
            if (Player == null)
                return;

            if (DisableSprint)
                Player.SprintSpeedModifier = 1f;

            if (DisableJetpack)
            {
                Jetpack jetpack = Player.GetComponent<Jetpack>();
                if (jetpack != null)
                    jetpack.enabled = false;
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
            EventManager.RemoveListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
        }

        void OnValidate()
        {
            TimeLimit = Mathf.Max(1f, TimeLimit);
            ScorePerKill = Mathf.Max(0, ScorePerKill);
            TimeBonusPerKill = Mathf.Max(0f, TimeBonusPerKill);
            ScorePerRemainingSecond = Mathf.Max(0, ScorePerRemainingSecond);
        }
    }
}
