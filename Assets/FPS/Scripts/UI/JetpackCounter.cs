using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class JetpackCounter : MonoBehaviour
    {
        [Tooltip("Image component representing jetpack fuel")]
        public Image JetpackFillImage;

        [Tooltip("Canvas group that contains the whole UI for the jetack")]
        public CanvasGroup MainCanvasGroup;

        [Tooltip("Component to animate the color when empty or full")]
        public FillBarColorChange FillBarColorChange;

        Jetpack m_Jetpack;

        void Awake()
        {
            FillBarColorChange.Initialize(1f, 0f);
        }

        void Start()
        {
            TryBind();
        }

        void Update()
        {
            // The local networked player can spawn a frame or two after the HUD.
            if (m_Jetpack == null && !TryBind())
                return;

            MainCanvasGroup.gameObject.SetActive(m_Jetpack.IsJetpackUnlocked);

            if (m_Jetpack.IsJetpackUnlocked)
            {
                JetpackFillImage.fillAmount = m_Jetpack.CurrentFillRatio;
                FillBarColorChange.UpdateVisual(m_Jetpack.CurrentFillRatio);
            }
        }

        bool TryBind()
        {
            PlayerCharacterController player = LocalPlayerLocator.Find();
            if (player == null)
                return false;

            Jetpack jetpack = player.GetComponent<Jetpack>();
            if (jetpack == null)
                return false;

            m_Jetpack = jetpack;
            return true;
        }
    }
}
