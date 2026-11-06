using System.Collections.Generic;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class WeaponHUDManager : MonoBehaviour
    {
        [Tooltip("UI panel containing the layoutGroup for displaying weapon ammo")]
        public RectTransform AmmoPanel;

        [Tooltip("Prefab for displaying weapon ammo")]
        public GameObject AmmoCounterPrefab;

        PlayerWeaponsManager m_PlayerWeaponsManager;
        readonly List<AmmoCounter> m_AmmoCounters = new List<AmmoCounter>();
        bool m_Bound;

        void Start()
        {
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
            if (weaponsManager == null)
                return false;

            if (weaponsManager == m_PlayerWeaponsManager)
            {
                m_Bound = true;
                return true;
            }

            Unbind();

            m_PlayerWeaponsManager = weaponsManager;
            m_PlayerWeaponsManager.OnAddedWeapon += AddWeapon;
            m_PlayerWeaponsManager.OnRemovedWeapon += RemoveWeapon;
            m_PlayerWeaponsManager.OnSwitchedToWeapon += ChangeWeapon;

            WeaponController activeWeapon = m_PlayerWeaponsManager.GetActiveWeapon();
            if (activeWeapon)
            {
                AddWeapon(activeWeapon, m_PlayerWeaponsManager.ActiveWeaponIndex);
                ChangeWeapon(activeWeapon);
            }

            m_Bound = true;
            return true;
        }

        void Unbind()
        {
            if (m_PlayerWeaponsManager != null)
            {
                m_PlayerWeaponsManager.OnAddedWeapon -= AddWeapon;
                m_PlayerWeaponsManager.OnRemovedWeapon -= RemoveWeapon;
                m_PlayerWeaponsManager.OnSwitchedToWeapon -= ChangeWeapon;
            }

            for (int i = 0; i < m_AmmoCounters.Count; i++)
            {
                if (m_AmmoCounters[i] != null)
                    Destroy(m_AmmoCounters[i].gameObject);
            }

            m_AmmoCounters.Clear();
            m_PlayerWeaponsManager = null;
            m_Bound = false;
        }

        void OnDestroy()
        {
            Unbind();
        }

        void AddWeapon(WeaponController newWeapon, int weaponIndex)
        {
            GameObject ammoCounterInstance = Instantiate(AmmoCounterPrefab, AmmoPanel);
            AmmoCounter newAmmoCounter = ammoCounterInstance.GetComponent<AmmoCounter>();
            DebugUtility.HandleErrorIfNullGetComponent<AmmoCounter, WeaponHUDManager>(newAmmoCounter, this,
                ammoCounterInstance.gameObject);

            newAmmoCounter.Initialize(newWeapon, weaponIndex, m_PlayerWeaponsManager);

            m_AmmoCounters.Add(newAmmoCounter);
        }

        void RemoveWeapon(WeaponController newWeapon, int weaponIndex)
        {
            int foundCounterIndex = -1;
            for (int i = 0; i < m_AmmoCounters.Count; i++)
            {
                if (m_AmmoCounters[i].WeaponCounterIndex == weaponIndex)
                {
                    foundCounterIndex = i;
                    Destroy(m_AmmoCounters[i].gameObject);
                }
            }

            if (foundCounterIndex >= 0)
            {
                m_AmmoCounters.RemoveAt(foundCounterIndex);
            }
        }

        void ChangeWeapon(WeaponController weapon)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(AmmoPanel);
        }
    }
}
