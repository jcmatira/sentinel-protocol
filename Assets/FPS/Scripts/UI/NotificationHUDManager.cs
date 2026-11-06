using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class NotificationHUDManager : MonoBehaviour
    {
        [Tooltip("UI panel containing the layoutGroup for displaying notifications")]
        public RectTransform NotificationPanel;

        [Tooltip("Prefab for the notifications")]
        public GameObject NotificationPrefab;

        PlayerWeaponsManager m_PlayerWeaponsManager;
        Jetpack m_Jetpack;
        bool m_Bound;

        void Start()
        {
            EventManager.AddListener<ObjectiveUpdateEvent>(OnObjectiveUpdateEvent);
            TryBind();
        }

        void Update()
        {
            // The local networked player can spawn a frame or two after the HUD.
            if (!m_Bound)
                TryBind();
        }

        bool TryBind()
        {
            PlayerCharacterController player = LocalPlayerLocator.Find();
            if (player == null)
                return false;

            PlayerWeaponsManager weaponsManager = player.GetComponent<PlayerWeaponsManager>();
            Jetpack jetpack = player.GetComponent<Jetpack>();
            if (weaponsManager == null || jetpack == null)
                return false;

            if (m_PlayerWeaponsManager != null)
                m_PlayerWeaponsManager.OnAddedWeapon -= OnPickupWeapon;
            if (m_Jetpack != null)
                m_Jetpack.OnUnlockJetpack -= OnUnlockJetpack;

            m_PlayerWeaponsManager = weaponsManager;
            m_Jetpack = jetpack;

            m_PlayerWeaponsManager.OnAddedWeapon += OnPickupWeapon;
            m_Jetpack.OnUnlockJetpack += OnUnlockJetpack;
            m_Bound = true;
            return true;
        }

        void OnObjectiveUpdateEvent(ObjectiveUpdateEvent evt)
        {
            if (!string.IsNullOrEmpty(evt.NotificationText))
                CreateNotification(evt.NotificationText);
        }

        void OnPickupWeapon(WeaponController weaponController, int index)
        {
            if (index != 0)
                CreateNotification("Picked up weapon : " + weaponController.WeaponName);
        }

        void OnUnlockJetpack(bool unlock)
        {
            CreateNotification("Jetpack unlocked");
        }

        public void CreateNotification(string text)
        {
            GameObject notificationInstance = Instantiate(NotificationPrefab, NotificationPanel);
            notificationInstance.transform.SetSiblingIndex(0);

            NotificationToast toast = notificationInstance.GetComponent<NotificationToast>();
            if (toast)
            {
                toast.Initialize(text);
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<ObjectiveUpdateEvent>(OnObjectiveUpdateEvent);

            if (m_PlayerWeaponsManager != null)
                m_PlayerWeaponsManager.OnAddedWeapon -= OnPickupWeapon;
            if (m_Jetpack != null)
                m_Jetpack.OnUnlockJetpack -= OnUnlockJetpack;
        }
    }
}
