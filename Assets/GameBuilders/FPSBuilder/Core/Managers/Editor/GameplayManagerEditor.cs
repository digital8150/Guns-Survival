//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using UnityEditor;

namespace GameBuilders.FPSBuilder.Core.Managers.Editor
{
    [CustomEditor(typeof(GameplayManager))]
    public sealed class GameplayManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty m_GameplaySettings;
        private SerializedProperty m_InputBindings;

        private void OnEnable ()
        {
            m_GameplaySettings = serializedObject.FindProperty("m_GameplaySettings");
            m_InputBindings = serializedObject.FindProperty("m_InputBindings");
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_GameplaySettings);
            EditorGUILayout.PropertyField(m_InputBindings);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
