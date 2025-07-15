//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using GameBuilders.FPSBuilder.Interfaces;
using UnityEngine;

#pragma warning disable CS0649

namespace FPSBuilder.EnvironmentPack
{
    [AddComponentMenu("FPS Builder/Gameplay/Interactive Door"), DisallowMultipleComponent]
    public class InteractiveDoor : MonoBehaviour, IActionable
    {
        [SerializeField]
        private Transform m_TargetTransform;

        [SerializeField]
        private Vector3 m_OpenedPosition;

        [SerializeField]
        private Vector3 m_OpenedRotation;

        [SerializeField]
        private Vector3 m_ClosedPosition;

        [SerializeField]
        private Vector3 m_ClosedRotation;

        [SerializeField]
        private bool m_Open;
        
        [SerializeField]
        private bool m_RequiresAnimation = true;

        [SerializeField]
        [MinMax(0.1f, 100f)]
        private float m_Speed = 10;

        public bool RequiresAnimation => m_RequiresAnimation;

        public void Interact ()
        {
            m_Open = !m_Open;
        }

        public string Message ()
        {
            return m_Open ? "Close" : "Open";
        }

        private void Update ()
        {
            if (m_Open)
            {
                m_TargetTransform.localPosition = Vector3.Lerp(m_TargetTransform.localPosition, m_OpenedPosition, Time.deltaTime * m_Speed);
                m_TargetTransform.localRotation = Quaternion.Slerp(m_TargetTransform.localRotation, Quaternion.Euler(m_OpenedRotation), Time.deltaTime * m_Speed);
            }
            else
            {
                m_TargetTransform.localPosition = Vector3.Lerp(m_TargetTransform.localPosition, m_ClosedPosition, Time.deltaTime * m_Speed);
                m_TargetTransform.localRotation = Quaternion.Slerp(m_TargetTransform.localRotation, Quaternion.Euler(m_ClosedRotation), Time.deltaTime * m_Speed);
            }
        }
    }
}
