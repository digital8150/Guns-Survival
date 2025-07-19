﻿//=========== Copyright (c) GameBuilders, All rights reserved. ================
//
// Purpose: The Weapon Manager Script is complete and intuitive system to controls the character equipment.  
//          It handles weapons slots, items and routines to identify objects that can be used by the character, 
//          such as weapons, items, pickups and interactable objects.
//
//=============================================================================

using System.Collections;
using System.Collections.Generic;

using GameBuilders.FPSBuilder.Core.Equipment;
using GameBuilders.FPSBuilder.Core.Managers;
using GameBuilders.FPSBuilder.Core.Player;
using GameBuilders.FPSBuilder.Core.Weapons;
using GameBuilders.FPSBuilder.Interfaces;

using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable CS0649

namespace GameBuilders.FPSBuilder.Core.Inventory
{
    /// <summary>
    /// Weapon States.
    /// </summary>
    public enum WeaponState
    {
        Idle,
        Firing,
        Reloading,
        MeleeAttacking,
        Interacting
    }
    
    /// <summary>
    /// The Weapon Manager Script controls the character equipment. 
    /// </summary>
    [AddComponentMenu("FPS Builder/Managers/Weapon Manager"), DisallowMultipleComponent]
    public sealed class WeaponManager : MonoBehaviour
    {
        /// <summary>
        /// Defines the reference to the First Person Character Controller script.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("Defines the reference to the First Person Character Controller script.")]
        private FirstPersonCharacterController m_FPController;

        /// <summary>
        /// Defines the reference to the character’s Main Camera transform.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("Defines the reference to the character’s Main Camera transform.")]
        private Transform m_CameraTransformReference;

        /// <summary>
        /// Defines how far the character can search for interactive objects.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines how far the character can search for interactive objects.")]
        private float m_InteractionRadius = 2;

        /// <summary>
        /// Determines the GameObject tag identifier to ammo pickups.
        /// </summary>
        [SerializeField]
        [Tag(AllowUntagged = false)]
        [Tooltip("Determines the GameObject tag identifier to ammo pickups.")]
        private string m_AmmoTag = "Ammo";

        /// <summary>
        /// Determines the GameObject tag identifier to adrenaline packs pickups.
        /// </summary>
        [SerializeField]
        [Tag(AllowUntagged = false)]
        [Tooltip("Determines the GameObject tag identifier to adrenaline packs pickups.")]
        private string m_AdrenalinePackTag = "Adrenaline Pack";

        /// <summary>
        /// Sound played when the character pick up an Item or Weapon.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the character pick up an Item or Weapon.")]
        private AudioClip m_ItemPickupSound;

        /// <summary>
        /// Defines the volume of Item Pickup Sound played when the character pick up an Item or Weapon.
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Defines the volume of Item Pickup Sound played when the character pick up an Item or Weapon.")]
        private float m_ItemPickupVolume = 0.3f;

        /// <summary>
        /// When activated, the character will change their current weapon for others instantly.
        /// </summary>
        [SerializeField]
        [Tooltip("When activated, the character will change their current weapon for others instantly.")]
        private bool m_FastChangeWeapons;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<Gun> m_EquippedWeaponsList = new List<Gun>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<Gun> m_WeaponList = new List<Gun>();
        
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<AmmoInstance> m_AmmoList = new List<AmmoInstance>();

        /// <summary>
        /// The Default Weapon is equipped if the Equipped Weapons list is empty.
        /// </summary>
        [SerializeField]
        [Tooltip("The Default Weapon is equipped if the Equipped Weapons list is empty.")]
        private MeleeWeapon m_DefaultWeapon;

        /// <summary>
        /// Defines the reference to the Frag Grenade item.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines the reference to the Frag Grenade item.")]
        private Grenade m_FragGrenade;

        /// <summary>
        /// Defines the reference to the Adrenaline item.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines the reference to the Adrenaline item.")]
        private FirstAidKit m_Adrenaline;

        private bool m_ItemCoolDown;
        private bool m_Climbing;
        private bool m_OnLadder;

        private Camera m_Camera;
        private IWeapon m_CurrentWeapon;
        private AudioEmitter m_PlayerBodySource;

        private InputActionMap m_WeaponMap;
        private InputActionMap m_MovementMap;
        private InputAction m_NextWeaponAction;
        private InputAction m_PreviousWeaponAction;
        
        private InputAction m_LethalEquipmentAction;
        private InputAction m_TacticalEquipmentAction;
        
        private InputAction m_InteractAction;

        #region PROPERTIES

        /// <summary>
        /// Returns the current state of the equipped weapon.
        /// </summary>
        public WeaponState State
        {
            get
            {
                switch (m_CurrentWeapon)
                {
                    case null:
                        return WeaponState.Idle;
                    case Gun gun when gun.Idle:
                        return WeaponState.Idle;
                    case Gun gun when gun.Firing:
                        return WeaponState.Firing;
                    case Gun gun when gun.Reloading:
                        return WeaponState.Reloading;
                    case Gun gun when gun.MeleeAttacking:
                        return WeaponState.MeleeAttacking;
                    case Gun gun when gun.Interacting:
                        return WeaponState.Interacting;
                    case MeleeWeapon meleeWeapon when meleeWeapon.Idle:
                        return WeaponState.Idle;
                    case MeleeWeapon meleeWeapon when meleeWeapon.MeleeAttacking:
                        return WeaponState.MeleeAttacking;
                    case MeleeWeapon meleeWeapon when meleeWeapon.Interacting:
                        return WeaponState.Interacting;
                    default:
                        return WeaponState.Idle;
                }
            }
        }

        /// <summary>
        /// The current weapon type.
        /// </summary>
        public System.Type Type => m_CurrentWeapon?.GetType();

        /// <summary>
        /// The current accuracy.
        /// </summary>
        public float? Accuracy
        {
            get
            {
                if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                    return ((Gun)m_CurrentWeapon).Accuracy;
                return null;
            }
        }

        /// <summary>
        /// The amount of ammunition of the current weapon.
        /// </summary>
        public int CurrentAmmo
        {
            get
            {
                if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                    return ((Gun)m_CurrentWeapon).CurrentRounds;
                return -1;
            }
        }

        /// <summary>
        /// The amount of ammunition remaining for the current weapon.
        /// </summary>
        public int Magazines
        {
            get
            {
                if (m_CurrentWeapon is Gun gun)
                {
                    return GetAmmoInstance(gun.GunData.AmmoType).Amount;
                }
                return -1;
            }
        }

        /// <summary>
        /// The ID of the equipped gun.
        /// </summary>
        public int GunID
        {
            get
            {
                if (m_CurrentWeapon != null)
                    return m_CurrentWeapon.Identifier;
                return -1;
            }
        }

        /// <summary>
        /// The name of the equipped gun.
        /// </summary>
        public string GunName
        {
            get
            {
                if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                {
                    return ((Gun)m_CurrentWeapon).GunName;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// The fire mode of the equipped gun.
        /// </summary>
        public string FireMode
        {
            get
            {
                if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                    return (((Gun)m_CurrentWeapon).HasSecondaryMode ? ((Gun)m_CurrentWeapon).FireMode.ToString() : string.Empty);
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns true if the character can switch guns, false otherwise.
        /// </summary>
        public bool CanSwitch
        {
            get
            {
                if (m_CurrentWeapon == null)
                {
                    return true;
                }
                return m_CurrentWeapon != null && m_CurrentWeapon.CanSwitch;
            }
        }
        
        /// <summary>
        /// Returns the current equipped weapons list.
        /// </summary>
        public Gun[] EquippedWeapons => m_EquippedWeaponsList.ToArray();
        
        /// <summary>
        /// Returns the current equipped gun.
        /// </summary>
        public Gun CurrentGun
        {
            get
            {
                if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                {
                    return (Gun)m_CurrentWeapon;
                }

                return null;
            }
        }
        
        public FirstAidKit Adrenaline => m_Adrenaline;
        
        public Grenade FragGrenade => m_FragGrenade;

        /// <summary>
        /// Returns the current targeted object.
        /// </summary>
        public GameObject Target
        {
            get;
            private set;
        }

        /// <summary>
        /// The GameObject tag used for ammunition.
        /// </summary>
        public string AmmoTag => m_AmmoTag;

        /// <summary>
        /// The GameObject tag used for adrenaline packs.
        /// </summary>
        public string AdrenalinePackTag => m_AdrenalinePackTag;

        /// <summary>
        /// Returns true if the equipped gun is a shotgun, false otherwise.
        /// </summary>
        public bool IsShotgun
        {
            get
            {
                if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                    return ((Gun)m_CurrentWeapon).FireMode == GunData.FireMode.ShotgunAuto || ((Gun)m_CurrentWeapon).FireMode == GunData.FireMode.ShotgunSingle;
                return false;
            }
        }

        /// <summary>
        /// Returns true if the character has a free slot for a new gun, false otherwise.
        /// </summary>
        public bool HasFreeSlot
        {
            get
            {
                for (int i = 0, c = m_EquippedWeaponsList.Count; i < c; i++)
                {
                    if (!m_EquippedWeaponsList[i])
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region EDITOR

        public void AddWeaponSlot()
        {
            m_EquippedWeaponsList.Add(null);
        }

        public void RemoveWeaponSlot(int index)
        {
            m_EquippedWeaponsList.RemoveAt(index);
        }

        public void AddWeapon()
        {
            m_WeaponList.Add(null);
        }

        public void RemoveWeapon(int index)
        {
            m_WeaponList.RemoveAt(index);
        }

        #endregion

        private void Start()
        {
            m_Camera = m_CameraTransformReference.GetComponent<Camera>();

            // Disable all weapons
            if (m_DefaultWeapon != null)
                m_DefaultWeapon.Viewmodel.SetActive(false);

            for (int i = 0, c = m_WeaponList.Count; i < c; i++)
            {
                if (m_WeaponList[i] != null)
                    m_WeaponList[i].Viewmodel.SetActive(false);
            }

            if (m_FragGrenade != null)
                m_FragGrenade.gameObject.SetActive(false);

            if (m_Adrenaline != null)
                m_Adrenaline.gameObject.SetActive(false);

            if (m_EquippedWeaponsList.Count > 0)
            {
                for (int i = 0, c = m_EquippedWeaponsList.Count; i < c; i++)
                {
                    if (m_EquippedWeaponsList[i] == null)
                        continue;
                    
                    // Initialize the weapon ammo
                    m_EquippedWeaponsList[i].InitializeMagazineAsDefault();

                    if (m_CurrentWeapon == null)
                    {
                        Select(m_EquippedWeaponsList[i], m_EquippedWeaponsList[i].CurrentRounds);
                    }
                }
            }

            if (m_CurrentWeapon == null && m_DefaultWeapon != null)
            {
                Select(m_DefaultWeapon, 0);
            }

            CalculateWeight();

            // Input Bindings
            m_WeaponMap = GameplayManager.Instance.GetActionMap("Weapons");
            m_WeaponMap.Enable();

            m_NextWeaponAction = m_WeaponMap.FindAction("Next Weapon");
            m_PreviousWeaponAction = m_WeaponMap.FindAction("Previous Weapon");
            m_LethalEquipmentAction = m_WeaponMap.FindAction("Lethal Equipment");
            m_TacticalEquipmentAction = m_WeaponMap.FindAction("Tactical Equipment");

            m_MovementMap = GameplayManager.Instance.GetActionMap("Movement");
            m_MovementMap.Enable();

            m_InteractAction = m_MovementMap.FindAction("Interact");

            InvokeRepeating(nameof(Search), 0, 0.1f);
            m_PlayerBodySource = AudioManager.Instance.RegisterSource("[AudioEmitter] CharacterBody", transform.root, spatialBlend: 0);

            m_FPController.LadderEvent += ClimbingLadder;
        }

        /// <summary>
        /// Notifies the equipped weapon that the character is climbing a ladder.
        /// </summary>
        /// <param name="climbing">Is the character climbing?</param>
        private void ClimbingLadder(bool climbing)
        {
            if (m_Climbing == climbing)
                return;

            m_Climbing = climbing;
            if (m_OnLadder)
            {
                OnExitLadder();
            }
        }

        /// <summary>
        /// Deselect the current weapon to simulate climbing a ladder.
        /// </summary>
        private void OnEnterLadder()
        {
            m_OnLadder = true;
            m_ItemCoolDown = true;
            m_CurrentWeapon.Deselect();
        }

        /// <summary>
        /// Select the previous weapon and reactive all weapon features.
        /// </summary>
        private void OnExitLadder()
        {
            m_CurrentWeapon.Select();
            m_ItemCoolDown = false;
            m_OnLadder = false;
        }

        private void SelectByPreviousAndNextButtons()
        {
            int weaponIndex = GetEquippedWeaponIndexOnList(m_CurrentWeapon.Identifier);
            
            if (m_EquippedWeaponsList.Count > 1)
            {
                if (m_NextWeaponAction.triggered)
                {
                    int newIndex = ++weaponIndex % m_EquippedWeaponsList.Count;
                    if (m_EquippedWeaponsList[newIndex])
                        StartCoroutine(Switch(m_CurrentWeapon, m_EquippedWeaponsList[newIndex], m_EquippedWeaponsList[newIndex].CurrentRounds));
                }
                else if (m_PreviousWeaponAction.triggered)
                {
                    int newIndex = --weaponIndex < 0 ? m_EquippedWeaponsList.Count - 1 : weaponIndex;
                    if (m_EquippedWeaponsList[newIndex])
                        StartCoroutine(Switch(m_CurrentWeapon, m_EquippedWeaponsList[newIndex], m_EquippedWeaponsList[newIndex].CurrentRounds));
                }
            }
        }

        private void Update()
        {
            // Analyze the character's target
            SearchForWeapons();
            SearchForAmmo();
            SearchForAdrenaline();
            SearchInteractiveObjects();

            if (!m_FPController.Controllable)
                return;

            // Switch equipped weapons
            if (m_CurrentWeapon != null)
            {
                if (m_CurrentWeapon.CanSwitch)
                {
                    // If the character is climbing a ladder
                    if (m_Climbing && !m_OnLadder)
                    {
                        OnEnterLadder();
                    }
                    else
                    {
                        SelectByPreviousAndNextButtons();
                    }
                }
            }
            else
            {
                m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, GameplayManager.Instance.FieldOfView, Time.deltaTime * 10);
            }

            // Throw a grenade
            if (!m_ItemCoolDown)
            {
                if (m_LethalEquipmentAction.triggered && m_CurrentWeapon != null && m_CurrentWeapon.CanUseEquipment && m_FragGrenade && m_FragGrenade.Amount > 0)
                {
                    StartCoroutine(ThrowGrenade());
                }
            }

            // Use adrenaline
            if (!m_ItemCoolDown)
            {
                if (m_TacticalEquipmentAction.triggered && m_CurrentWeapon != null && m_CurrentWeapon.CanUseEquipment && m_Adrenaline && m_Adrenaline.Amount > 0)
                {
                    StartCoroutine(AdrenalineShot());
                }
            }
        }

        private IEnumerator ThrowGrenade()
        {
            m_ItemCoolDown = true;

            m_CurrentWeapon.Deselect();
            yield return new WaitForSeconds(m_CurrentWeapon.HideAnimationLength);
            m_CurrentWeapon.Viewmodel.SetActive(false);

            m_FragGrenade.gameObject.SetActive(true);
            m_FragGrenade.Init();
            m_FragGrenade.Use();

            yield return new WaitForSeconds(m_FragGrenade.UsageDuration);
            m_FragGrenade.gameObject.SetActive(false);

            m_CurrentWeapon.Viewmodel.SetActive(true);
            m_CurrentWeapon.Select();
            m_ItemCoolDown = false;
        }

        private IEnumerator AdrenalineShot()
        {
            m_ItemCoolDown = true;

            m_CurrentWeapon.Deselect();
            yield return new WaitForSeconds(m_CurrentWeapon.HideAnimationLength);
            m_CurrentWeapon.Viewmodel.SetActive(false);

            m_Adrenaline.gameObject.SetActive(true);
            m_Adrenaline.Init();
            m_Adrenaline.Use();

            yield return new WaitForSeconds(m_Adrenaline.UsageDuration);
            m_Adrenaline.gameObject.SetActive(false);

            m_CurrentWeapon.Viewmodel.SetActive(true);
            m_CurrentWeapon.Select();
            m_ItemCoolDown = false;
        }

        /// <summary>
        /// Checks the target object to analyze if it is a weapon.
        /// </summary>
        private void SearchForWeapons()
        {
            if (Target)
            {
                // Try to convert the target for a gun pickup.
                GunPickup target = Target.GetComponent<GunPickup>();

                // If the gun pickup is not null means that the target is actually a weapon.
                if (target)
                {
                    IWeapon weapon = GetWeaponByID(target.Identifier);

                    if (weapon == null)
                        return;

                    if (m_CurrentWeapon != null)
                    {
                        if (!m_CurrentWeapon.CanSwitch)
                            return;

                        if (IsEquipped(weapon))
                            return;

                        if (HasFreeSlot)
                        {
                            if (m_InteractAction.triggered)
                            {
                                EquipWeapon(GetWeaponIndexOnList(weapon.Identifier));
                                Destroy(target.transform.gameObject);
                                StartCoroutine(Change(m_CurrentWeapon, weapon, target.CurrentRounds));

                                m_PlayerBodySource.ForcePlay(m_ItemPickupSound, m_ItemPickupVolume);
                                CalculateWeight();
                            }
                        }
                        else
                        {
                            if (m_InteractAction.triggered)
                            {
                                UnequipWeapon(GetEquippedWeaponIndexOnList(m_CurrentWeapon.Identifier));
                                EquipWeapon(GetWeaponIndexOnList(weapon.Identifier));
                                StartCoroutine(DropAndChange(m_CurrentWeapon, weapon, target, target.CurrentRounds));

                                if (m_FastChangeWeapons)
                                    m_PlayerBodySource.ForcePlay(m_ItemPickupSound, m_ItemPickupVolume);

                                CalculateWeight();
                            }
                        }
                    }
                    else
                    {
                        if (HasFreeSlot)
                        {
                            if (m_InteractAction.triggered)
                            {
                                EquipWeapon(GetWeaponIndexOnList(weapon.Identifier));
                                Select(weapon, target.CurrentRounds);
                                Destroy(target.transform.gameObject);
                                CalculateWeight();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the target object to analyze if it is a ammo box.
        /// </summary>
        private void SearchForAmmo()
        {
            if (!m_ItemCoolDown && m_EquippedWeaponsList.Count > 0 && m_CurrentWeapon != null 
                && m_CurrentWeapon.CanUseEquipment && CanRefillAmmo())
            {
                if (Target)
                {
                    // If the target has the Ammo Tag
                    if (Target.CompareTag(m_AmmoTag))
                    {
                        if (m_InteractAction.triggered)
                        {
                            StartCoroutine(RefillAmmo());
                            Destroy(Target);        //Ammokit Destroy
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the target object to analyze if it is a adrenaline pack.
        /// </summary>
        private void SearchForAdrenaline()
        {
            if (!m_ItemCoolDown && m_EquippedWeaponsList.Count > 0 && m_CurrentWeapon != null && m_CurrentWeapon.CanUseEquipment)
            {
                if (Target)
                {
                    // If the target has the Adrenaline Tag
                    if (Target.CompareTag(m_AdrenalinePackTag) && m_Adrenaline.CanRefill)
                    {
                        if (m_InteractAction.triggered)
                        {
                            StartCoroutine(RefillItem(new Equipment.Equipment[] { m_Adrenaline }));
                            Destroy(Target);        //HealKit Destroy
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the target object to analyze if it is a interactive object.
        /// </summary>
        private void SearchInteractiveObjects()
        {
            if (!m_ItemCoolDown)
            {
                if (Target)
                {
                    IActionable target = Target.GetComponent<IActionable>();

                    if (target != null)
                    {
                        if (m_InteractAction.triggered)
                        {
                            StartCoroutine(Interact(target));
                        }
                    }
                }
            }
        }

        private IEnumerator RefillItem(Equipment.Equipment[] items)
        {
            m_ItemCoolDown = true;

            m_CurrentWeapon.Interact();
            yield return new WaitForSeconds(m_CurrentWeapon.InteractDelay);

            for (int i = 0; i < items.Length; i++)
            {
                items[i].Refill();
            }

            m_PlayerBodySource.ForcePlay(m_ItemPickupSound, m_ItemPickupVolume);

            yield return new WaitForSeconds(Mathf.Max(m_CurrentWeapon.InteractAnimationLength - m_CurrentWeapon.InteractDelay, 0));
            m_ItemCoolDown = false;
        }

        public bool CanRefillAmmo()
        {
            for (int i = 0, c = m_EquippedWeaponsList.Count; i < c; i++)
            {
                if (!m_EquippedWeaponsList[i]) 
                    continue;
                
                AmmoInstance ammoInstance = GetAmmoInstance(m_EquippedWeaponsList[i].GunData.AmmoType);
                if (ammoInstance.Amount != ammoInstance.MaxAmount)
                    return true;
            }

            if (m_FragGrenade.CanRefill)
                return true;
            
            return false;
        }
        
        /// <summary>
        /// Refills the character magazines for all equipped weapons.
        /// </summary>
        private IEnumerator RefillAmmo()
        {
            m_ItemCoolDown = true;

            m_CurrentWeapon.Interact();
            yield return new WaitForSeconds(m_CurrentWeapon.InteractDelay);

            for (int i = 0, c = m_EquippedWeaponsList.Count; i < c; i++)
            {
                if (!m_EquippedWeaponsList[i]) 
                    continue;
                
                AmmoInstance ammoInstance = GetAmmoInstance(m_EquippedWeaponsList[i].GunData.AmmoType);
                ammoInstance.Amount = ammoInstance.MaxAmount;
            }
            
            m_PlayerBodySource.ForcePlay(m_ItemPickupSound, m_ItemPickupVolume);

            // Also refill the grenades
            if (m_FragGrenade)
                m_FragGrenade.Refill();

            yield return new WaitForSeconds(Mathf.Max(m_CurrentWeapon.InteractAnimationLength - m_CurrentWeapon.InteractDelay, 0));
            m_ItemCoolDown = false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ammoType"></param>
        /// <returns></returns>
        public int GetAmmo(AmmoType ammoType)
        {
            return GetAmmoInstance(ammoType)?.Amount ?? 0;
        }

        private AmmoInstance GetAmmoInstance(AmmoType ammoType)
        {
            int c = m_AmmoList.Count;
            if (!ammoType && c <= 0) return null;

            for (int i = 0; i < c; i++)
            {
                if (m_AmmoList[i].Instance && m_AmmoList[i].Instance.GetInstanceID() == ammoType.GetInstanceID())
                {
                    return m_AmmoList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the requested amount of ammo of the ammo type.
        /// </summary>
        /// <param name="ammoType">Type of ammunition requested.</param>
        /// <param name="amount">Number of rounds requested by the gun.</param>
        public int RequestAmmunition(AmmoType ammoType, int amount)
        {
            AmmoInstance instance = GetAmmoInstance(ammoType);

            if (instance == null)
                return 0;
            
            if (instance.Amount >= amount)
            {
                instance.Amount -= amount;
                return amount;
            }

            int remainingAmmo = instance.Amount;
            instance.Amount = 0;
            return remainingAmmo;
        }

        private IEnumerator Interact(IActionable target)
        {
            if (m_CurrentWeapon != null && m_CurrentWeapon.CanUseEquipment)
            {
                m_ItemCoolDown = true;

                if (target.RequiresAnimation)
                {
                    m_CurrentWeapon.Interact();
                    yield return new WaitForSeconds(m_CurrentWeapon.InteractDelay);
                }

                target.Interact();

                yield return new WaitForSeconds(Mathf.Max(m_CurrentWeapon.InteractAnimationLength - m_CurrentWeapon.InteractDelay, 0));
                m_ItemCoolDown = false;
            }
            else if (m_CurrentWeapon == null)
            {
                target.Interact();
            }
        }

        /// <summary>
        /// Casts a ray forward trying to find any targetable object in front of the character. 
        /// </summary>
        private void Search()
        {
            Ray ray = new Ray(m_CameraTransformReference.position, m_CameraTransformReference.TransformDirection(Vector3.forward));

            RaycastHit[] results = new RaycastHit[4];
            int amount = Physics.SphereCastNonAlloc(ray, m_FPController.Radius, results, m_InteractionRadius,
                Physics.AllLayers, QueryTriggerInteraction.Collide);

            float dist = m_InteractionRadius;
            GameObject temp = null;

            for (int i = 0, l = results.Length; i < l; i++)
            {
                if (!results[i].collider)
                    continue;

                GameObject c = results[i].collider.gameObject;

                if (c.transform.root == transform.root)
                    continue;

                // Is the object visible?
                if (Physics.Linecast(m_CameraTransformReference.position, results[i].point, out RaycastHit hitInfo, Physics.AllLayers, QueryTriggerInteraction.Collide))
                {
                    if (hitInfo.collider.gameObject != c)
                        continue;
                }

                // Discard unnecessary objects.
                if (!c.CompareTag(m_AdrenalinePackTag) && !c.CompareTag(m_AmmoTag) && c.GetComponent<IActionable>() == null && c.GetComponent<GunPickup>() == null)
                    continue;

                if (results[i].distance > dist)
                    continue;

                temp = c;
                dist = results[i].distance;
            }

            Target = temp;
        }

        /// <summary>
        /// Switch the weapons the character is equipped with.
        /// </summary>
        /// <param name="current">The current weapon.</param>
        /// <param name="target">The desired weapon.</param>
        /// <param name="currentRounds"></param>
        private IEnumerator Switch(IWeapon current, IWeapon target, int currentRounds)
        {
            current.Deselect();
            yield return new WaitForSeconds(current.HideAnimationLength);

            current.Viewmodel.SetActive(false);
            Select(target, currentRounds);
        }

        /// <summary>
        /// Change the weapons the character is equipped with.
        /// </summary>
        /// <param name="current">The current weapon.</param>
        /// <param name="target">The desired weapon.</param>
        /// <param name="currentRounds"></param>
        private IEnumerator Change(IWeapon current, IWeapon target, int currentRounds)
        {
            current.Deselect();
            if (!m_FastChangeWeapons)
            {
                yield return new WaitForSeconds(current.HideAnimationLength);
            }

            current.Viewmodel.SetActive(false);
            Select(target, currentRounds);
        }

        /// <summary>
        /// Replace the current weapon for the target weapon and drop it.
        /// </summary>
        /// <param name="current">The current weapon.</param>
        /// <param name="target">The desired weapon.</param>
        /// <param name="drop">The current weapon Prefab.</param>
        /// <param name="currentRounds"></param>
        private IEnumerator DropAndChange(IWeapon current, IWeapon target, GunPickup drop, int currentRounds)
        {
            current.Deselect();
            if (!m_FastChangeWeapons)
            {
                yield return new WaitForSeconds(((Gun)current).HideAnimationLength);
            }

            if (((Gun)current).DroppablePrefab)
                // ReSharper disable once Unity.InefficientPropertyAccess
            {
                GameObject newGunPickup = Instantiate(((Gun)current).DroppablePrefab, drop.transform.position, drop.transform.rotation);
                newGunPickup.GetComponent<GunPickup>().CurrentRounds = ((Gun) current).CurrentRounds;
            }

            Destroy(drop.transform.gameObject);

            current.Viewmodel.SetActive(false);
            Select(target, currentRounds);
        }

        /// <summary>
        /// Select the target weapon.
        /// </summary>
        /// <param name="weapon">The weapon to be draw.</param>
        /// <param name="currentRounds"></param>
        private void Select(IWeapon weapon, int currentRounds)
        {
            m_CurrentWeapon = weapon;
            weapon.SetCurrentRounds(currentRounds);
            weapon.Viewmodel.SetActive(true);
            weapon.Select();
        }

        /// <summary>
        /// Calculates the weight the character is carrying on based on the equipped weapons.
        /// </summary>
        private void CalculateWeight()
        {
            float weight = 0;
            for (int i = 0, c = m_EquippedWeaponsList.Count; i < c; i++)
            {
                if (m_EquippedWeaponsList[i] && m_EquippedWeaponsList[i].GetType() == typeof(Gun))
                    weight += m_EquippedWeaponsList[i].Weight;
            }
            m_FPController.Weight = weight;
        }

        /// <summary>
        /// Makes the character equip a weapon based on its index on the list.
        /// </summary>
        /// <param name="index">The weapon index.</param>
        public void EquipWeapon(int index)
        {
            if (HasFreeSlot)
            {
                for (int i = 0, c = m_EquippedWeaponsList.Count; i < c; i++)
                {
                    if (m_EquippedWeaponsList[i])
                        continue;

                    m_EquippedWeaponsList[i] = m_WeaponList[index];
                    return;
                }
            }
        }

        /// <summary>
        /// Makes the character unequip a weapon based on its index on the list.
        /// </summary>
        /// <param name="index"></param>
        public void UnequipWeapon(int index)
        {
            m_EquippedWeaponsList[index] = null;
        }

        /// <summary>
        /// Is the weapon on the Equipped Weapons List?
        /// </summary>
        /// <param name="weapon">The target weapon.</param>
        public bool IsEquipped(IWeapon weapon)
        {
            if (m_DefaultWeapon)
            {
                if (weapon.Identifier == m_DefaultWeapon.Identifier)
                    return true;
            }

            for (int i = 0, c = m_EquippedWeaponsList.Count; i < c; i++)
            {
                if (!m_EquippedWeaponsList[i])
                    continue;

                if (m_EquippedWeaponsList[i].Identifier == weapon.Identifier)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetEquippedWeaponIndexOnList(int id)
        {
            for (int i = 0, c = m_EquippedWeaponsList.Count; i < c; i++)
            {
                if (!m_EquippedWeaponsList[i])
                    continue;

                if (m_EquippedWeaponsList[i].Identifier == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetWeaponIndexOnList(int id)
        {
            for (int i = 0, c = m_WeaponList.Count; i < c; i++)
            {
                if (!m_WeaponList[i])
                    continue;

                if (m_WeaponList[i].Identifier == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public IWeapon GetWeaponByID(int id)
        {
            for (int i = 0, c = m_WeaponList.Count; i < c; i++)
            {
                if (!m_WeaponList[i])
                    continue;

                if (m_WeaponList[i].Identifier == id)
                {
                    return m_WeaponList[i];
                }
            }
            return null;
        }
    }
}
