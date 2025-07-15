//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using System.Collections;
using GameBuilders.FPSBuilder.Core.Managers;
using GameBuilders.FPSBuilder.Core.Player;
using GameBuilders.FPSBuilder.Core.Weapons.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable CS0649

namespace GameBuilders.FPSBuilder.Core.Equipment
{
    /// <summary>
    /// First aid kit is an item utilized to restore the character’s vitality and heal injuries.
    /// </summary>
    [AddComponentMenu("FPS Builder/Items/First aid kit"), DisallowMultipleComponent]
    public sealed class FirstAidKit : Equipment
    {
        /// <summary>
        /// The character HealthController reference.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("The character HealthController reference.")]
        private HealthController m_HealthController;

        /// <summary>
        /// Defines how much vitality will be restored per use.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines how much vitality will be restored per use.")]
        private float m_HealAmount = 100;

        /// <summary>
        /// Defines the delay in seconds to apply the healing effect.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines the delay in seconds to apply the healing effect.")]
        private float m_DelayToHeal = 1.3f;
        
        /// <summary>
        /// Allow the character to receive an additional stamina bonus after being healed.
        /// </summary>
        [SerializeField]
        [Tooltip("Allow the character to receive an additional stamina bonus after being healed.")]
        private bool m_StaminaBonus;
        
        /// <summary>
        /// Defines how long the effect will last.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines how long the effect will last.")]
        private float m_StaminaBonusDuration = 10;
        
        /// <summary>
        /// Should the character injuries be healed?
        /// </summary>
        [SerializeField]
        [Tooltip("Should the character injuries be healed?")]
        private bool m_HealInjuries;
        
        /// <summary>
        /// Defines the amount of syringes the character will start.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines the amount of syringes the character will start.")]
        private int m_Amount = 3;

        /// <summary>
        /// Allow the character to use unlimited shots.
        /// </summary>
        [SerializeField]
        [Tooltip("Allow the character to use unlimited shots.")]
        private bool m_InfiniteShots;

        /// <summary>
        /// Defines the maximum number of syringes the character can carry.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines the maximum number of syringes the character can carry.")]
        private int m_MaxAmount = 3;

        /// <summary>
        /// The animator reference.
        /// </summary>
        [SerializeField]
        [Tooltip("The animator reference.")]
        private Animator m_Animator;

        /// <summary>
        /// The name of the animation that will be played when the character is healing.
        /// </summary>
        [SerializeField]
        [Tooltip("The name of the animation that will be played when the character is healing.")]
        private string m_HealAnimation = "Heal";

        /// <summary>
        /// The audio that will be played when the character is using the first aid kit.
        /// </summary>
        [SerializeField]
        [Tooltip("The audio that will be played when the character is using the first aid kit.")]
        private AudioClip m_HealSound;

        /// <summary>
        /// Defines the sound volume when the character is using the first aid kit.
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Defines the sound volume when the character is using the first aid kit.")]
        private float m_HealVolume = 0.3f;

        private bool m_Initialized;

        private WaitForSeconds m_HealDuration;
        private AudioEmitter m_PlayerBodySource;

        #region PROPERTIES

        /// <summary>
        /// The duration in seconds of the animation of using the first aid kit.
        /// </summary>
        public override float UsageDuration
        {
            get
            {
                if (!m_Animator)
                    return 0;

                if (m_HealAnimation.Length == 0)
                    return 0;

                AnimationClip healAnimation = m_Animator.GetAnimationClip(m_HealAnimation);
                return healAnimation.length > m_DelayToHeal ? healAnimation.length : m_DelayToHeal;
            }
        }
        
        /// <summary>
        /// The current amount of syringes the character has.
        /// </summary>
        public int Amount => m_InfiniteShots ? 99 : m_Amount;
        
        /// <summary>
        /// Can the character carry more adrenaline?
        /// </summary>
        public bool CanRefill => m_Amount < m_MaxAmount;

        #endregion

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public override void Init()
        {
            if (m_Initialized) 
                return;
            
            AnimationClip healAnimation = m_Animator.GetAnimationClip(m_HealAnimation);
            if (healAnimation)
                m_HealDuration = new WaitForSeconds(healAnimation.length > m_DelayToHeal ? healAnimation.length - m_DelayToHeal : m_DelayToHeal);
            else
            {
                Debug.LogError("Heal animation not found!");
            }

            DisableShadowCasting();

            m_Initialized = true;
        }

        /// <summary>
        /// Uses a unit of the item and apply the effects to the character.
        /// </summary>
        public override void Use()
        {
            if (m_Animator)
                StartCoroutine(AdrenalineShot());
        }

        /// <summary>
        /// Play the animation, regenerate the character's vitality and apply a temporary speed bonus.
        /// </summary>
        private IEnumerator AdrenalineShot()
        {
            if (m_HealAnimation.Length > 0)
                m_Animator.CrossFadeInFixedTime(m_HealAnimation, 0.1f);

            if (m_PlayerBodySource == null)
                m_PlayerBodySource = AudioManager.Instance.RegisterSource("[AudioEmitter] CharacterBody", transform.root, spatialBlend: 0);

            m_PlayerBodySource.ForcePlay(m_HealSound, m_HealVolume);

            yield return new WaitForSeconds(m_DelayToHeal);
            m_HealthController.Heal(m_HealAmount, m_HealInjuries, m_StaminaBonus && m_StaminaBonusDuration > 0, m_StaminaBonusDuration);
            
            if (!m_InfiniteShots)
                m_Amount--;

            yield return m_HealDuration;
        }
        
        /// <summary>
        /// Refill the adrenaline's syringes.
        /// </summary>
        public override void Refill()
        {
            m_Amount = m_MaxAmount;
        }
    }
}
