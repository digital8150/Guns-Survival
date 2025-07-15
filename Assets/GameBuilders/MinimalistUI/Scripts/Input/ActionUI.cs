using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable CS0649

namespace GameBuilders.MinimalistUI.Scripts.Input
{
    public class ActionUI : MonoBehaviour
    {
        /// <summary>
        /// Reference to action that is to be rebound from the UI.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to action that is to be rebound from the UI.")]
        private InputActionReference m_Action;
    
        /// <summary>
        /// Text label that will receive the name of the action.
        /// </summary>
        [SerializeField]
        [Tooltip("Text label that will receive the name of the action.")]
        private TextMeshProUGUI m_ActionLabel;
    
        [SerializeField]
        private RectTransform m_ButtonBackground;
    
        [SerializeField]
        private bool m_AdjustPosition = true;
    
        /// <summary>
        /// Text label that will receive the current, formatted binding string.
        /// </summary>
        [SerializeField]
        [Tooltip("Text label that will receive the current, formatted binding string.")]
        private TextMeshProUGUI m_BindingText;

        private void OnEnable()
        {
            if (!m_Action || !m_ButtonBackground) 
                return;
        
            RectTransform actionRect = null;
            if (m_ActionLabel)
            {
                m_ActionLabel.text = m_Action.action.name;
                actionRect = m_ActionLabel.GetComponent<RectTransform>();
            }
        
            if (m_BindingText)
                m_BindingText.text = m_Action.action.bindings[0].ToDisplayString();
        
            Rect buttonRect = m_ButtonBackground.rect;

            if (m_AdjustPosition)
            {
                // Make space for the key name
                if (m_BindingText.text.Length > 2)
                {
                    m_ButtonBackground.sizeDelta = new Vector2(48, buttonRect.height);
                    m_ButtonBackground.localPosition = new Vector3(-64, 0, 0);
            
                    if (actionRect != null)
                        actionRect.localPosition = new Vector3(48, 0, 0);
                }
                else
                {
                    m_ButtonBackground.sizeDelta = new Vector2(24, buttonRect.height);
                    m_ButtonBackground.localPosition = new Vector3(-80, 0, 0);
            
                    if (actionRect != null)
                        actionRect.localPosition = new Vector3(16, 0, 0);
                }
            }
            else
            {
                m_ButtonBackground.sizeDelta = new Vector2(24, buttonRect.height);
                m_ButtonBackground.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}
