//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using GameBuilders.FPSBuilder.Interfaces;
using UnityEngine;

#pragma warning disable CS0649

namespace GameBuilders.FPSBuilder.Core.Weapons
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("FPS Builder/Weapon/Gun Pickup"), DisallowMultipleComponent]
    public class GunPickup : MonoBehaviour, IPickup
    {
        /// <summary>
        /// The Gun Data Asset that this gun pickup represents.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("The Gun Data Asset that this gun pickup represents.")]
        private GunData m_GunData;
        
        [SerializeField]
        private int m_CurrentRounds;

        /// <summary>
        /// Returns the ID represented by this gun pickup.
        /// </summary>
        public int Identifier => m_GunData != null ? m_GunData.GetInstanceID() : -1;
        
        /// <summary>
        /// 
        /// </summary>
        public int CurrentRounds
        {
            get => m_CurrentRounds;
            set => m_CurrentRounds = Mathf.Max(value, 0);
        }
    }
}

