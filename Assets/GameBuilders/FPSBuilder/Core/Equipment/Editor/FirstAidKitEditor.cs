//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using UnityEditor;

namespace GameBuilders.FPSBuilder.Core.Equipment.Editor
{
    [CustomEditor(typeof(FirstAidKit))]
    public sealed class FirstAidKitEditor : UnityEditor.Editor
    {
        private FirstAidKit m_Target;
        private SerializedProperty m_EquipmentData;
        private SerializedProperty m_HealthController;
        private SerializedProperty m_HealAmount;
        private SerializedProperty m_DelayToHeal;
        private SerializedProperty m_StaminaBonus;
        private SerializedProperty m_StaminaBonusDuration;
        private SerializedProperty m_HealInjuries;
        
        private SerializedProperty m_Amount;
        private SerializedProperty m_InfiniteShots;
        private SerializedProperty m_MaxAmount;
        
        private SerializedProperty m_Animator;
        private SerializedProperty m_HealAnimation;
        private SerializedProperty m_HealSound;
        private SerializedProperty m_HealVolume;

        private void OnEnable ()
        {
            m_Target = (target as FirstAidKit);
            
            if (m_Target != null) 
                m_Target.DisableShadowCasting();

            m_EquipmentData = serializedObject.FindProperty("m_EquipmentData");
            m_HealthController = serializedObject.FindProperty("m_HealthController");
            m_HealAmount = serializedObject.FindProperty("m_HealAmount");
            m_DelayToHeal = serializedObject.FindProperty("m_DelayToHeal");
            m_StaminaBonus = serializedObject.FindProperty("m_StaminaBonus");
            m_StaminaBonusDuration = serializedObject.FindProperty("m_StaminaBonusDuration");
            m_HealInjuries = serializedObject.FindProperty("m_HealInjuries");
            m_Animator = serializedObject.FindProperty("m_Animator");
            m_HealAnimation = serializedObject.FindProperty("m_HealAnimation");
            m_HealSound = serializedObject.FindProperty("m_HealSound");
            m_HealVolume = serializedObject.FindProperty("m_HealVolume");
            
            m_Amount = serializedObject.FindProperty("m_Amount");
            m_InfiniteShots = serializedObject.FindProperty("m_InfiniteShots");
            m_MaxAmount = serializedObject.FindProperty("m_MaxAmount");
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(m_EquipmentData);
            
            EditorGUILayout.PropertyField(m_HealthController);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_HealAmount);
            EditorGUILayout.PropertyField(m_DelayToHeal);
            EditorGUILayout.PropertyField(m_StaminaBonus);
            
            if (m_StaminaBonus.boolValue)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(m_StaminaBonusDuration);
            }
            
            EditorGUI.indentLevel = 0;
            EditorGUILayout.PropertyField(m_HealInjuries);
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_InfiniteShots);

            EditorGUI.indentLevel = 0;
            if (!m_InfiniteShots.boolValue)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(m_Amount);
                EditorGUILayout.PropertyField(m_MaxAmount);
            }

            EditorGUI.indentLevel = 0;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Healing Animations", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_Animator);
            
            EditorGUILayout.PropertyField(m_HealAnimation);
            EditorGUILayout.PropertyField(m_HealSound);
            EditorGUILayout.PropertyField(m_HealVolume);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

