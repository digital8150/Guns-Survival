using GameBuilders.FPSBuilder.Core.Weapons;
using GameBuilders.FPSBuilder.Interfaces;

using TMPro;
using UnityEngine;

#pragma warning disable CS0649

namespace GameBuilders.MinimalistUI.Scripts.Equipment
{
    public class EquipmentUI : MonoBehaviour
    {
        [Header("Controller")]
        [SerializeField] 
        private UIController m_UIController;

        [Header("Slots")] 
        [SerializeField] 
        private GunIcon[] m_GunIcons;
        
        [SerializeField] 
        private SlotUI[] m_WeaponSlots;
        
        [SerializeField] 
        private SlotUI m_GrenadeSlot;

        [Header("Weapon")]
        [SerializeField]
        private TMP_Text m_CurrentAmmo;
        
        [SerializeField]
        private TMP_Text m_Magazines;

        [SerializeField] 
        private GameObject m_FireModeAction;
        
        [SerializeField] 
        private GameObject m_ReloadAction;

        [SerializeField] 
        private GameObject m_InteractAction;
        
        [SerializeField]
        private TMP_Text m_InteractActionText;

        [Header("Crosshair")] 
        [SerializeField] 
        private CrosshairUI m_NormalCrosshair;
        
        [SerializeField] 
        private CrosshairUI m_RoundedCrosshair;

        private void Update()
        {
            bool weaponEquipped = m_UIController.InventoryManager.CurrentGun;
            
            m_CurrentAmmo.gameObject.SetActive(weaponEquipped);
            m_Magazines.gameObject.SetActive(weaponEquipped);
            
            for (int i = 0; i < m_WeaponSlots.Length; i++)
            {
                if (m_UIController.InventoryManager.EquippedWeapons[i])
                {
                    m_WeaponSlots[i].SlotObject.SetActive(true);
                    int weaponId = m_UIController.InventoryManager.EquippedWeapons[i].Identifier;
                    int currentRounds = m_UIController.InventoryManager.EquippedWeapons[i].CurrentRounds;
                    bool selected = weaponId == m_UIController.InventoryManager.GunID;
                    
                    m_WeaponSlots[i].ItemIcon.sprite = GetWeaponIcon(weaponId);
                    m_WeaponSlots[i].Amount.text = currentRounds.ToString();
                    m_WeaponSlots[i].SetActiveWeapon(selected);

                    if (selected)
                    {
                        m_CurrentAmmo.text = currentRounds.ToString();
                        m_Magazines.text = m_UIController.InventoryManager.Magazines.ToString();
                    }
                }
                else
                {
                    m_WeaponSlots[i].SlotObject.SetActive(false);
                }
            }
            
            m_FireModeAction.SetActive(weaponEquipped && m_UIController.InventoryManager.FireMode != string.Empty);

            m_NormalCrosshair.SetActive(!m_UIController.FirstPersonCharacter.IsAiming && weaponEquipped && !m_UIController.InventoryManager.IsShotgun);
            m_RoundedCrosshair.SetActive(!m_UIController.FirstPersonCharacter.IsAiming && weaponEquipped && m_UIController.InventoryManager.IsShotgun);

            if (weaponEquipped)
            {
                int magazines = m_UIController.InventoryManager.CurrentGun.RoundsPerMagazine;
                m_ReloadAction.SetActive((int)(magazines * 0.25f) >= m_UIController.InventoryManager.CurrentAmmo);
                
                if (m_UIController.InventoryManager.IsShotgun)
                {
                    m_RoundedCrosshair.Move(m_UIController.InventoryManager.Accuracy);
                }
                else
                {
                    m_NormalCrosshair.Move(m_UIController.InventoryManager.Accuracy);
                }
            }

            int grenadeAmount = m_UIController.InventoryManager.FragGrenade.Amount;
            bool hasGrenades = m_UIController.InventoryManager.FragGrenade && grenadeAmount > 0;
            
            m_GrenadeSlot.SlotObject.SetActive(hasGrenades);
            if (hasGrenades)
            {
                m_GrenadeSlot.Amount.text = grenadeAmount.ToString();
            }

            if (m_UIController.InventoryManager.Target)
            {
                GunPickup gunPickup = m_UIController.InventoryManager.Target.GetComponent<GunPickup>();

                if (gunPickup)
                {
                    IWeapon weapon = m_UIController.InventoryManager.GetWeaponByID(gunPickup.Identifier);

                    if (!m_UIController.InventoryManager.IsEquipped(weapon))
                    {
                        m_InteractActionText.text = "TO PICK UP";
                        ShowInteractAction();
                    }
                }
                
                IActionable target = m_UIController.InventoryManager.Target.GetComponent<IActionable>();
                
                if (target != null)
                {
                    m_InteractActionText.text = "TO " + target.Message();
                    ShowInteractAction();
                }

                bool isAmmo =
                    m_UIController.InventoryManager.Target.CompareTag(m_UIController.InventoryManager.AmmoTag) &&
                    m_UIController.InventoryManager.CanRefillAmmo();
                if (isAmmo)
                {
                    m_InteractActionText.text = "TO REFILL AMMO";
                    ShowInteractAction();
                }

                //메디킷 상호작용 메세지 추가
                bool isMedkit =
                    m_UIController.InventoryManager.Target.CompareTag(m_UIController.InventoryManager.MedkitTag);
                if (isMedkit)
                {
                    m_InteractActionText.text = "TO USE MEDKIT";
                    ShowInteractAction();
                }

                if (!gunPickup && target == null && !isAmmo && !isMedkit)
                {
                    HideInteractAction();
                }
            }
            else
            {
                HideInteractAction();
            }
        }

        private void ShowInteractAction()
        {
            m_InteractAction.SetActive(true);
        }

        private void HideInteractAction()
        {
            m_InteractAction.SetActive(false);
        }
        
        private Sprite GetWeaponIcon (int id)
        {
            for (int i = 0; i < m_GunIcons.Length; i++)
            {
                if (!m_GunIcons[i].GunData)
                    continue;

                if (m_GunIcons[i].GunData.GetInstanceID() == id)
                    return m_GunIcons[i].Icon;
            }
            return null;
        }
    }
}
