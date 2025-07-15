using System;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace GameBuilders.MinimalistUI.Scripts.Equipment
{
    [Serializable]
    public class SlotUI
    {
        [SerializeField] 
        private GameObject m_SlotObject;
        
        [SerializeField]
        private Image m_ItemIcon;
        
        [SerializeField]
        private TMP_Text m_Amount;
        
        [SerializeField]
        private GameObject m_ActiveStatus;
    
        public GameObject SlotObject => m_SlotObject;
        
        public Image ItemIcon => m_ItemIcon;
        public TMP_Text Amount => m_Amount;

        public void SetActiveWeapon(bool active)
        {
            m_ActiveStatus.SetActive(active);
        }
    }
}