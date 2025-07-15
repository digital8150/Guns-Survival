//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using UnityEditor;

namespace FPSBuilder.EnvironmentPack.Editor
{
    [CustomEditor(typeof(BreakableLight))]
    public class BreakableLightEditor : UnityEditor.Editor
    {
        private static readonly string[] m_ScriptField = { "m_Script" };

        public override void OnInspectorGUI ()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            DrawPropertiesExcluding(serializedObject, m_ScriptField);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
