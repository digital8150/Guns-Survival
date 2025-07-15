﻿//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using System.Collections;
using GameBuilders.FPSBuilder.Core.Player;
using GameBuilders.FPSBuilder.Core.Utilities;
using UnityEngine;

#pragma warning disable CS0649

namespace GameBuilders.FPSBuilder.Core.Animation
{
    /// <summary>
    /// Breath Animation Modes.
    /// </summary>
    public enum BreathAnimationMode
    {
        Unrestricted = 0,
        EnableWhenAim = 1,
        DisableWhenAim = 2
    }

    /// <summary>
    /// The Motion Animation class is responsible for all procedural animations performed on the character.
    /// </summary>
    [System.Serializable]
    public sealed class MotionAnimation
    {
        /// <summary>
        /// Determines the overall magnitude of the animation amplitude.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Determines the overall magnitude of the animation amplitude.")]
        private float m_ScaleFactor = 1;

        /// <summary>
        /// Define the amount of smoothness applied by this controller on the motion animation, useful to create natural-looking animations.
        /// </summary>
        [SerializeField]
        [MinMax(0.001f, Mathf.Infinity)]
        [Tooltip("Define the amount of smoothness applied by this controller on the motion animation, useful to create natural-looking animations.")]
        private float m_Smoothness = 1;

        /// <summary>
        /// Provides a set of rules used to simulate the walking animation on the Target Transform.
        /// </summary>
        [SerializeField]
        [Tooltip("Provides a set of rules used to simulate the walking animation on the Target Transform.")]
        private MotionData m_WalkingMotionData;
        
        /// <summary>
        /// Provides a set of rules used to simulate the walking animation on the Target Transform.
        /// </summary>
        [SerializeField]
        [Tooltip("Provides a set of rules used to simulate the walking animation on the Target Transform.")]
        private MotionData m_CrouchedMotionData;

        /// <summary>
        /// Provides a set of rules used to simulate the walking animation on the Target Transform (Used when the character has their legs injured).
        /// </summary>
        [SerializeField]
        [Tooltip("Provides a set of rules used to simulate the walking animation on the Target Transform (Used when the character has their legs injured).")]
        private MotionData m_BrokenLegsMotionData;

        /// <summary>
        /// Provides a set of rules used to simulate the running animation on the Target Transform.
        /// </summary>
        [SerializeField]
        [Tooltip("Provides a set of rules used to simulate the running animation on the Target Transform.")]
        private MotionData m_RunningMotionData;

        /// <summary>
        /// Defines the transform which will be animated.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("Defines the transform which will be animated.")]
        private Transform m_TargetTransform;

        #region JUMP & FALL ANIMATIONS

        /// <summary>
        /// Brace For Jump Animation is a routine that defines the transform’s target position and rotation,
        /// interpolating between them and returning to the original coordinates.
        /// This routine is used to simulate the animation of the character while it’s preparing to perform a jump, like channeling energy.
        /// </summary>
        [SerializeField]
        private LerpAnimation m_BraceForJumpAnimation = new LerpAnimation(new Vector3(0, -0.2f, 0), new Vector3(5, 0, 0), 0.25f, 0.2f);

        /// <summary>
        /// The Jump Animation is a routine that defines the transform’s target position and rotation,
        /// interpolating between them and returning to the original coordinates.
        /// This routine is used to simulate the character jumping animation.
        /// </summary>
        [SerializeField]
        private LerpAnimation m_JumpAnimation = new LerpAnimation(new Vector3(0, 0.05f, 0), new Vector3(10, 0, 0), 0.15f);

        /// <summary>
        /// The Landing Animation is a routine that defines the transform’s target position and rotation,
        /// interpolating between them and returning to the original coordinates.
        /// This routine is used to simulate the character landing animation, played after the character touches the ground.
        /// </summary>
        [SerializeField]
        private LerpAnimation m_LandingAnimation = new LerpAnimation(new Vector3(0, -0.075f, 0), Vector3.zero, 0.15f, 0.15f);

        #endregion

        #region DAMAGE

        /// <summary>
        /// Defines the point showing the smallest effect that will be applied when the character gets hit.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines the point showing the smallest effect that will be applied when the character gets hit.")]
        private Vector3 m_MinHitRotation = new Vector3(5, -5, 0);

        /// <summary>
        /// Defines the point indicating the maximum effect that will be applied when the character gets hit.
        /// The animation will be generated by computing a random point inside the minimum and maximum bounds, rotating the camera towards the calculated target direction.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines the point indicating the maximum effect that will be applied when the character gets hit."
            + " The animation will be generated by computing a random point inside the minimum and maximum bounds, rotating the camera towards the calculated target direction.")]
        private Vector3 m_MaxHitRotation = new Vector3(5, 5, 0);

        /// <summary>
        /// Defines how long the hit animation will last.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines how long the hit animation will last.")]
        private float m_HitDuration = 0.1f;

        #endregion

        #region BREATH

        /// <summary>
        /// Enables Breath Animation simulation.
        /// </summary>
        [SerializeField]
        [Tooltip("Enables Breath Animation simulation.")]
        private bool m_BreathAnimation = true;

        /// <summary>
        /// Defines how fast the breathing animation will be.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines how fast the breathing animation will be.")]
        private float m_BreathingSpeed = 2;

        /// <summary>
        /// Defines the how much the Target Transform will be affected by this animation by increasing the animation amplitude.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines the how much the Target Transform will be affected by this animation by increasing the animation amplitude.")]
        private float m_BreathingAmplitude = 1;

        #endregion

        #region RECOIL

        private WeaponKickbackAnimation m_WeaponKickbackAnimation;
        private Vector3 m_WeaponKickbackPos;
        private Vector3 m_WeaponKickbackRot;
        private Vector3 m_WeaponKickbackTargetPos;
        private Vector3 m_WeaponKickbackTargetRot;

        private float m_NextWeaponRecoilTime;
        private float m_NextRecoilTime;
        private float m_NextRecoilDirection;
        private float m_CurrentKickBack;
        private float m_CurrentRecoilRotation;
        
        private CameraKickbackAnimation m_CameraKickbackAnimation;
        private Vector3 m_CameraKickbackRot;
        
        #endregion

        #region EXPLOSION

        /// <summary>
        /// Struct that define how the tremor caused by an explosion close to the character will be simulated.
        /// </summary>
        [SerializeField]
        private ExplosionShakeProperties m_ExplosionShake;

        private const float k_MaxAngle = 90.0f;
        private Vector3 m_ExplosionPos;
        private Vector3 m_ExplosionRot;

        #endregion

        #region VAULT

        /// <summary>
        /// The Vault Animation is a routine that defines the transform’s target position and rotation,
        /// interpolating between them and returning to the original coordinates.
        /// This routine is used to simulate the character vaulting animation.
        /// </summary>
        [SerializeField]
        private LerpAnimation m_VaultAnimation = new LerpAnimation(Vector3.zero, new Vector3(-10, 0, 10), 0.3f, 0.3f);

        #endregion

        #region LEAN

        /// <summary>
        /// Enables peek and leaning functions on the character.
        /// </summary>
        [SerializeField]
        [Tooltip("Enables peek and leaning functions on the character.")]
        private bool m_Lean = true;

        /// <summary>
        /// Defines the horizontal offset while leaning.
        /// </summary>
        [SerializeField]
        [Range(0, 0.2f)]
        [Tooltip("Defines the horizontal offset while leaning.")]
        private float m_LeanAmount = 0.2f;

        /// <summary>
        /// Defines the angle (in degrees) that the target will be rotated.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines the angle (in degrees) that the target will be rotated.")]
        private float m_LeanAngle = 10;

        /// <summary>
        /// Defines how fast the animation will be played.
        /// </summary>
        [SerializeField]
        [MinMax(1, 20)]
        [Tooltip("Defines how fast the animation will be played.")]
        private float m_LeanSpeed = 5;

        #endregion

        // Variables used to simulate the animation.
        private Vector3 m_TargetPos;
        private Quaternion m_TargetRot;
        
        private Vector3 m_PosAnimated;
        private Vector3 m_RotAnimated;

        private float m_AnglePosition;
        private float m_AngleRotation;
        private float m_VerticalInfluence;

        private Vector3 m_CurrentPos;
        private Vector3 m_CurrentRot;

        private Vector3 m_DamageRot;

        private Vector3 m_LeanPos;
        private Vector3 m_LeanRot;

        private Vector3 m_BreathingRot;
        private float m_BreathingProgress;

        #region PROPERTIES

        /// <summary>
        /// The overall speed of this animator.
        /// </summary>
        public float BlendFactor
        {
            get => m_Smoothness;
            set => m_Smoothness = Mathf.Clamp(value, 0, Mathf.Infinity);
        }

        /// <summary>
        /// The overall magnitude of the animation amplitude.
        /// </summary>
        public float ScaleFactor => m_ScaleFactor;

        /// <summary>
        /// Struct that define how the tremor caused by an explosion close to the character will be simulated.
        /// </summary>
        public ExplosionShakeProperties ExplosionShake => m_ExplosionShake;

        /// <summary>
        /// Brace For Jump Animation is a routine that defines the transform’s target position and rotation,
        /// interpolating between them and returning to the original coordinates.
        /// routine is used to simulate the animation of the character while it’s preparing to perform a jump, like channeling energy.
        /// </summary>
        public LerpAnimation BraceForJumpAnimation => m_BraceForJumpAnimation;

        /// <summary>
        /// The Jump Animation is a routine that defines the transform’s target position and rotation,
        /// interpolating between them and returning to the original coordinates.
        /// This routine is used to simulate the character jumping animation.
        /// </summary>
        public LerpAnimation JumpAnimation => m_JumpAnimation;

        /// <summary>
        /// The Landing Animation is a routine that defines the transform’s target position and rotation,
        /// interpolating between them and returning to the original coordinates.
        /// This routine is used to simulate the character landing animation, played after the character touches the ground.
        /// </summary>
        public LerpAnimation LandingAnimation => m_LandingAnimation;

        /// <summary>
        /// The Vault Animation is a routine that defines the transform’s target position and rotation,
        /// interpolating between them and returning to the original coordinates.
        /// This routine is used to simulate the character vaulting animation.
        /// </summary>
        public LerpAnimation VaultAnimation => m_VaultAnimation;

        /// <summary>
        /// Enables peek and leaning functions on the character.
        /// </summary>
        public bool Lean => m_Lean;

        /// <summary>
        /// Defines the horizontal offset while leaning.
        /// </summary>
        public float LeanAmount
        {
            get => m_LeanAmount;
            set => m_LeanAmount = Mathf.Clamp(value, 0, 0.2f);
        }

        #endregion

        /// <summary>
        /// Breath animation simulates the natural movement of the arms while the character holds a weapon.
        /// </summary>
        /// <param name="speed">The animation speed.</param>
        public void BreathingAnimation(float speed = 1)
        {
            if (m_BreathAnimation)
            {
                // The animation progress
                CalculateAngle(ref m_BreathingProgress, m_BreathingSpeed, speed);

                if (speed > 0)
                {
                    float sin = Mathf.Sin(m_BreathingProgress);
                    float cos = Mathf.Cos(m_BreathingProgress);

                    // Calculates the target rotation using the values of sine and cosine multiplied by the animation magnitude.
                    Vector3 breathingRot = new Vector3(sin * cos * m_BreathingAmplitude, sin * m_BreathingAmplitude);

                    m_BreathingRot = Vector3.Lerp(m_BreathingRot, breathingRot, Time.deltaTime * 5 * m_BreathingSpeed * speed);
                }
                else
                {
                    m_BreathingRot = Vector3.Lerp(m_BreathingRot, Vector3.zero, Time.deltaTime * 5);
                }
            }
            else
            {
                m_BreathingRot = Vector3.Lerp(m_BreathingRot, Vector3.zero, Time.deltaTime * 5);
            }
        }

        /// <summary>
        /// The movement animation simulates the wave-like motion animation while the character is walking or running.
        /// </summary>
        public void MovementAnimation(FirstPersonCharacterController FPController)
        {
            if (!m_TargetTransform)
                return;

            // Stops the movement animation if the character is not walking or running.
            if (FPController.State == MotionState.Flying || FPController.State == MotionState.Idle || FPController.State == MotionState.Climbing || FPController.IsSliding)
            {
                PerformMovementAnimation(null, FPController.Velocity, 10);
            }
            else
            {
                // Calculates the animation speed
                float speed = Mathf.Max(FPController.State == MotionState.Running ? 6.5f : 2.75f, FPController.CurrentTargetForce);

                // Lowerbody damaged (broken legs)
                if (FPController.LowerBodyDamaged && !FPController.IsAiming)
                {
                    PerformMovementAnimation(m_BrokenLegsMotionData, FPController.Grounded ? FPController.Velocity : Vector3.zero, speed);
                }
                else
                {
                    switch (FPController.State)
                    {
                        case MotionState.Running:
                            PerformMovementAnimation(m_RunningMotionData, FPController.Grounded ? FPController.Velocity : Vector3.zero, speed);
                            break;
                        case MotionState.Crouched:
                            PerformMovementAnimation(m_CrouchedMotionData, FPController.Grounded ? FPController.Velocity : Vector3.zero, speed);
                            break;
                        case MotionState.Walking:
                            PerformMovementAnimation(m_WalkingMotionData, FPController.Grounded ? FPController.Velocity : Vector3.zero, speed);
                            break;
                    }
                }
            }
            
            m_TargetTransform.localPosition = m_TargetPos + (m_VaultAnimation.Position + m_WeaponKickbackPos + m_ExplosionPos + m_LeanPos) * m_ScaleFactor;

            m_TargetTransform.localRotation = m_TargetRot 
                                              * Quaternion.Euler(m_BreathingRot) 
                                              * Quaternion.Euler(m_CameraKickbackRot) 
                                              * Quaternion.Euler(m_WeaponKickbackRot)
                                              * Quaternion.Euler(m_ExplosionRot)
                                              * Quaternion.Euler(m_DamageRot) 
                                              * Quaternion.Euler(m_VaultAnimation.Rotation) * Quaternion.Euler(m_LeanRot);
        }

        /// <summary>
        /// Calculates the next angle based on the time elapsed since the last frame.
        /// </summary>
        /// <param name="angle">The reference angle to be updated.</param>
        /// <param name="animationSpeed">The current animation speed.</param>
        /// <param name="overallSpeed">The overall animator speed.</param>
        private void CalculateAngle(ref float angle, float animationSpeed, float overallSpeed)
        {
            if (angle >= Mathf.PI * 2)
            {
                angle -= Mathf.PI * 2;
            }

            // Sum the time elapsed since the last frame multiplied by the animation speed.
            angle += Time.deltaTime * animationSpeed * overallSpeed;
        }

        /// <summary>
        /// Movement animation is simulated by simple waves, such as sine and cosine. Using the information contained in MotionData, 
        /// we simulate the animation by interpolating the target transforms following the parameters defined in the asset.
        /// </summary>
        /// <param name="motionData">The MotionData Asset used to define the animation behaviour.</param>
        /// <param name="velocityInfluence">The current velocity of the character.</param>
        /// <param name="speed">The animation speed.</param>
        private void PerformMovementAnimation(MotionData motionData, Vector3 velocityInfluence, float speed)
        {
            // Brief explanation on how the procedural animation system works:
            // Basically we have two main variables "m_TargetPos" and "m_TargetRot",
            // they are the values used to interpolate the current position of the target transform.

            // What we need to do is to simulate each animation separately and then sum all its respective
            // values in the variable "m_CurrentPos" and "m_CurrentRot" and then Interpolate(Target, Current, Speed).
            // As we use LerpAnimation all animations will be simulated in parallel using coroutines.
            // This way we can simulate all animations in a single frame using only one transform.

            if (motionData)
            {
                CalculateAngle(ref m_AnglePosition, motionData.PositionSpeed, speed);
                CalculateAngle(ref m_AngleRotation, motionData.RotationSpeed, speed);

                float sinPos = Mathf.Sin(m_AnglePosition);
                float cosPos = Mathf.Cos(m_AnglePosition);
                
                float sinRot = Mathf.Sin(m_AngleRotation);
                float cosRot = Mathf.Cos(m_AngleRotation);
                
                // Calculates the velocity influence
                m_VerticalInfluence = Mathf.Lerp(m_VerticalInfluence, velocityInfluence.y * motionData.VerticalRotationAmplitude * motionData.VelocityInfluence, Time.deltaTime * speed);

                if (motionData.AnimatePosition)
                {
                    m_PosAnimated = new Vector3(sinPos * motionData.HorizontalPositionAmplitude,Mathf.Abs(cosPos) * motionData.VerticalPositionAmplitude, sinPos * motionData.DistalAmplitude);
                }
                else
                {
                    m_PosAnimated = Vector3.zero;
                }

                // Calculates the current position. (The sum of all the positions of the simulations)
                m_CurrentPos = (motionData.PositionOffset + m_PosAnimated) * m_ScaleFactor + m_JumpAnimation.Position + m_LandingAnimation.Position + m_BraceForJumpAnimation.Position;

                if (motionData.AnimateRotation)
                {
                    m_RotAnimated = new Vector3(
                                        (-Mathf.Abs(sinRot) * motionData.VerticalRotationAmplitude +
                                         motionData.VerticalRotationAmplitude)
                                        * motionData.VerticalRotationAnimationCurve.Evaluate(Mathf.Abs(cosRot)),
                                        cosRot * motionData.HorizontalRotationAmplitude, cosRot * -motionData.TiltAmplitude)
                                    + new Vector3(m_VerticalInfluence, 0, 0) + motionData.RotationOffset;
                }
                else
                {
                    m_RotAnimated = Vector3.zero;
                }
                
                // Calculates the current rotation. (The multiplication of all the rotations of the simulations)
                m_CurrentRot = m_RotAnimated + m_BraceForJumpAnimation.Rotation + m_JumpAnimation.Rotation + m_LandingAnimation.Rotation;
                
                // Interpolate from the last position/rotation to the new current position/rotation.
                float blending = 1 / m_Smoothness;
                m_TargetPos = Vector3.Lerp(m_TargetPos, m_CurrentPos, Time.deltaTime * blending * motionData.PositionSpeed * speed);
                m_TargetRot = Quaternion.Slerp(m_TargetRot, Quaternion.Euler(m_CurrentRot), Time.deltaTime * blending * motionData.RotationSpeed * speed);
            }
            else
            {
                float blending = 1 / m_Smoothness;
                m_TargetPos = Vector3.Lerp(m_TargetPos, m_JumpAnimation.Position + m_LandingAnimation.Position + m_BraceForJumpAnimation.Position, Time.deltaTime * blending * 5);
                m_TargetRot = Quaternion.Slerp(m_TargetRot, Quaternion.Euler(m_BraceForJumpAnimation.Rotation) *
                                                            Quaternion.Euler(m_JumpAnimation.Rotation)
                                                            * Quaternion.Euler(m_LandingAnimation.Rotation), Time.deltaTime * blending * 5);

                m_VerticalInfluence = 0;
                
                // Random angle of initialization for the next animation.
                m_AnglePosition = Random.Range(0, 10) % 2 == 0 ? 0 : Mathf.PI;
                m_AngleRotation = Random.Range(0, 10) % 2 == 0 ? 0 : Mathf.PI;
            }
        }

        /// <summary>
        /// Tilts the target slightly in a certain direction.
        /// </summary>
        /// <param name="direction">The desired direction.</param>
        public void LeanAnimation(int direction)
        {
            // direction = 1   -> Right
            // direction = -1  -> Left
            if (m_Lean)
            {
                switch (direction)
                {
                    case 1:
                        m_LeanPos = Vector3.Lerp(m_LeanPos, new Vector3(m_LeanAmount, 0, 0), Time.deltaTime * m_LeanSpeed);
                        m_LeanRot = Vector3.Lerp(m_LeanRot, new Vector3(0, 0, -m_LeanAngle), Time.deltaTime * m_LeanSpeed);
                        break;
                    case -1:
                        m_LeanPos = Vector3.Lerp(m_LeanPos, new Vector3(-m_LeanAmount, 0, 0), Time.deltaTime * m_LeanSpeed);
                        m_LeanRot = Vector3.Lerp(m_LeanRot, new Vector3(0, 0, m_LeanAngle), Time.deltaTime * m_LeanSpeed);
                        break;
                    default:
                        m_LeanPos = Vector3.Lerp(m_LeanPos, Vector3.zero, Time.deltaTime * m_LeanSpeed);
                        m_LeanRot = Vector3.Lerp(m_LeanRot, Vector3.zero, Time.deltaTime * m_LeanSpeed);
                        break;
                }
            }
            else
            {
                m_LeanPos = Vector3.zero;
                m_LeanRot = Vector3.zero;
            }
        }

        /// <summary>
        /// Method that simulates the effect of being hit by a bullet projectile.
        /// </summary>
        public IEnumerator HitAnimation()
        {
            Vector3 initialRot = MathfUtilities.RandomInsideBounds(m_MinHitRotation, m_MaxHitRotation);

            // Make the GameObject move to target slightly
            for (float t = 0f; t <= m_HitDuration; t += Time.deltaTime)
            {
                m_DamageRot = Vector3.Lerp(initialRot, initialRot, t / m_HitDuration);
                yield return new WaitForFixedUpdate();
            }

            // Make it move back to neutral
            for (float t = 0f; t <= m_HitDuration; t += Time.deltaTime)
            {
                m_DamageRot = Vector3.Lerp(initialRot, Vector3.zero, t / m_HitDuration);
                yield return new WaitForFixedUpdate();
            }

            m_DamageRot = Vector3.zero;
        }

        public void WeaponRecoilAnimation(WeaponKickbackAnimation weaponKickbackAnimation)
        {
            if (m_NextWeaponRecoilTime > Time.time || !weaponKickbackAnimation.Enabled) 
                return;

            m_WeaponKickbackAnimation = weaponKickbackAnimation;
            float xPos = m_WeaponKickbackAnimation.SidewaysForce * Random.Range(m_WeaponKickbackAnimation.SidewaysRandomness.x, m_WeaponKickbackAnimation.SidewaysRandomness.y);
            float yPos = m_WeaponKickbackAnimation.UpwardForce * Random.Range(m_WeaponKickbackAnimation.UpwardRandomness.x, m_WeaponKickbackAnimation.UpwardRandomness.y);
            float zPos = -m_WeaponKickbackAnimation.KickbackForce * Random.Range(m_WeaponKickbackAnimation.KickbackRandomness.x, m_WeaponKickbackAnimation.KickbackRandomness.y);
            m_WeaponKickbackTargetPos = new Vector3(xPos, yPos, zPos);
            
            float xRot = -m_WeaponKickbackAnimation.VerticalRotation * Random.Range(m_WeaponKickbackAnimation.VerticalRotationRandomness.x, m_WeaponKickbackAnimation.VerticalRotationRandomness.y);
            float yRot = m_WeaponKickbackAnimation.HorizontalRotation * Random.Range(m_WeaponKickbackAnimation.HorizontalRotationRandomness.x, m_WeaponKickbackAnimation.HorizontalRotationRandomness.y);
            m_WeaponKickbackTargetRot = new Vector3(xRot, yRot, 0);
            m_NextWeaponRecoilTime = Time.time + m_WeaponKickbackAnimation.KickbackDuration;
        }

        public void CameraRecoilAnimation (CameraKickbackAnimation cameraKickbackAnimation)
        {
            if (m_NextRecoilTime > Time.time || !cameraKickbackAnimation.Enabled) 
                return;
            
            m_CameraKickbackAnimation = cameraKickbackAnimation;
            m_NextRecoilDirection = Random.Range(m_CameraKickbackAnimation.KickbackRandomness.x, m_CameraKickbackAnimation.KickbackRandomness.y);
            m_CurrentKickBack = Mathf.Clamp(m_CurrentKickBack + m_CameraKickbackAnimation.Kickback * m_NextRecoilDirection, -m_CameraKickbackAnimation.MaxKickback, m_CameraKickbackAnimation.MaxKickback);
            m_CurrentRecoilRotation = Mathf.Clamp(Random.Range(m_CameraKickbackAnimation.HorizontalKickbackRandomness.x, m_CameraKickbackAnimation.HorizontalKickbackRandomness.y) * m_CameraKickbackAnimation.HorizontalKickback + m_CurrentRecoilRotation,  -m_CameraKickbackAnimation.HorizontalKickback, m_CameraKickbackAnimation.HorizontalKickback);
            m_NextRecoilTime = Time.time + cameraKickbackAnimation.KickbackDuration;
        }

        public void StabiliseCameraRecoil()
        {
            if (m_NextRecoilTime > Time.time)
            {
                Vector3 target = new Vector3(-m_CurrentKickBack, m_CurrentRecoilRotation, m_CurrentRecoilRotation * m_CameraKickbackAnimation.KickbackRotation);
                m_CameraKickbackRot = Vector3.MoveTowards(m_CameraKickbackRot, target, Time.deltaTime * m_CameraKickbackAnimation.KickbackSpeed);
            }
            else
                m_CameraKickbackRot = Vector3.Slerp(m_CameraKickbackRot, Vector3.zero, Time.deltaTime * 10);
        }
        
        public void StabiliseWeaponRecoil()
        {
            if (!m_WeaponKickbackAnimation.Enabled)
                return;
            
            m_WeaponKickbackPos = Vector3.Lerp(m_WeaponKickbackPos, m_WeaponKickbackTargetPos, Time.deltaTime * m_WeaponKickbackAnimation.KickbackSpeed);
            m_WeaponKickbackTargetPos = Vector3.Lerp(m_WeaponKickbackTargetPos, Vector3.zero, Time.deltaTime * m_WeaponKickbackAnimation.KickbackSpeed);
            m_WeaponKickbackRot = Vector3.Lerp(m_WeaponKickbackRot, m_WeaponKickbackTargetRot, Time.deltaTime * m_WeaponKickbackAnimation.KickbackSpeed);
            m_WeaponKickbackTargetRot = Vector3.Lerp(m_WeaponKickbackTargetRot, Vector3.zero, Time.deltaTime * m_WeaponKickbackAnimation.KickbackSpeed);
        }

        /// <summary>
        /// Method that mimics a shake animation, simulating the effect of an explosion near the character.
        /// </summary>
        public IEnumerator Shake(ExplosionShakeProperties prop)
        {
            // Original code written by Sebastian Lague.
            // https://github.com/SebLague/Camera-Shake

            float completionPercent = 0;
            float movePercent = 0;

            float radians = prop.Angle * Mathf.Deg2Rad - Mathf.PI;
            Vector3 previousWaypoint = Vector3.zero;
            Vector3 currentWaypoint = Vector3.zero;
            float moveDistance = 0;
            float speed = 0;

            Vector3 targetRotation = Vector3.zero;
            Vector3 previousRotation = Vector3.zero;

            do
            {
                if (movePercent >= 1 || Mathf.Abs(completionPercent) < Mathf.Epsilon)
                {
                    float dampingFactor = DampingCurve(completionPercent, prop.DampingPercent);
                    float noiseAngle = (Random.value - 0.5f) * Mathf.PI;
                    radians += Mathf.PI + noiseAngle * prop.NoisePercent;
                    currentWaypoint = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * (prop.Strength * dampingFactor);
                    previousWaypoint = m_ExplosionPos;
                    moveDistance = Vector3.Distance(currentWaypoint, previousWaypoint);

                    targetRotation = new Vector3(currentWaypoint.y, currentWaypoint.x).normalized * (prop.RotationPercent * dampingFactor * k_MaxAngle);
                    previousRotation = m_ExplosionRot;

                    speed = Mathf.Lerp(prop.MinSpeed, prop.MaxSpeed, dampingFactor);

                    movePercent = 0;
                }

                completionPercent += Time.deltaTime / prop.Duration;
                movePercent += Time.deltaTime / moveDistance * speed;
                m_ExplosionPos = Vector3.Lerp(previousWaypoint, currentWaypoint, movePercent);
                m_ExplosionRot = Vector3.Lerp(previousRotation, targetRotation, movePercent);

                yield return null;

            } while (moveDistance > 0);
        }

        private static float DampingCurve(float x, float dampingPercent)
        {
            float a = Mathf.Lerp(2, .25f, dampingPercent);
            float b = 1 - Mathf.Pow(Mathf.Clamp01(x), a);
            return Mathf.Pow(b, 3);
        }
    }
}

