using UnityEngine;

namespace GameBuilders.FPSBuilder.Core.Equipment
{
    /// <summary>
    /// EquipmentData Asset is a container responsible for defining individual equipment characteristics.
    /// </summary>
    [CreateAssetMenu(menuName = "Equipment Data", fileName = "Equipment Data", order = 201)]
    public class EquipmentData : ScriptableObject
    {
        /// <summary>
        /// The equipment name.
        /// </summary>
        [SerializeField]
        [Tooltip("The equipment name.")]
        private string m_EquipmentName = "Equipment";
    
        /// <summary>
        /// The Prefab dropped when the character remove the equipment from the inventory.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("The Prefab dropped when the character remove the equipment from the inventory.")]
        private GameObject m_DroppablePrefab;
    
        /// <summary>
        /// The equipment weight.
        /// </summary>
        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        [Tooltip("The equipment weight.")]
        private float m_Weight = 0.5f;
    
        #region PROPERTIES
    
        public string EquipmentName => m_EquipmentName;

        public GameObject DroppablePrefab
        {
            get => m_DroppablePrefab;
            set => m_DroppablePrefab = value;
        }

        public float Weight
        {
            get => m_Weight;
            set => m_Weight = value;
        }
    
        #endregion
    }
}
