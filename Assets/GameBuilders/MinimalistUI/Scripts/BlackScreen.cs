//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace GameBuilders.MinimalistUI.Scripts
{
    public class BlackScreen : MonoBehaviour
    {
        [SerializeField] 
        private bool m_ShowAtStart;

        [SerializeField] 
        private float m_StartDelay;
        
        [SerializeField] 
        private float m_FadeSpeed = 1f;

        [SerializeField] 
        private Image m_BlackScreen;

        public bool Show
        {
            get; 
            set;
        }
        
        private void Start()
        {
            if (m_ShowAtStart)
            {
                Show = true;
                m_BlackScreen.color = new Color(0, 0, 0, 1);
                Invoke(nameof(FadeBlackScreen), m_StartDelay);
            }
            else
            {
                m_BlackScreen.color = new Color(0, 0, 0, 0);
            }
        }
        
        private void Update()
        {
            m_BlackScreen.color = new Color(0, 0, 0, 
                Mathf.MoveTowards(m_BlackScreen.color.a, Show ? 1 : 0, Time.deltaTime * m_FadeSpeed));
        }

        private void FadeBlackScreen()
        {
            Show = false;
        }
    }
}
