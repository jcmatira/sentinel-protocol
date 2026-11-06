using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [Tooltip("Image component dispplaying current health")]
        public Image HealthFillImage;

        Health m_PlayerHealth;

        void Start()
        {
            TryBind();
        }

        void Update()
        {
            // The local networked player can spawn a frame or two after the HUD.
            if (m_PlayerHealth == null && !TryBind())
                return;

            HealthFillImage.fillAmount = m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth;
        }

        bool TryBind()
        {
            PlayerCharacterController player = LocalPlayerLocator.Find();
            if (player == null)
                return false;

            Health health = player.GetComponent<Health>();
            if (health == null)
                return false;

            m_PlayerHealth = health;
            return true;
        }
    }
}
