//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using GameBuilders.FPSBuilder.Editor.Utility;
using UnityEditor;

namespace GameBuilders.FPSBuilder.Core.Animation.Editor
{
    [CustomEditor(typeof(MotionData))]
    public sealed class MotionDataEditor : UnityEditor.Editor
    {
        private SerializedProperty m_AnimatePosition;
        private SerializedProperty m_PositionSpeed;
        private SerializedProperty m_HorizontalPositionAmplitude;
        private SerializedProperty m_VerticalPositionAmplitude;
        private SerializedProperty m_VerticalPositionAnimationCurve;
        private SerializedProperty m_DistalAmplitude;
        
        private SerializedProperty m_AnimateRotation;
        private SerializedProperty m_RotationSpeed;
        private SerializedProperty m_HorizontalRotationAmplitude;
        private SerializedProperty m_VerticalRotationAmplitude;
        private SerializedProperty m_VerticalRotationAnimationCurve;
        private SerializedProperty m_TiltAmplitude;
        
        private SerializedProperty m_VelocityInfluence;
        private SerializedProperty m_PositionOffset;
        private SerializedProperty m_RotationOffset;

        private void OnEnable()
        {
            m_AnimatePosition = serializedObject.FindProperty("m_AnimatePosition");
            m_PositionSpeed = serializedObject.FindProperty("m_PositionSpeed");
            m_HorizontalPositionAmplitude = serializedObject.FindProperty("m_HorizontalPositionAmplitude");
            m_VerticalPositionAmplitude = serializedObject.FindProperty("m_VerticalPositionAmplitude");
            m_VerticalPositionAnimationCurve = serializedObject.FindProperty("m_VerticalPositionAnimationCurve");
            m_DistalAmplitude = serializedObject.FindProperty("m_DistalAmplitude");
            
            m_AnimateRotation = serializedObject.FindProperty("m_AnimateRotation");
            m_RotationSpeed = serializedObject.FindProperty("m_RotationSpeed");
            m_HorizontalRotationAmplitude = serializedObject.FindProperty("m_HorizontalRotationAmplitude");
            m_VerticalRotationAmplitude = serializedObject.FindProperty("m_VerticalRotationAmplitude");
            m_VerticalRotationAnimationCurve = serializedObject.FindProperty("m_VerticalRotationAnimationCurve");
            m_TiltAmplitude = serializedObject.FindProperty("m_TiltAmplitude");
            
            m_VelocityInfluence = serializedObject.FindProperty("m_VelocityInfluence");
            m_PositionOffset = serializedObject.FindProperty("m_PositionOffset");
            m_RotationOffset = serializedObject.FindProperty("m_RotationOffset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.LabelField(EditorUtilities.GetRect(36), "Animation Properties", Styling.headerLabel);
            
            EditorUtilities.ToggleHeader("Animate Position", m_AnimatePosition);

            if (m_AnimatePosition.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                using (new EditorGUI.DisabledScope(!m_AnimatePosition.boolValue))
                {
                    EditorGUILayout.PropertyField(m_PositionSpeed);
                    EditorGUILayout.PropertyField(m_HorizontalPositionAmplitude);
                    EditorGUILayout.PropertyField(m_VerticalPositionAmplitude);
                    EditorGUILayout.PropertyField(m_VerticalPositionAnimationCurve);
                    EditorGUILayout.PropertyField(m_DistalAmplitude);
                }
            }
            EditorGUI.indentLevel = 0;

            EditorUtilities.ToggleHeader("Animate Rotation", m_AnimateRotation);

            if (m_AnimateRotation.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                using (new EditorGUI.DisabledScope(!m_AnimateRotation.boolValue))
                {
                    EditorGUILayout.PropertyField(m_RotationSpeed);
                    EditorGUILayout.PropertyField(m_HorizontalRotationAmplitude);
                    EditorGUILayout.PropertyField(m_VerticalRotationAmplitude);
                    EditorGUILayout.PropertyField(m_VerticalRotationAnimationCurve);
                    EditorGUILayout.PropertyField(m_TiltAmplitude);
                }
            }
            EditorGUI.indentLevel = 0;
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(m_VelocityInfluence);

            using (new EditorGUILayout.VerticalScope(Styling.background))
            {
                EditorGUILayout.LabelField("Animation Offset", EditorStyles.boldLabel);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_PositionOffset);
                EditorGUILayout.PropertyField(m_RotationOffset);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}