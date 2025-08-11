//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using System;
using System.Collections;
using GameBuilders.FPSBuilder.Core.Animation;
using GameBuilders.FPSBuilder.Core.Inventory;
using GameBuilders.FPSBuilder.Core.Managers;
using GameBuilders.FPSBuilder.Core.Player;
using GameBuilders.FPSBuilder.Core.Surface;
using GameBuilders.FPSBuilder.Core.Surface.Extensions;
using GameBuilders.FPSBuilder.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

#pragma warning disable CS0649

namespace GameBuilders.FPSBuilder.Core.Weapons
{
    [AddComponentMenu("FPS Builder/Weapon/Gun"), DisallowMultipleComponent]
    public class Gun : MonoBehaviour, IWeapon
    {
        /// <summary>
        /// GunData Asset is a container responsible for defining individual weapon characteristics.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("GunData Asset is a container responsible for defining individual weapon characteristics.")]
        private GunData m_GunData;
        
        /// <summary>
        /// Defines the reference to the inventory system.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("Defines the reference to the inventory system.")]
        private WeaponManager m_InventoryManager;

        /// <summary>
        /// Defines the reference to the character’s Main Camera transform.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("Defines the reference to the character’s Main Camera transform.")]
        private Transform m_CameraTransformReference;

        /// <summary>
        /// Defines the reference to the First Person Character Controller.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("Defines the reference to the First Person Character Controller.")]
        private FirstPersonCharacterController m_FPController;

        /// <summary>
        /// Defines the reference to the Camera Animator.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("Defines the reference to the Camera Animator.")]
        private CameraAnimator m_CameraAnimationsController;

        /// <summary>
        /// The WeaponSwing component.
        /// </summary>
        [SerializeField]
        private WeaponSwing m_WeaponSwing = new WeaponSwing();

        /// <summary>
        /// The MotionAnimation component.
        /// </summary>
        [SerializeField]
        private MotionAnimation m_MotionAnimation = new MotionAnimation();

        [SerializeField] 
        private WeaponKickbackAnimation m_WeaponKickbackAnimation;
        
        [SerializeField] 
        private WeaponKickbackAnimation m_AimingWeaponKickbackAnimation;

        [SerializeField]
        private CameraKickbackAnimation m_CameraKickbackAnimation;
        
        [SerializeField]
        private CameraKickbackAnimation m_AimingCameraKickbackAnimation;

        /// <summary>
        /// The GunAnimator component.
        /// </summary>
        [SerializeField]
        private GunAnimator m_GunAnimator = new GunAnimator();

        /// <summary>
        /// The GunEffects component.
        /// </summary>
        [SerializeField]
        private GunEffects m_GunEffects = new GunEffects();

        private bool m_GunActive;
        private bool m_Aiming;
        private bool m_IsReloading;
        private bool m_Attacking;

        private WaitForSeconds m_ReloadDuration;
        private WaitForSeconds m_CompleteReloadDuration;

        private WaitForSeconds m_StartReloadDuration;
        private WaitForSeconds m_InsertInChamberDuration;
        private WaitForSeconds m_InsertDuration;
        private WaitForSeconds m_StopReloadDuration;

        private float m_FireInterval;
        private float m_NextFireTime;
        private float m_NextReloadTime;
        private float m_NextSwitchModeTime;
        private float m_NextInteractTime;
        private float m_Accuracy;

        private Camera m_Camera;
        private float m_IsShooting;
        private Vector3 m_NextShootDirection;

        private InputActionMap m_InputBindings;
        private InputAction m_FireAction;
        private InputAction m_AimAction;
        private InputAction m_ReloadAction;
        private InputAction m_MeleeAction;
        private InputAction m_FireModeAction;

        #region EDITOR

        /// <summary>
        /// Returns the ReloadMode used by this gun.
        /// </summary>
        public GunData.ReloadMode ReloadType => m_GunData != null ? m_GunData.ReloadType : GunData.ReloadMode.Magazines;

        /// <summary>
        /// Returns true if this gun has secondary firing mode, false otherwise.
        /// </summary>
        public bool HasSecondaryMode => m_GunData != null && m_GunData.SecondaryFireMode != GunData.FireMode.None;

        /// <summary>
        /// Returns true if this gun has a chamber, false otherwise.
        /// </summary>
        public bool HasChamber => m_GunData != null && m_GunData.HasChamber;

        /// <summary>
        /// Returns the name of the weapon displayed on the Inspector tab.
        /// </summary>
        public string InspectorName => m_GunData != null ? m_GunData.GunName : "No Name";

        #endregion

        #region GUN PROPERTIES

        /// <summary>
        /// Returns true if this weapon is ready to be replaced, false otherwise.
        /// </summary>
        public virtual bool CanSwitch
        {
            get
            {
                if (!m_FPController)
                    return false;

                return m_GunActive && m_NextSwitchModeTime < Time.time && !m_Attacking && m_NextInteractTime < Time.time && m_NextFireTime < Time.time;
            }
        }

        /// <summary>
        /// Returns true if the character is not performing an action that prevents him from using items, false otherwise.
        /// </summary>
        public virtual bool CanUseEquipment
        {
            get
            {
                if (!m_FPController)
                    return false;

                return m_GunActive && !IsAiming && !m_IsReloading && m_NextReloadTime < Time.time && m_FPController.State != MotionState.Running
                && m_NextFireTime < Time.time && m_NextSwitchModeTime < Time.time && !m_Attacking && m_NextInteractTime < Time.time;
            }
        }

        /// <summary>
        /// Returns true if the character is performing an action that prevents him from vaulting, false otherwise.
        /// </summary>
        public virtual bool IsBusy
        {
            get
            {
                if (!m_FPController)
                    return true;

                return !m_GunActive || IsAiming || m_IsReloading || m_NextReloadTime > Time.time || m_NextFireTime > Time.time
                || m_NextSwitchModeTime > Time.time || m_Attacking || m_NextInteractTime > Time.time;
            }
        }

        /// <summary>
        /// Returns true if the character is aiming with this weapon, false otherwise.
        /// </summary>
        public virtual bool IsAiming => m_GunAnimator.IsAiming;

        /// <summary>
        /// Returns the maximum number of rounds a magazine can hold.
        /// </summary>
        public int RoundsPerMagazine
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the current number of rounds that are in the magazine coupled to the gun.
        /// </summary>
        public int CurrentRounds
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public GunData GunData => m_GunData;

        /// <summary>
        /// Return the weapon identifier.
        /// </summary>
        public int Identifier => m_GunData != null ? m_GunData.GetInstanceID() : -1;

        /// <summary>
        /// Return the name of the gun.
        /// </summary>
        public string GunName => m_GunData != null ? m_GunData.GunName : "No Name";

        /// <summary>
        /// Returns the current accuracy of the gun.
        /// </summary>
        public float Accuracy => m_Accuracy;

        /// <summary>
        /// Returns the selected fire mode on the gun.
        /// </summary>
        public GunData.FireMode FireMode
        {
            get;
            protected set;
        }

        /// <summary>
        /// Returns the viewmodel (GameObject) of this gun.
        /// </summary>
        public GameObject Viewmodel => gameObject;

        /// <summary>
        /// Returns the dropped object when swapping the gun.
        /// </summary>
        public GameObject DroppablePrefab => m_GunData != null ? m_GunData.DroppablePrefab : null;

        /// <summary>
        /// Returns the weight of the gun.
        /// </summary>
        public float Weight => m_GunData != null ? m_GunData.Weight : 0;

        /// <summary>
        /// Returns the length of the gun.
        /// </summary>
        public float Size => m_GunData != null ? m_GunData.Size : 0;

        /// <summary>
        /// 
        /// </summary>
        public float HideAnimationLength => m_GunAnimator.HideAnimationLength;

        public float DrawAnimationLenght => m_GunAnimator.DrawAnimationLength;
        
        public float InteractAnimationLength => m_GunAnimator.InteractAnimationLength;
        public float InteractDelay => m_GunAnimator.InteractDelay;

        public bool Reloading => m_IsReloading || m_NextReloadTime > Time.time;
        public bool Firing => m_NextFireTime > Time.time;
        public bool MeleeAttacking => m_Attacking;
        public bool Interacting => m_NextInteractTime > Time.time;
        
        public bool Idle => !Reloading && !Firing && !MeleeAttacking && !Interacting;

        public bool OutOfAmmo => CurrentRounds == 0;

        #endregion

        protected virtual void Awake()
        {
            if (!m_GunData)
            {
                throw new Exception("Gun Controller was not assigned");
            }

            if (!m_CameraTransformReference)
            {
                throw new Exception("Camera Transform Reference was not assigned");
            }

            if (!m_CameraAnimationsController)
            {
                throw new Exception("Camera Animations Controller was not assigned");
            }

            if (!m_FPController)
            {
                throw new Exception("FPController was not assigned");
            }

            if (!m_InventoryManager)
            {
                throw new Exception("Inventory Manager was not assigned");
            }
        }

        private void Start()
        {
            FireMode = m_GunData.PrimaryFireMode;
            m_FireInterval = m_GunData.PrimaryRateOfFire;
            RoundsPerMagazine = m_GunData.RoundsPerMagazine;

            if (m_MotionAnimation.Lean)
                m_MotionAnimation.LeanAmount = 0;

            switch (m_GunData.ReloadType)
            {
                case GunData.ReloadMode.Magazines:
                    m_ReloadDuration = new WaitForSeconds(m_GunAnimator.ReloadAnimationLength);
                    m_CompleteReloadDuration = new WaitForSeconds(m_GunAnimator.FullReloadAnimationLength);
                break;
                case GunData.ReloadMode.BulletByBullet:
                    m_StartReloadDuration = new WaitForSeconds(m_GunAnimator.StartReloadAnimationLength);
                    m_InsertInChamberDuration = new WaitForSeconds(m_GunAnimator.InsertInChamberAnimationLength / 2);
                    m_InsertDuration = new WaitForSeconds(m_GunAnimator.InsertAnimationLength / 2);
                    m_StopReloadDuration = new WaitForSeconds(m_GunAnimator.StopReloadAnimationLength);
                break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            m_GunEffects.Init();
            InitSwing(transform);
            DisableShadowCasting();

            // Input Bindings
            m_InputBindings = GameplayManager.Instance.GetActionMap("Weapons");
            m_InputBindings.Enable();
            
            m_FireAction = m_InputBindings.FindAction("Fire");
            m_AimAction = m_InputBindings.FindAction("Aim");
            m_ReloadAction = m_InputBindings.FindAction("Reload");
            m_MeleeAction = m_InputBindings.FindAction("Melee");
            m_FireModeAction = m_InputBindings.FindAction("Fire Mode");

            m_FPController.PreJumpEvent += PrepareForJump;
            m_FPController.JumpEvent += WeaponJump;
            m_FPController.LandingEvent += WeaponLanding;
            m_FPController.VaultEvent += Vault;
            m_FPController.GettingUpEvent += GettingUp;
        }

        private void Update()
        {
            if (m_GunActive)
            {
                m_CameraAnimationsController.HoldBreath = m_GunAnimator.CanHoldBreath;
                m_FPController.IsAiming = m_GunAnimator.IsAiming;
                m_FPController.ReadyToVault = !IsBusy;
                m_IsShooting = Mathf.MoveTowards(m_IsShooting, 0, Time.deltaTime);
                m_GunAnimator.SetCrouchStatus(m_FPController.IsCrouched);
                m_MotionAnimation.StabiliseWeaponRecoil();
                
                if (m_FPController.Controllable)
                    HandleInput();
            }

            if (m_Aiming)
            {
                m_GunAnimator.Aim(true);
            }
            else
            {
                bool canSprint = m_FPController.State == MotionState.Running && !m_IsReloading && m_NextReloadTime < Time.time
                                 && m_NextSwitchModeTime < Time.time && m_NextFireTime < Time.time && m_GunActive && !m_Attacking
                                 && m_NextInteractTime < Time.time && !m_FPController.IsSliding;

                m_GunAnimator.Sprint(canSprint, m_FPController.IsSliding);
            }

            m_Accuracy = Mathf.Clamp(Mathf.MoveTowards(m_Accuracy, GetCurrentAccuracy(), Time.deltaTime *
                                    (m_IsShooting > 0 ? m_GunData.DecreaseRateByShooting : m_GunData.DecreaseRateByWalking)),
                                    m_GunData.BaseAccuracy, m_GunData.AIMAccuracy);

            bool canLean = !m_Attacking && m_FPController.State != MotionState.Running && m_NextInteractTime < Time.time
                                        && m_CameraAnimationsController != null && m_MotionAnimation.Lean;

            m_MotionAnimation.LeanAnimation(canLean ? m_CameraAnimationsController.LeanDirection : 0);

            m_WeaponSwing.Swing(transform.parent, m_FPController);
            m_MotionAnimation.MovementAnimation(m_FPController);
            m_MotionAnimation.BreathingAnimation(m_FPController.IsAiming ? 0 : 1);
        }

        private float GetCurrentAccuracy()
        {
            if (m_GunAnimator.IsAiming)
            {
                if (m_IsShooting > 0)
                    return m_GunData.HIPAccuracy;

                return m_FPController.State != MotionState.Idle ? m_GunData.HIPAccuracy : m_GunData.AIMAccuracy;
            }
            if (m_IsShooting > 0)
                return m_GunData.BaseAccuracy;

            return m_FPController.State != MotionState.Idle ? m_GunData.BaseAccuracy : m_GunData.HIPAccuracy;
        }

        public void InitializeMagazineAsDefault()
        {
            CurrentRounds = m_GunData.HasChamber ? m_GunData.RoundsPerMagazine + 1 : m_GunData.RoundsPerMagazine;
        }

        public void Select()
        {
            if (!m_GunAnimator.Initialized)
            {
                m_Camera = m_CameraTransformReference.GetComponent<Camera>();
                m_GunAnimator.Init(transform, m_Camera);
            }
            
            m_GunAnimator.Draw();
            StartCoroutine(Draw());
        }

        private IEnumerator Draw()
        {
            yield return new WaitForSeconds(m_GunAnimator.DrawAnimationLength);
            m_GunActive = true;
        }

        public void Deselect()
        {
            m_GunActive = false;
            m_Aiming = false;
            m_IsReloading = false;
            m_NextReloadTime = 0;
            m_FPController.IsAiming = false;
            m_FPController.ReadyToVault = false;
            m_IsShooting = 0;
            m_GunAnimator.Hide();
        }

        public void DeselectImmediately()
        {
            m_GunActive = false;
            m_Aiming = false;
            m_IsReloading = false;
            m_NextReloadTime = 0;
            m_FPController.IsAiming = false;
            m_FPController.ReadyToVault = false;
            m_IsShooting = 0;
        }

        public void SetCurrentRounds(int currentRounds)
        {
            CurrentRounds = currentRounds;
        }

        private void HandleInput()
        {
            // Restrictions:
            // Is firing = m_NextFireTime > Time.time
            // Is reloading = m_IsReloading || m_NextReloadTime > Time.time
            // Is empty = CurrentRounds == 0
            // Is running = m_FPController.State == MotionState.Running
            // Is attacking = m_Attacking
            // Is switching mode = m_NextSwitchModeTime > Time.time
            // Is interacting = m_NextInteractTime > Time.time
            // Can reload = Magazines > 0

            bool canShoot = !m_IsReloading && m_NextReloadTime < Time.time && m_NextFireTime < Time.time && CurrentRounds >= 0
                            && (m_FPController.State != MotionState.Running || m_FPController.IsSliding) && !m_Attacking
                            && m_NextSwitchModeTime < Time.time && m_NextInteractTime < Time.time;

            if (canShoot)
            {
                if (FireMode == GunData.FireMode.FullAuto || FireMode == GunData.FireMode.ShotgunAuto)
                {
                    if (m_FireAction.ReadValue<float>() > 0)
                    {
                        if (CurrentRounds == 0 && m_InventoryManager.GetAmmo(m_GunData.AmmoType) > 0)
                        {
                            Reload();
                        }
                        else
                        {
                            PullTheTrigger();
                        }
                    }
                }
                else if (FireMode == GunData.FireMode.Single || FireMode == GunData.FireMode.ShotgunSingle || FireMode == GunData.FireMode.Burst)
                {
                    if (m_FireAction.triggered)
                    {
                        if (CurrentRounds == 0 && m_InventoryManager.GetAmmo(m_GunData.AmmoType) > 0)
                        {
                            Reload();
                        }
                        else
                        {
                            PullTheTrigger();
                        }
                    }
                }
            }

            if (m_GunData.ReloadType == GunData.ReloadMode.BulletByBullet && m_IsReloading && m_NextReloadTime < Time.time && CurrentRounds > (m_GunData.HasChamber ? 1 : 0))
            {
                if (m_FireAction.triggered)
                {
                    m_IsReloading = false;
                    StartCoroutine(StopReload());
                }
            }

            bool canAim = !m_IsReloading && m_NextReloadTime < Time.time && m_FPController.State != MotionState.Running && !m_Attacking && m_NextInteractTime < Time.time;

            if (canAim)
            {
                if (GameplayManager.Instance.AimStyle == ActionMode.Toggle)
                {
                    if (m_AimAction.triggered && !m_Aiming)
                    {
                        m_Aiming = !m_Aiming;
                    }
                    else if (m_AimAction.triggered && m_Aiming && m_GunAnimator.IsAiming)
                    {
                        m_Aiming = !m_Aiming;
                    }
                }
                else
                {
                    m_Aiming = m_AimAction.ReadValue<float>() > 0;
                }
            }
            else
            {
                m_Aiming = false;
            }

            bool canReload = !m_IsReloading && m_NextReloadTime < Time.time && CurrentRounds < (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine) && m_InventoryManager.GetAmmo(m_GunData.AmmoType) > 0
                             && !m_Attacking && m_NextSwitchModeTime < Time.time && m_NextInteractTime < Time.time && m_NextFireTime < Time.time;

            if (canReload)
            {
                if (m_ReloadAction.triggered)
                {
                    Reload();
                }
            }

            bool canAttack = !m_Attacking && !m_IsReloading && m_NextReloadTime < Time.time && m_FPController.State != MotionState.Running && !IsAiming
                             && m_NextFireTime < Time.time && m_GunAnimator.CanMeleeAttack && m_NextSwitchModeTime < Time.time && m_NextInteractTime < Time.time;

            if (canAttack)
            {
                if (m_MeleeAction.triggered)
                {
                    StartCoroutine(MeleeAttack());
                }
            }

            bool canChangeFireMode = HasSecondaryMode && !m_Attacking && !m_IsReloading
                                     && m_NextReloadTime < Time.time && m_FPController.State != MotionState.Running && m_NextSwitchModeTime < Time.time
                                     && m_NextInteractTime < Time.time;

            if (canChangeFireMode)
            {
                if (m_FireModeAction.triggered)
                {
                    if (FireMode == m_GunData.PrimaryFireMode)
                    {
                        m_NextSwitchModeTime = Time.time + m_GunAnimator.SwitchModeAnimationLength;
                        m_GunAnimator.SwitchMode();

                        FireMode = m_GunData.SecondaryFireMode;
                        m_FireInterval = m_GunData.SecondaryRateOfFire;
                    }
                    else
                    {
                        m_NextSwitchModeTime = Time.time + m_GunAnimator.SwitchModeAnimationLength;
                        m_GunAnimator.SwitchMode();

                        FireMode = m_GunData.PrimaryFireMode;
                        m_FireInterval = m_GunData.PrimaryRateOfFire;
                    }
                }
            }
        }

        private void PullTheTrigger()
        {
            if (CurrentRounds > 0 && m_InventoryManager.GetAmmo(m_GunData.AmmoType) >= 0)
            {
                if (FireMode == GunData.FireMode.FullAuto || FireMode == GunData.FireMode.Single)
                {
                    m_NextFireTime = Time.time + m_FireInterval;
                    CurrentRounds--;

                    m_NextShootDirection = GetBulletSpread();
                    Shot();

                    m_IsShooting = 0.1f;

                    m_GunAnimator.Shot(CurrentRounds == 0);
                    m_GunEffects.Play();

                    m_MotionAnimation.WeaponRecoilAnimation(IsAiming ? m_AimingWeaponKickbackAnimation : m_WeaponKickbackAnimation);

                    if (m_CameraAnimationsController)
                    {
                        m_CameraAnimationsController.ApplyRecoil(IsAiming ? m_AimingCameraKickbackAnimation : m_CameraKickbackAnimation);
                    }
                }
                else if (FireMode == GunData.FireMode.ShotgunAuto || FireMode == GunData.FireMode.ShotgunSingle)
                {
                    m_NextFireTime = Time.time + m_FireInterval;
                    CurrentRounds--;

                    for (int i = 0; i < m_GunData.BulletsPerShoot; i++)
                    {
                        m_NextShootDirection = GetBulletSpread();
                        Shot();
                    }

                    m_IsShooting = 0.1f;

                    m_GunAnimator.Shot(CurrentRounds == 0);
                    m_GunEffects.Play();

                    m_MotionAnimation.WeaponRecoilAnimation(IsAiming ? m_AimingWeaponKickbackAnimation : m_WeaponKickbackAnimation);

                    if (m_CameraAnimationsController)
                    {
                        m_CameraAnimationsController.ApplyRecoil(IsAiming ? m_AimingCameraKickbackAnimation : m_CameraKickbackAnimation);
                    }

                }
                else if (FireMode == GunData.FireMode.Burst)
                {
                    m_NextFireTime = Time.time + m_FireInterval * (m_GunData.BulletsPerBurst + 1);
                    StartCoroutine(Burst());
                }
            }
            else
            {
                m_NextFireTime = Time.time + 0.25f;
                m_GunAnimator.OutOfAmmo();
            }
        }

        private IEnumerator Burst()
        {
            for (int i = 0; i < m_GunData.BulletsPerBurst; i++)
            {
                if (CurrentRounds == 0)
                    break;

                m_NextShootDirection = GetBulletSpread();
                CurrentRounds--;
                Shot();

                m_IsShooting = 0.1f;

                m_GunAnimator.Shot(CurrentRounds == 0);
                m_GunEffects.Play();

                m_MotionAnimation.WeaponRecoilAnimation(IsAiming ? m_AimingWeaponKickbackAnimation : m_WeaponKickbackAnimation);

                if (m_CameraAnimationsController)
                {
                    m_CameraAnimationsController.ApplyRecoil(IsAiming ? m_AimingCameraKickbackAnimation : m_CameraKickbackAnimation);
                }
                yield return new WaitForSeconds(m_FireInterval);
            }
        }

        private void Shot()
        {
            Vector3 direction = m_CameraTransformReference.TransformDirection(m_NextShootDirection);
            Vector3 origin = m_CameraTransformReference.transform.position;

            Ray ray = new Ray(origin, direction);

            float tracerDuration = m_GunEffects.TracerDuration;

            if (Physics.Raycast(ray, out RaycastHit hitInfo, m_GunData.Range, m_GunData.AffectedLayers, QueryTriggerInteraction.Collide))
            {
                float damage = m_GunData.CalculateDamage(hitInfo.distance);
                
                //Debug.DrawLine(origin, hitInfo.point, Color.green, 10);
                
                SurfaceIdentifier surf = hitInfo.collider.GetSurface();
                bool hasSplintered = false;
                
                if (surf)
                {
                    bool canPenetrate = m_GunData.AmmoType.CanPenetrate && m_GunData.AmmoType.PenetrationPower > 0 && surf.CanPenetrate(hitInfo.triangleIndex);
                    float density = surf.Density(hitInfo.triangleIndex);
                
                    bool hasRicocheted = m_GunData.AmmoType.Ricochet && (!canPenetrate || density >= m_GunData.AmmoType.RicochetDensityThreshold) && m_GunData.AmmoType.RicochetChance > 0 && Random.Range(0.0f, 1.0f) <= m_GunData.AmmoType.RicochetChance;
                    hasSplintered  = m_GunData.AmmoType.Fragmentation && canPenetrate && m_GunData.AmmoType.FragmentationChance > 0 && Random.Range(0.0f, 1.0f) <= m_GunData.AmmoType.FragmentationChance;

                    // Generates a bullet mark
                    BulletDecalsManager.Instance.CreateBulletDecal(surf, hitInfo);

                    if (hasRicocheted && Vector3.Angle(direction, hitInfo.normal) - 90 <= m_GunData.AmmoType.MaxIncidentAngle)
                    {
                        Vector3 reflection = direction - 2 * Vector3.Dot(direction, hitInfo.normal) * hitInfo.normal;
                        Vector3 ricochetDirection = reflection;
                        
                        if (m_GunData.AmmoType.TrajectoryDeflection > 0)
                        {
                            float minDeflection = 0.5f - m_GunData.AmmoType.TrajectoryDeflection * 0.5f;
                            float maxDeflection = 1 - minDeflection;
                            
                            ricochetDirection = new Vector3
                            {
                                x = reflection.x * Random.Range(Mathf.Min(minDeflection, maxDeflection), Mathf.Max(minDeflection, maxDeflection)),
                                y = reflection.y * Random.Range(Mathf.Min(minDeflection, maxDeflection), Mathf.Max(minDeflection, maxDeflection)),
                                z = reflection.z * Random.Range(Mathf.Min(minDeflection, maxDeflection), Mathf.Max(minDeflection, maxDeflection)),
                            };
                        }

                        float ricochetPower = Random.Range(0.01f, 0.25f);
                        float ricochetRange = (m_GunData.Range - hitInfo.distance) * ricochetPower;
                        Ricochet(hitInfo.point, ricochetDirection, m_GunData.AmmoType.PenetrationPower * ricochetPower, ricochetRange, damage * ricochetPower);

                        SurfaceType surfaceType = surf.GetSurfaceType(hitInfo.point, hitInfo.triangleIndex);
                        if (surfaceType)
                        {
                            AudioClip ricochetSound = surfaceType.GetRandomRicochetSound();
                            float ricochetVolume = surfaceType.BulletImpactVolume;
                            
                            AudioManager.Instance.PlayClipAtPoint(ricochetSound, hitInfo.point, 3, 15, ricochetVolume);
                        }
                    }
                    else
                    {
                        if (hasSplintered && density >= m_GunData.AmmoType.FragmentationDensityThreshold)
                        {
                            int fragments = Random.Range(1, m_GunData.AmmoType.MaxFragments + 1);
                            for (int i = 0; i < fragments; i++)
                            {
                                Vector3 newDirection;
                                if (m_GunData.AmmoType.FragmentScattering.sqrMagnitude > 0)
                                {
                                    newDirection = new Vector3
                                    {
                                        x = direction.x * Random.Range(m_GunData.AmmoType.FragmentScattering[0], m_GunData.AmmoType.FragmentScattering[1]),
                                        y = direction.y * Random.Range(m_GunData.AmmoType.FragmentScattering[0], m_GunData.AmmoType.FragmentScattering[1]),
                                        z = 1
                                    };
                                    
                                    newDirection = m_CameraTransformReference.TransformDirection(newDirection);
                                }
                                else
                                {
                                    newDirection = direction;
                                }

                                float fragmentPower = Random.Range(0.01f, 0.25f);
                                float fragmentDamage = damage * fragmentPower;
                                float fragmentRange = (m_GunData.Range - hitInfo.distance) * fragmentPower;
                                float fragmentPenetrationPower = m_GunData.AmmoType.PenetrationPower * fragmentPower;

                                if (m_GunData.AmmoType.PenetrationPower > 0)
                                {
                                    Penetrate(hitInfo, newDirection, surf, fragmentPenetrationPower, fragmentRange, fragmentDamage);
                                }
                            }
                        }
                        
                        if (canPenetrate)
                        {
                            Penetrate(hitInfo, direction, surf, m_GunData.AmmoType.PenetrationPower, m_GunData.Range - hitInfo.distance, damage);
                        }
                    }
                }
                
                if (hitInfo.transform.root != transform.root)
                {
                    IProjectileDamageable damageableTarget = hitInfo.collider.GetComponent<IProjectileDamageable>();
                    damageableTarget?.ProjectileDamage(hasSplintered ? damage * m_GunData.AmmoType.FragmentationDamageMultiplier : damage, transform.root.position, hitInfo.point, m_GunData.AmmoType.PenetrationPower);
                }

                // If hit a rigid body applies force to push.
                Rigidbody rigidBody = hitInfo.collider.attachedRigidbody;
                if (rigidBody)
                {
                    float impactForce = m_GunData.AmmoType.CalculateImpactForce(hitInfo.distance);
                    rigidBody.AddForce(direction * impactForce, ForceMode.Impulse);
                }

                tracerDuration = hitInfo.distance / m_GunEffects.TracerSpeed;
            }

            if (tracerDuration > 0.05f)
                m_GunEffects.CreateTracer(transform, direction, tracerDuration);
        }

        private void Ricochet(Vector3 origin, Vector3 direction, float penetrationPower, float range, float damage)
        {
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, range, m_GunData.AffectedLayers, QueryTriggerInteraction.Collide))
            {
                SurfaceIdentifier surf = hitInfo.collider.GetSurface();
                
                //Debug.DrawLine(origin, hitInfo.point, Color.red, 10);

                if (surf)
                {
                    BulletDecalsManager.Instance.CreateBulletDecal(surf, hitInfo);
                }

                IProjectileDamageable damageableTarget = hitInfo.collider.GetComponent<IProjectileDamageable>();
                damageableTarget?.ProjectileDamage(damage, transform.root.position, hitInfo.point, penetrationPower);
                
                // If hit a rigidbody applies force to push.
                Rigidbody rigidBody = hitInfo.collider.attachedRigidbody;
                if (rigidBody)
                {
                    float impactForce = m_GunData.AmmoType.CalculateImpactForce(hitInfo.distance);
                    rigidBody.AddForce(direction * impactForce, ForceMode.Impulse);
                }
            }
        }

        private void Penetrate(RaycastHit lastHitInfo, Vector3 direction, SurfaceIdentifier surf, float penetrationPower, float range, float damage)
        {
            SurfaceIdentifier newSurf = surf;
            float remainingPower = penetrationPower;

            while (newSurf && remainingPower > 0 && newSurf.CanPenetrate(lastHitInfo.triangleIndex))
            {
                const float distanceEpsilon = 0.01f;
                Ray ray = new Ray(lastHitInfo.point + direction * distanceEpsilon, direction);

                int affectedObjectID = lastHitInfo.collider.GetInstanceID();

                if (Physics.Raycast(ray, out RaycastHit hitInfo, range, m_GunData.AffectedLayers, QueryTriggerInteraction.Collide))
                {
                    // Get the surface type of the object.
                    newSurf = hitInfo.collider.GetSurface();

                    // Exit hole
                    Ray exitRay = new Ray(hitInfo.point, direction * -1);

                    if (Physics.Raycast(exitRay, out RaycastHit exitInfo, hitInfo.distance + distanceEpsilon, m_GunData.AffectedLayers, QueryTriggerInteraction.Collide))
                    {
                        surf = exitInfo.collider.GetSurface();
                        float density = surf.Density(lastHitInfo.triangleIndex);
                        float distanceTraveled = Vector3.Distance(lastHitInfo.point, exitInfo.point) * density;

                        // Does the bullet gets through?
                        if (penetrationPower > distanceTraveled)
                        {
                            //Debug.DrawLine(lastHitInfo.point, hitInfo.point, Color.blue, 10);
                            
                            BulletDecalsManager.Instance.CreateBulletDecal(newSurf, hitInfo);
                            
                            if (affectedObjectID == exitInfo.collider.GetInstanceID())
                            {
                                BulletDecalsManager.Instance.CreateBulletDecal(surf, exitInfo);
                            }
                            
                            // Make sure you don't hit yourself
                            if (hitInfo.transform.root != transform.root)
                            {
                                IProjectileDamageable damageableTarget = hitInfo.collider.GetComponent<IProjectileDamageable>();
                                damageableTarget?.ProjectileDamage(damage * (distanceTraveled / m_GunData.AmmoType.PenetrationPower), transform.root.position, exitInfo.point, m_GunData.AmmoType.PenetrationPower - distanceTraveled);
                            }

                            // If hit a rigidbody applies force to push.
                            Rigidbody rigidBody = hitInfo.collider.attachedRigidbody;
                            if (rigidBody)
                            {
                                float impactForce = m_GunData.AmmoType.CalculateImpactForce(hitInfo.distance);
                                rigidBody.AddForce(direction * impactForce, ForceMode.Impulse);
                            }

                            if (m_GunData.AmmoType.Refraction.sqrMagnitude > 0)
                            {
                                float densityInfluence = density * m_GunData.AmmoType.DensityInfluence;
                                Vector3 newDirection = new Vector3
                                {
                                    x = Random.Range(m_GunData.AmmoType.Refraction[0], m_GunData.AmmoType.Refraction[1]) * densityInfluence,
                                    y = Random.Range(m_GunData.AmmoType.Refraction[0], m_GunData.AmmoType.Refraction[1]) * densityInfluence,
                                    z = 1
                                };
                                direction = m_CameraTransformReference.TransformDirection(newDirection);
                            }

                            remainingPower = penetrationPower - distanceTraveled;
                            lastHitInfo = hitInfo;
                            penetrationPower = remainingPower;
                            range -= hitInfo.distance;
                            continue;
                        }
                    }
                }
                else
                {
                    // Exit hole
                    float maxDistance = penetrationPower / newSurf.Density(lastHitInfo.triangleIndex);

                    Ray exitRay = new Ray(lastHitInfo.point + direction * (maxDistance - distanceEpsilon), direction * -1);
                    if (Physics.Raycast(exitRay, out RaycastHit exitInfo, maxDistance, m_GunData.AffectedLayers, QueryTriggerInteraction.Collide))
                    {
                        if (affectedObjectID == exitInfo.collider.GetInstanceID()) 
                            BulletDecalsManager.Instance.CreateBulletDecal(newSurf, exitInfo);
                    }
                }

                break;
            }
        }

        private Vector3 GetBulletSpread()
        {
            if (Mathf.Abs(m_Accuracy - 1) < Mathf.Epsilon)
            {
                return new Vector3(0, 0, 1);
            }
            else
            {
                Vector2 randomPointInScreen = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * ((1 - m_Accuracy) * (m_GunData.MaximumSpread / 10));
                return new Vector3(randomPointInScreen.x, randomPointInScreen.y, 1);
            }
        }

        private IEnumerator MeleeAttack()
        {
            m_Attacking = true;
            m_GunAnimator.Melee();
            yield return new WaitForSeconds(m_GunAnimator.MeleeDelay);

            Vector3 direction = m_CameraTransformReference.TransformDirection(Vector3.forward);
            Vector3 origin = m_CameraTransformReference.transform.position;
            float range = m_GunData.Size * 0.5f + m_FPController.Radius;

            Ray ray = new Ray(origin, direction);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, range, m_GunData.AffectedLayers, QueryTriggerInteraction.Collide))
            {
                m_GunAnimator.Hit(hitInfo.point);
                
                if (hitInfo.transform.root != transform.root)
                {
                    IProjectileDamageable damageableTarget = hitInfo.collider.GetComponent<IProjectileDamageable>();
                    damageableTarget?.ProjectileDamage(m_GunData.MeleeDamage, transform.root.position, hitInfo.point, 0);
                }

                // If hit a rigidbody applies force to push.
                Rigidbody rigidBody = hitInfo.collider.attachedRigidbody;
                if (rigidBody)
                {
                    rigidBody.AddForce(direction * m_GunData.MeleeForce, ForceMode.Impulse);
                }
            }

            yield return new WaitForSeconds(m_GunAnimator.MeleeAnimationLength - m_GunAnimator.MeleeDelay);
            m_Attacking = false;
        }

        private void Reload()
        {
            if (m_GunData.ReloadType == GunData.ReloadMode.Magazines)
            {
                StartCoroutine(ReloadMagazines());

                if (CurrentRounds == 0 && m_GunEffects.FullReloadDrop)
                {
                    Invoke(nameof(DropMagazinePrefab), m_GunEffects.FullDropDelay);
                }
                else if (CurrentRounds > 0 && m_GunEffects.TacticalReloadDrop)
                {
                    Invoke(nameof(DropMagazinePrefab), m_GunEffects.TacticalDropDelay);
                }
            }
            else if (m_GunData.ReloadType == GunData.ReloadMode.BulletByBullet)
            {
                StartCoroutine(ReloadBulletByBullet());
            }
        }

        private IEnumerator ReloadMagazines()
        {
            m_IsReloading = true;

            m_GunAnimator.Reload(CurrentRounds > 0);

            yield return CurrentRounds == 0 ? m_CompleteReloadDuration : m_ReloadDuration;

            if (m_GunActive && m_IsReloading)
            {
                if (CurrentRounds > 0)
                {
                    int amount = (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine) - CurrentRounds;
                    CurrentRounds += m_InventoryManager.RequestAmmunition(m_GunData.AmmoType, amount);
                }
                else
                {
                    CurrentRounds += m_InventoryManager.RequestAmmunition(m_GunData.AmmoType, RoundsPerMagazine);
                }
            }

            m_IsReloading = false;
        }

        private IEnumerator ReloadBulletByBullet()
        {
            m_IsReloading = true;

            m_GunAnimator.StartReload(CurrentRounds > 0);
            
            if (CurrentRounds == 0)
            {
                yield return m_InsertInChamberDuration;
                CurrentRounds += m_InventoryManager.RequestAmmunition(m_GunData.AmmoType, 1);
                yield return m_InsertInChamberDuration;
            }
            else
            {
                yield return m_StartReloadDuration;
            }

            while (m_GunActive && CurrentRounds < (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine) && m_InventoryManager.GetAmmo(m_GunData.AmmoType) > 0 && m_IsReloading)
            {
                m_GunAnimator.Insert();
                yield return m_InsertDuration;

                if (m_GunActive && m_IsReloading)
                {
                    CurrentRounds += m_InventoryManager.RequestAmmunition(m_GunData.AmmoType, 1);
                }
                yield return m_InsertDuration;
            }

            if (m_GunActive && m_IsReloading)
            {
                StartCoroutine(StopReload());
            }
        }

        private IEnumerator StopReload()
        {
            m_GunAnimator.StopReload();
            m_IsReloading = false;
            m_NextReloadTime = m_GunAnimator.StopReloadAnimationLength + Time.time;
            yield return m_StopReloadDuration;
        }

        private void DropMagazinePrefab()
        {
            m_GunEffects.DropMagazine(m_FPController.GetComponent<Collider>());
        }

        private void InitSwing(Transform weaponSwing)
        {
            if (!weaponSwing.parent.name.Equals("WeaponSwing"))
            {
                Transform parent = weaponSwing.parent.Find("WeaponSwing");

                if (parent != null)
                {
                    weaponSwing.parent = parent;
                }
                else
                {
                    GameObject weaponController = new GameObject("WeaponSwing");
                    weaponController.transform.SetParent(weaponSwing.parent, false);
                    weaponSwing.parent = weaponController.transform;
                }
            }
            m_WeaponSwing.Init(weaponSwing.parent, m_MotionAnimation.ScaleFactor);
        }

        private void PrepareForJump()
        {
            if (m_GunActive)
                StartCoroutine(m_MotionAnimation.BraceForJumpAnimation.Play());
        }

        private void WeaponJump()
        {
            if (m_GunActive)
                StartCoroutine(m_MotionAnimation.JumpAnimation.Play());
        }

        private void WeaponLanding(float fallDamage)
        {
            if (m_GunActive)
                StartCoroutine(m_MotionAnimation.LandingAnimation.Play());
        }

        public void DisableShadowCasting()
        {
            // For each object that has a renderer inside the weapon gameObject
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.sharedMaterial.EnableKeyword("_VIEWMODEL");
            }
        }

        public void SetRelodSpeed(float speed)
        {
            if(m_GunAnimator == null)
            {
                return;
            }
            m_GunAnimator.ReloadSpeed = speed;
            m_GunAnimator.ReloadEmptySpeed = speed;
            m_ReloadDuration = new WaitForSeconds(m_GunAnimator.ReloadAnimationLength);
            m_CompleteReloadDuration = new WaitForSeconds(m_GunAnimator.FullReloadAnimationLength);
        }

        public virtual void Interact()
        {
            m_NextInteractTime = Time.time + Mathf.Max(InteractAnimationLength, InteractDelay);
            m_GunAnimator.Interact();
        }

        private void Vault()
        {
            if (m_GunActive)
                m_GunAnimator.Vault();
        }

        private void GettingUp()
        {
            if (m_GunActive && !IsBusy)
                m_GunAnimator.Vault();
        }

        #region WEAPON CUSTOMIZATION

        public void UpdateAiming(Vector3 aimingPosition, Vector3 aimingRotation, bool zoomAnimation = false, float aimFOV = 50)
        {
            m_GunAnimator.UpdateAiming(aimingPosition, aimingRotation, zoomAnimation, aimFOV);
        }

        public void UpdateFireSound(AudioClip[] fireSoundList)
        {
            m_GunAnimator.UpdateFireSound(fireSoundList);
        }

        public void UpdateMuzzleFlash(ParticleSystem muzzleParticle)
        {
            m_GunEffects.UpdateMuzzleBlastParticle(muzzleParticle);
        }

        public void UpdateRecoil(Vector3 minCameraRecoilRotation, Vector3 maxCameraRecoilRotation)
        {
            
        }

        public void UpdateRoundsPerMagazine(int roundsPerMagazine)
        {
            RoundsPerMagazine = roundsPerMagazine;
        }

        #endregion
    }
}
