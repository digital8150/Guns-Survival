using System;

using GameBuilders.FPSBuilder.Core.Weapons;

using UnityEngine;

#pragma warning disable CS0649

namespace GameBuilders.MinimalistUI.Scripts.Equipment
{
    [Serializable]
    public class GunIcon
    {
        [SerializeField]
        [Required]
        private GunData m_GunData;
        
        [SerializeField]
        [Required]
        private Sprite m_Icon;

        public GunData GunData => m_GunData;
        public Sprite Icon => m_Icon;
    }
}