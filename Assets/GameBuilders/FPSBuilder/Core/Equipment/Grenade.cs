//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using System.Collections;
using GameBuilders.FPSBuilder.Core.Managers;
using GameBuilders.FPSBuilder.Core.Weapons.Extensions;
using UnityEngine;

#pragma warning disable CS0649

namespace GameBuilders.FPSBuilder.Core.Equipment
{
    [AddComponentMenu("FPS Builder/Items/Grenade"), DisallowMultipleComponent]
    public sealed class Grenade : Equipment
    {
        /// <summary>
        /// The grenade explosive prefab.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("The grenade explosive prefab.")]
        private Rigidbody m_Grenade;

        /// <summary>
        /// The Transform reference used to know where the grenade will be instantiated from.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("The Transform reference used to know where the grenade will be instantiated from.")]
        private Transform m_ThrowTransformReference;

        /// <summary>
        /// Defines the force that the character will throw the grenade.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines the force that the character will throw the grenade.")]
        private float m_ThrowForce = 20;
        
        /// <summary>
        /// Defines the amount of grenades the character will start.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines the amount of grenades the character will start.")]
        private int m_Amount = 3;
        
        /// <summary>
        /// Allow the character to use unlimited grenades.
        /// </summary>
        [SerializeField]
        [Tooltip("Allow the character to use unlimited grenades.")]
        private bool m_InfiniteGrenades;

        /// <summary>
        /// Defines the maximum number of grenades the character can carry.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines the maximum number of grenades the character can carry.")]
        private int m_MaxAmount = 3;

        /// <summary>
        /// Defines the delay in seconds for the character throw the grenade.
        /// (For some grenades it is necessary to remove the protection pin before throwing, use this field to adjust the necessary time for such action.)
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("Defines the delay in seconds for the character throw the grenade. " +
                 "(For some grenades it is necessary to remove the protection pin before throwing, use this field to adjust the necessary time for such action.)")]
        private float m_DelayToInstantiate = 0.14f;

        /// <summary>
        /// The animator reference.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("The animator reference.")]
        private Animator m_Animator;

        /// <summary>
        /// Animation of pulling the grenade pin.
        /// </summary>
        [SerializeField]
        [Tooltip("Animation of pulling the grenade pin.")]
        private string m_PullAnimation;

        /// <summary>
        /// Sound of pulling the grenade pin.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound of pulling the grenade pin.")]
        private AudioClip m_PullSound;

        /// <summary>
        /// Defines the volume of the grenade pin pulling sound.
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Defines the volume of the grenade pin pulling sound.")]
        private float m_PullVolume = 0.2f;

        /// <summary>
        /// Animation of throwing the grenade.
        /// </summary>
        [SerializeField]
        [Tooltip("Animation of throwing the grenade.")]
        private string m_ThrowAnimation;

        /// <summary>
        /// The sound of throwing the grenade.
        /// </summary>
        [SerializeField]
        [Tooltip("The sound of throwing the grenade.")]
        private AudioClip m_ThrowSound;

        /// <summary>
        /// The volume of the grenade throwing sound.
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The volume of the grenade throwing sound.")]
        private float m_ThrowVolume = 0.2f;

        private bool m_Initialized;
        
        private WaitForSeconds m_PullDuration;
        private WaitForSeconds m_InstantiateDelay;
        private AudioEmitter m_PlayerBodySource;

        #region PROPERTIES

        /// <summary>
        /// The duration in seconds of the animations of pulling a grenade pin.
        /// </summary>
        public override float UsageDuration
        {
            get
            {
                if (!m_Animator)
                    return 0;

                float duration = 0;

                if (m_PullAnimation.Length > 0)
                {
                    duration += m_Animator.GetAnimationClip(m_PullAnimation).length;
                }

                if (m_ThrowAnimation.Length > 0)
                {
                    duration += Mathf.Max(m_Animator.GetAnimationClip(m_ThrowAnimation).length, m_DelayToInstantiate);
                }

                return duration;
            }
        }
        
        /// <summary>
        /// The current amount of grenades the character has.
        /// </summary>
        public int Amount => m_InfiniteGrenades ? 99 : m_Amount;
        
        /// <summary>
        /// Can the character carry more grenades?
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
            
            m_PullDuration = m_PullAnimation.Length > 0 ? new WaitForSeconds(m_Animator.GetAnimationClip(m_PullAnimation).length) : new WaitForSeconds(0);
            m_InstantiateDelay = new WaitForSeconds(m_DelayToInstantiate);
            
            m_PlayerBodySource = AudioManager.Instance.RegisterSource("[AudioEmitter] CharacterBody", transform.root, spatialBlend: 0);

            DisableShadowCasting();
        }

        /// <summary>
        /// Uses a unit of the item and instantiates a grenade.
        /// </summary>
        public override void Use()
        {
            if (m_Grenade && m_ThrowTransformReference && m_Animator)
                StartCoroutine(ThrowGrenade());
        }

        /// <summary>
        /// Play the animation and instantiates a grenade.
        /// </summary>
        private IEnumerator ThrowGrenade()
        {
            if (m_PullAnimation.Length > 0)
            {
                m_Animator.CrossFadeInFixedTime(m_PullAnimation, 0.1f);
                m_PlayerBodySource.ForcePlay(m_PullSound, m_PullVolume);
                yield return m_PullDuration;
            }

            if (m_ThrowAnimation.Length > 0)
            {
                m_Animator.CrossFadeInFixedTime(m_ThrowAnimation, 0.1f);
                m_PlayerBodySource.ForcePlay(m_ThrowSound, m_ThrowVolume);
                yield return m_InstantiateDelay;
            }
            
            InstantiateGrenade();
        }

        /// <summary>
        /// Throw a grenade using the parameters.
        /// </summary>
        private void InstantiateGrenade()
        {
            if (!m_Grenade)
                return;
            
            if (!m_InfiniteGrenades)
                m_Amount--;

            Rigidbody clone = Instantiate(m_Grenade, m_ThrowTransformReference.position, m_ThrowTransformReference.rotation);
            clone.linearVelocity = clone.transform.TransformDirection(Vector3.forward) * m_ThrowForce;
        }
        
        /// <summary>
        /// Refill the grenades.
        /// </summary>
        public override void Refill()
        {
            m_Amount = m_MaxAmount;
        }
    }
}
