using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class FeedbackFlashHUD : MonoBehaviour
    {
        [Header("References")] [Tooltip("Image component of the flash")]
        public Image FlashImage;

        [Tooltip("CanvasGroup to fade the damage flash, used when recieving damage end healing")]
        public CanvasGroup FlashCanvasGroup;

        [Tooltip("CanvasGroup to fade the critical health vignette")]
        public CanvasGroup VignetteCanvasGroup;

        [Header("Damage")] [Tooltip("Color of the damage flash")]
        public Color DamageFlashColor;

        [Tooltip("Duration of the damage flash")]
        public float DamageFlashDuration;

        [Tooltip("Max alpha of the damage flash")]
        public float DamageFlashMaxAlpha = 1f;

        [Header("Critical health")] [Tooltip("Max alpha of the critical vignette")]
        public float CriticaHealthVignetteMaxAlpha = .8f;

        [Tooltip("Frequency at which the vignette will pulse when at critical health")]
        public float PulsatingVignetteFrequency = 4f;

        [Header("Heal")] [Tooltip("Color of the heal flash")]
        public Color HealFlashColor;

        [Tooltip("Duration of the heal flash")]
        public float HealFlashDuration;

        [Tooltip("Max alpha of the heal flash")]
        public float HealFlashMaxAlpha = 1f;

        bool m_FlashActive;
        float m_LastTimeFlashStarted = Mathf.NegativeInfinity;
        Health m_PlayerHealth;
        GameFlowManager m_GameFlowManager;

        void Start()
        {
            m_GameFlowManager = FindAnyObjectByType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, FeedbackFlashHUD>(m_GameFlowManager, this);

            TryBind();
        }

        void Update()
        {
            // The local networked player can spawn a frame or two after the HUD.
            if (m_PlayerHealth == null && !TryBind())
                return;

            if (m_PlayerHealth.IsCritical())
            {
                VignetteCanvasGroup.gameObject.SetActive(true);
                float vignetteAlpha =
                    (1 - (m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth /
                          m_PlayerHealth.CriticalHealthRatio)) * CriticaHealthVignetteMaxAlpha;

                if (m_GameFlowManager != null && m_GameFlowManager.GameIsEnding)
                    VignetteCanvasGroup.alpha = vignetteAlpha;
                else
                    VignetteCanvasGroup.alpha =
                        ((Mathf.Sin(Time.time * PulsatingVignetteFrequency) / 2) + 0.5f) * vignetteAlpha;
            }
            else
            {
                VignetteCanvasGroup.gameObject.SetActive(false);
            }


            if (m_FlashActive)
            {
                float normalizedTimeSinceDamage = (Time.time - m_LastTimeFlashStarted) / DamageFlashDuration;

                if (normalizedTimeSinceDamage < 1f)
                {
                    float flashAmount = DamageFlashMaxAlpha * (1f - normalizedTimeSinceDamage);
                    FlashCanvasGroup.alpha = flashAmount;
                }
                else
                {
                    FlashCanvasGroup.gameObject.SetActive(false);
                    m_FlashActive = false;
                }
            }
        }

        bool TryBind()
        {
            PlayerCharacterController player = LocalPlayerLocator.Find();
            if (player == null)
                return false;

            Health health = player.GetComponent<Health>();
            if (health == null)
                return false;

            if (m_PlayerHealth == health)
                return true;

            if (m_PlayerHealth != null)
            {
                m_PlayerHealth.OnDamaged -= OnTakeDamage;
                m_PlayerHealth.OnHealed -= OnHealed;
            }

            m_PlayerHealth = health;
            m_PlayerHealth.OnDamaged += OnTakeDamage;
            m_PlayerHealth.OnHealed += OnHealed;
            return true;
        }

        void OnDestroy()
        {
            if (m_PlayerHealth != null)
            {
                m_PlayerHealth.OnDamaged -= OnTakeDamage;
                m_PlayerHealth.OnHealed -= OnHealed;
            }
        }

        void ResetFlash()
        {
            m_LastTimeFlashStarted = Time.time;
            m_FlashActive = true;
            FlashCanvasGroup.alpha = 0f;
            FlashCanvasGroup.gameObject.SetActive(true);
        }

        void OnTakeDamage(float dmg, GameObject damageSource)
        {
            ResetFlash();
            FlashImage.color = DamageFlashColor;
        }

        void OnHealed(float amount)
        {
            ResetFlash();
            FlashImage.color = HealFlashColor;
        }
    }
}