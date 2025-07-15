//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using GameBuilders.FPSBuilder.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace FPSBuilder.EnvironmentPack.Editor
{
    [CustomEditor(typeof(InteractiveDoor))]
    public class InteractiveDoorEditor : UnityEditor.Editor
    {
        private SerializedProperty m_TargetTransform;

        private SerializedProperty m_OpenedPosition;
        private SerializedProperty m_OpenedRotation;

        private SerializedProperty m_ClosedPosition;
        private SerializedProperty m_ClosedRotation;

        private SerializedProperty m_Open;
        private SerializedProperty m_RequiresAnimation;
        private SerializedProperty m_Speed;

        private void OnEnable ()
        {
            m_TargetTransform = serializedObject.FindProperty("m_TargetTransform");
            m_OpenedPosition = serializedObject.FindProperty("m_OpenedPosition");
            m_OpenedRotation = serializedObject.FindProperty("m_OpenedRotation");
            m_ClosedPosition = serializedObject.FindProperty("m_ClosedPosition");
            m_ClosedRotation = serializedObject.FindProperty("m_ClosedRotation");
            m_Open = serializedObject.FindProperty("m_Open");
            m_RequiresAnimation = serializedObject.FindProperty("m_RequiresAnimation");
            m_Speed = serializedObject.FindProperty("m_Speed");
        }

        public override void OnInspectorGUI ()
        {
            //Update the serializedProperty - always do this in the beginning of OnInspectorGUI
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_TargetTransform);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_Speed);

            EditorGUILayout.Space();
            using (new EditorGUILayout.VerticalScope(Styling.background))
            {
                EditorGUILayout.PropertyField(m_OpenedPosition);
                EditorGUILayout.PropertyField(m_OpenedRotation);

                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Set Position & Rotation", Styling.leftButton))
                    {
                        m_OpenedPosition.vector3Value = ((Transform)m_TargetTransform.objectReferenceValue).localPosition;
                        m_OpenedRotation.vector3Value = ((Transform)m_TargetTransform.objectReferenceValue).localEulerAngles;
                    }

                    if (GUILayout.Button("Reset", Styling.rightButton))
                    {
                        m_OpenedPosition.vector3Value = Vector3.zero;
                        m_OpenedRotation.vector3Value = Vector3.zero;
                    }

                    GUILayout.FlexibleSpace();
                }
            }


            EditorGUILayout.Space();
            using (new EditorGUILayout.VerticalScope(Styling.background))
            {
                EditorGUILayout.PropertyField(m_ClosedPosition);
                EditorGUILayout.PropertyField(m_ClosedRotation);

                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Set Position & Rotation", Styling.leftButton))
                    {
                        m_ClosedPosition.vector3Value = ((Transform)m_TargetTransform.objectReferenceValue).localPosition;
                        m_ClosedRotation.vector3Value = ((Transform)m_TargetTransform.objectReferenceValue).localEulerAngles;
                    }

                    if (GUILayout.Button("Reset", Styling.rightButton))
                    {
                        m_ClosedPosition.vector3Value = Vector3.zero;
                        m_ClosedRotation.vector3Value = Vector3.zero;
                    }

                    GUILayout.FlexibleSpace();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_Open, new GUIContent("Start Open"));
            EditorGUILayout.PropertyField(m_RequiresAnimation);

            serializedObject.ApplyModifiedProperties();
        }
    }
}