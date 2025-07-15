using UnityEngine;

#pragma warning disable CS0649

namespace GameBuilders.FPSBuilder.Core.Equipment
{
    public abstract class Equipment : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [Tooltip("")]
        protected EquipmentData m_EquipmentData;

        public abstract void Init();
    
        public abstract void Use();
    
        #region PROPERTIES
    
        public virtual int Identifier => m_EquipmentData.GetInstanceID();

        public abstract float UsageDuration
        {
            get;
        }
    
        #endregion

        /// <summary>
        /// Deactivates the shadows created by the equipment.
        /// </summary>
        public virtual void DisableShadowCasting()
        {
            // For each object that has a renderer inside the equipment gameObject.
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0, l = renderers.Length; i < l; i++)
            {
                renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderers[i].sharedMaterial.EnableKeyword("_VIEWMODEL");
            }
        }

        public abstract void Refill();
    }
}
