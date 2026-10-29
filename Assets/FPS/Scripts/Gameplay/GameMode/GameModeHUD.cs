using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// Runtime HUD for <see cref="GameModeManager"/>.
    /// Built entirely from code so the gameplay mode works without touching any prefab or canvas.
    /// </summary>
    [DisallowMultipleComponent]
    public class GameModeHUD : MonoBehaviour
    {
        static readonly Color k_PanelColor = new Color(0.02f, 0.04f, 0.07f, 0.62f);
        static readonly Color k_Accent = new Color(0.42f, 0.88f, 1f);
        static readonly Color k_Text = new Color(0.95f, 0.97f, 1f);
        static readonly Color k_Muted = new Color(0.62f, 0.70f, 0.79f);
        static readonly Color k_Danger = new Color(1f, 0.36f, 0.32f);
        static readonly Color k_Success = new Color(0.45f, 1f, 0.62f);

        static Font s_Font;

        GameModeManager m_Mode;

        Text m_TimeText;
        Text m_KillsText;
        Text m_ScoreText;

        CanvasGroup m_BannerGroup;
        Image m_BannerOverlay;
        Text m_BannerTitle;
        Text m_BannerSubtitle;
        Text m_BannerScore;

        CanvasGroup m_IntroGroup;
        float m_IntroTimer = 4f;

        GameObject m_PanelObject;

        int m_LastDisplayedSecond = int.MinValue;
        bool m_EndShown;

        public static GameModeHUD Create(GameModeManager mode)
        {
            GameObject go = new GameObject("Game Mode HUD");
            GameModeHUD hud = go.AddComponent<GameModeHUD>();
            hud.m_Mode = mode;
            hud.Build();
            return hud;
        }

        static Font GetFont()
        {
            if (s_Font == null)
            {
                s_Font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (s_Font == null)
                    s_Font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            }

            return s_Font;
        }

        static RectTransform CreateRect(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform) go.transform;
        }

        static void SetRect(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPosition, Vector2 size)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = size;
        }

        static Text CreateText(string name, Transform parent, int fontSize, FontStyle style,
            TextAnchor alignment, Color color, string content)
        {
            RectTransform rt = CreateRect(name, parent);
            Text text = rt.gameObject.AddComponent<Text>();
            text.font = GetFont();
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.text = content;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        void Build()
        {
            Canvas canvas;
            BuildCanvas(out canvas);

            // ---------- mission panel (top right) ----------
            RectTransform panel = CreateRect("Mission Panel", canvas.transform);
            SetRect(panel, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-24f, -24f), new Vector2(372f, 172f));

            Image panelBackground = panel.gameObject.AddComponent<Image>();
            panelBackground.color = k_PanelColor;
            panelBackground.raycastTarget = false;
            m_PanelObject = panel.gameObject;

            RectTransform accent = CreateRect("Accent", panel);
            SetRect(accent, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f),
                Vector2.zero, new Vector2(3f, 0f));
            Image accentImage = accent.gameObject.AddComponent<Image>();
            accentImage.color = k_Accent;
            accentImage.raycastTarget = false;

            Text missionTitle = CreateText("Mission Title", panel, 15, FontStyle.Bold, TextAnchor.UpperLeft,
                k_Accent, m_Mode.LevelLabel);
            SetRect(missionTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
                new Vector2(18f, -12f), new Vector2(-36f, 20f));

            m_TimeText = CreateText("Time", panel, 46, FontStyle.Bold, TextAnchor.UpperLeft, k_Text, "00:00");
            SetRect(m_TimeText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(18f, -32f), new Vector2(210f, 56f));

            m_KillsText = CreateText("Kills", panel, 17, FontStyle.Bold, TextAnchor.UpperRight, k_Text,
                "BOTS 0 / 0");
            SetRect(m_KillsText.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-18f, -38f), new Vector2(150f, 22f));

            m_ScoreText = CreateText("Score", panel, 17, FontStyle.Normal, TextAnchor.UpperRight, k_Muted,
                "SCORE 0000");
            SetRect(m_ScoreText.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-18f, -62f), new Vector2(150f, 22f));

            RectTransform divider = CreateRect("Divider", panel);
            SetRect(divider, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -94f), new Vector2(-36f, 1f));
            Image dividerImage = divider.gameObject.AddComponent<Image>();
            dividerImage.color = new Color(1f, 1f, 1f, 0.14f);
            dividerImage.raycastTarget = false;

            Text objective = CreateText("Objective", panel, 13, FontStyle.Normal, TextAnchor.UpperLeft, k_Text,
                m_Mode.ObjectiveText);
            SetRect(objective.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
                new Vector2(18f, -100f), new Vector2(-36f, 32f));

            Text rules = CreateText("Rules", panel, 11, FontStyle.Normal, TextAnchor.UpperLeft, k_Muted,
                m_Mode.RulesText);
            rules.lineSpacing = 1.05f;
            SetRect(rules.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
                new Vector2(18f, -134f), new Vector2(-36f, 30f));

            // ---------- end-of-mission banner ----------
            RectTransform banner = CreateRect("End Banner", canvas.transform);
            banner.anchorMin = Vector2.zero;
            banner.anchorMax = Vector2.one;
            banner.offsetMin = Vector2.zero;
            banner.offsetMax = Vector2.zero;

            m_BannerGroup = banner.gameObject.AddComponent<CanvasGroup>();
            m_BannerGroup.alpha = 0f;
            m_BannerGroup.blocksRaycasts = false;
            m_BannerGroup.interactable = false;

            RectTransform overlay = CreateRect("Overlay", banner);
            overlay.anchorMin = Vector2.zero;
            overlay.anchorMax = Vector2.one;
            overlay.offsetMin = Vector2.zero;
            overlay.offsetMax = Vector2.zero;
            m_BannerOverlay = overlay.gameObject.AddComponent<Image>();
            m_BannerOverlay.color = new Color(0f, 0f, 0f, 0.55f);
            m_BannerOverlay.raycastTarget = false;

            m_BannerTitle = CreateText("Title", banner, 72, FontStyle.Bold, TextAnchor.MiddleCenter, k_Success,
                string.Empty);
            SetRect(m_BannerTitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), new Vector2(0f, 70f), new Vector2(1500f, 110f));

            m_BannerSubtitle = CreateText("Subtitle", banner, 28, FontStyle.Normal, TextAnchor.MiddleCenter,
                k_Text, string.Empty);
            SetRect(m_BannerSubtitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), new Vector2(0f, 5f), new Vector2(1500f, 50f));

            m_BannerScore = CreateText("Score", banner, 24, FontStyle.Bold, TextAnchor.MiddleCenter, k_Muted,
                string.Empty);
            SetRect(m_BannerScore.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), new Vector2(0f, -55f), new Vector2(1500f, 40f));

            // ---------- level intro (fades away a few seconds after the level starts) ----------
            RectTransform intro = CreateRect("Level Intro", canvas.transform);
            SetRect(intro, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, 150f), new Vector2(1200f, 170f));

            m_IntroGroup = intro.gameObject.AddComponent<CanvasGroup>();
            m_IntroGroup.alpha = 1f;
            m_IntroGroup.blocksRaycasts = false;
            m_IntroGroup.interactable = false;

            Text mission = CreateText("Mission", intro, 16, FontStyle.Bold, TextAnchor.UpperCenter, k_Accent,
                m_Mode.MissionName);
            SetRect(mission.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                Vector2.zero, new Vector2(0f, 22f));
            AddShadow(mission);

            Text level = CreateText("Level", intro, 52, FontStyle.Bold, TextAnchor.UpperCenter, k_Text,
                m_Mode.LevelLabel);
            SetRect(level.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -24f), new Vector2(0f, 66f));
            AddShadow(level);

            Text introObjective = CreateText("Objective", intro, 20, FontStyle.Normal,
                TextAnchor.UpperCenter, k_Muted, m_Mode.ObjectiveText);
            SetRect(introObjective.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0.5f, 1f), new Vector2(0f, -96f), new Vector2(0f, 30f));
            AddShadow(introObjective);
        }

        static void AddShadow(Text text)
        {
            Shadow shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.85f);
            shadow.effectDistance = new Vector2(1.5f, -1.5f);
        }

        void BuildCanvas(out Canvas canvas)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            // Above the Microgame HUD (0) and above the end-game fade canvas (100) so the
            // mission-over banner stays readable while the screen fades to black.
            canvas.sortingOrder = 150;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        void Update()
        {
            if (m_Mode == null)
                return;

            if (m_IntroGroup != null && m_IntroGroup.alpha > 0f)
            {
                m_IntroTimer -= Time.unscaledDeltaTime;
                if (m_IntroTimer < 1f)
                    m_IntroGroup.alpha = Mathf.Clamp01(m_IntroTimer);
            }

            if (m_EndShown)
            {
                if (m_BannerGroup.alpha < 1f)
                    m_BannerGroup.alpha = Mathf.MoveTowards(m_BannerGroup.alpha, 1f, Time.unscaledDeltaTime * 3f);
                return;
            }

            int seconds = Mathf.CeilToInt(m_Mode.TimeRemaining);
            if (seconds != m_LastDisplayedSecond)
            {
                m_LastDisplayedSecond = seconds;
                m_TimeText.text = string.Format("{0:00}:{1:00}", seconds / 60, seconds % 60);
            }

            bool lowTime = m_Mode.IsRunning && m_Mode.TimeRemaining <= 15f;
            m_TimeText.color = lowTime
                ? Color.Lerp(k_Danger, k_Text, Mathf.PingPong(Time.unscaledTime * 2.5f, 1f))
                : k_Text;

            int total = Mathf.Max(m_Mode.TotalEnemies, m_Mode.Kills);
            m_KillsText.text = string.Format("BOTS {0} / {1}", m_Mode.Kills, total);
            m_ScoreText.text = string.Format("SCORE {0:0000}", m_Mode.Score);
        }

        public void ShowEnd(bool victory, string reason, int score)
        {
            m_EndShown = true;

            if (m_PanelObject != null)
                m_PanelObject.SetActive(false);

            if (m_IntroGroup != null)
                m_IntroGroup.alpha = 0f;

            m_BannerTitle.text = victory ? "MISSION COMPLETE" : "MISSION FAILED";
            m_BannerTitle.color = victory ? k_Success : k_Danger;
            m_BannerSubtitle.text = reason;
            m_BannerScore.text = "FINAL SCORE  " + score.ToString("0000");

            m_BannerOverlay.color = victory
                ? new Color(0.01f, 0.05f, 0.03f, 0.55f)
                : new Color(0.07f, 0.01f, 0.01f, 0.60f);

            m_BannerGroup.alpha = 0f;
        }
    }
}
