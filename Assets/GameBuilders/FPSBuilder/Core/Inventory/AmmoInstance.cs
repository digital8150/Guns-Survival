using System;

using GameBuilders.FPSBuilder.Core.Weapons;

using UnityEngine;

#pragma warning disable CS0649

namespace GameBuilders.FPSBuilder.Core.Inventory
{
    [Serializable]
    public class AmmoInstance
    {
        [SerializeField]
        private AmmoType m_AmmoType;
        
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        private int m_Amount;
        
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        private int m_MaxAmount;
        
        public AmmoType Instance => m_AmmoType;

        public int Amount
        {
            get => m_Amount;
            set => m_Amount = Mathf.Clamp(value, 0, m_MaxAmount);
        }
        
        public int MaxAmount => m_MaxAmount;
        
        /// <summary>
        /// 
        /// </summary>
        public bool IsEmptySlot => m_AmmoType == null;
    }
}