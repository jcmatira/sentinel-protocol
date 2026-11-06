using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class StanceHUD : MonoBehaviour
    {
        [Tooltip("Image component for the stance sprites")]
        public Image StanceImage;

        [Tooltip("Sprite to display when standing")]
        public Sprite StandingSprite;

        [Tooltip("Sprite to display when crouching")]
        public Sprite CrouchingSprite;

        PlayerCharacterController m_Character;

        void Start()
        {
            TryBind();
        }

        void Update()
        {
            // The local networked player can spawn a frame or two after the HUD.
            if (m_Character == null)
                TryBind();
        }

        bool TryBind()
        {
            PlayerCharacterController character = LocalPlayerLocator.Find();
            if (character == null)
                return false;

            if (m_Character != null)
                m_Character.OnStanceChanged -= OnStanceChanged;

            m_Character = character;
            m_Character.OnStanceChanged += OnStanceChanged;
            OnStanceChanged(m_Character.IsCrouching);
            return true;
        }

        void OnDestroy()
        {
            if (m_Character != null)
                m_Character.OnStanceChanged -= OnStanceChanged;
        }

        void OnStanceChanged(bool crouched)
        {
            StanceImage.sprite = crouched ? CrouchingSprite : StandingSprite;
        }
    }
}
