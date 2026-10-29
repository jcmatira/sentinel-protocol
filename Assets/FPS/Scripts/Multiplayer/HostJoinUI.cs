using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.Multiplayer
{
    /// <summary>
    /// Runtime LAN host / join panel shown on the IntroMenu.
    /// Built from code so no prefab or scene editing is required.
    /// </summary>
    [DisallowMultipleComponent]
    public class HostJoinUI : MonoBehaviour
    {
        static readonly Color k_Panel = new Color(0.02f, 0.04f, 0.07f, 0.88f);
        static readonly Color k_Accent = new Color(0.42f, 0.88f, 1f);
        static readonly Color k_Text = new Color(0.95f, 0.97f, 1f);
        static readonly Color k_Muted = new Color(0.62f, 0.70f, 0.79f);
        static readonly Color k_HostColor = new Color(0.18f, 0.52f, 0.32f, 1f);
        static readonly Color k_JoinColor = new Color(0.16f, 0.36f, 0.62f, 1f);
        static readonly Color k_LeaveColor = new Color(0.52f, 0.18f, 0.18f, 1f);

        static Font s_Font;

        Text m_Status;
        Text m_Hint;
        InputField m_Address;
        Button m_HostButton;
        Button m_JoinButton;
        Button m_DisconnectButton;

        void Awake() => Build();

        void Update()
        {
            NetworkSession session = NetworkSession.Instance;
            bool online = session != null && session.IsOnline;

            m_HostButton.interactable = !online;
            m_JoinButton.interactable = !online;
            m_DisconnectButton.interactable = online;

            string status = session != null ? session.StatusMessage : "Offline";
            if (m_Status.text != status)
                m_Status.text = status;

            string hint = session != null && session.IsHost
                ? "Host IP: " + NetworkSession.GetLocalIPv4() + "   Port: " + NetworkSession.DefaultPort
                : "Host: share your IP.   Join: enter the host IP.";
            if (m_Hint.text != hint)
                m_Hint.text = hint;
        }

        void OnHostClicked()
        {
            NetworkSession session = NetworkSession.EnsureExists();
            session.StartHost();
        }

        void OnJoinClicked()
        {
            NetworkSession session = NetworkSession.EnsureExists();
            session.StartClient(m_Address.text);
        }

        void OnDisconnectClicked()
        {
            if (NetworkSession.Instance != null)
                NetworkSession.Instance.Shutdown();
        }

        void Build()
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 300;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform panel = CreateRect("LAN Panel", canvas.transform);
            SetRect(panel, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-30f, -30f), new Vector2(400f, 258f));

            Image panelBackground = panel.gameObject.AddComponent<Image>();
            panelBackground.color = k_Panel;
            panelBackground.raycastTarget = false;

            RectTransform accent = CreateRect("Accent", panel);
            SetRect(accent, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                Vector2.zero, new Vector2(0f, 3f));
            Image accentImage = accent.gameObject.AddComponent<Image>();
            accentImage.color = k_Accent;
            accentImage.raycastTarget = false;

            Text title = CreateText("Title", panel, 18, FontStyle.Bold, TextAnchor.MiddleLeft, k_Accent,
                "LAN MULTIPLAYER");
            SetRect(title.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(18f, -12f), new Vector2(364f, 26f));

            m_Status = CreateText("Status", panel, 14, FontStyle.Normal, TextAnchor.MiddleLeft, k_Muted,
                "Offline");
            SetRect(m_Status.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(18f, -40f), new Vector2(364f, 24f));

            m_Address = CreateInputField(panel, "127.0.0.1");
            SetRect(m_Address.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(0f, 1f), new Vector2(18f, -74f), new Vector2(364f, 36f));

            m_HostButton = CreateButton(panel, "HOST", k_HostColor, OnHostClicked);
            SetRect(m_HostButton.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(0f, 1f), new Vector2(18f, -124f), new Vector2(172f, 40f));

            m_JoinButton = CreateButton(panel, "JOIN", k_JoinColor, OnJoinClicked);
            SetRect(m_JoinButton.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(0f, 1f), new Vector2(210f, -124f), new Vector2(172f, 40f));

            m_DisconnectButton = CreateButton(panel, "DISCONNECT", k_LeaveColor, OnDisconnectClicked);
            SetRect(m_DisconnectButton.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(0f, 1f), new Vector2(18f, -174f), new Vector2(364f, 34f));

            m_Hint = CreateText("Hint", panel, 11, FontStyle.Normal, TextAnchor.UpperLeft, k_Muted,
                string.Empty);
            SetRect(m_Hint.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(18f, -214f), new Vector2(364f, 34f));
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

        static InputField CreateInputField(Transform parent, string value)
        {
            RectTransform rt = CreateRect("Host Address", parent);
            Image background = rt.gameObject.AddComponent<Image>();
            background.color = new Color(0.06f, 0.08f, 0.11f, 0.95f);

            InputField field = rt.gameObject.AddComponent<InputField>();
            field.targetGraphic = background;
            field.lineType = InputField.LineType.SingleLine;
            field.characterLimit = 32;

            Text text = CreateText("Text", rt, 16, FontStyle.Normal, TextAnchor.MiddleLeft, k_Text,
                string.Empty);
            text.supportRichText = false;
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(10f, 3f);
            text.rectTransform.offsetMax = new Vector2(-10f, -3f);
            field.textComponent = text;

            Text placeholder = CreateText("Placeholder", rt, 16, FontStyle.Normal, TextAnchor.MiddleLeft,
                new Color(1f, 1f, 1f, 0.32f), "Host IP (e.g. 192.168.1.10)");
            placeholder.rectTransform.anchorMin = Vector2.zero;
            placeholder.rectTransform.anchorMax = Vector2.one;
            placeholder.rectTransform.offsetMin = new Vector2(10f, 3f);
            placeholder.rectTransform.offsetMax = new Vector2(-10f, -3f);
            field.placeholder = placeholder;

            field.text = value;
            return field;
        }

        static Button CreateButton(Transform parent, string label, Color color,
            UnityEngine.Events.UnityAction onClick)
        {
            RectTransform rt = CreateRect(label, parent);
            Image image = rt.gameObject.AddComponent<Image>();
            image.color = color;

            Button button = rt.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(onClick);

            Text text = CreateText("Label", rt, 15, FontStyle.Bold, TextAnchor.MiddleCenter, k_Text, label);
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;

            return button;
        }
    }
}
