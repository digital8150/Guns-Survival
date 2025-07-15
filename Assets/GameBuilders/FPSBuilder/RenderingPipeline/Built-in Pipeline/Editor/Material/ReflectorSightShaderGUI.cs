//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using GameBuilders.FPSBuilder.Editor.Utility;
using UnityEditor;
using UnityEngine;

public class ReflectorSightShaderGUI : ShaderGUI
{
    private static GUIContent s_staticLabel = new GUIContent();
    private MaterialEditor m_Editor;
    private MaterialProperty[] m_Properties;
    
    private bool m_ReticleInputs;
    private bool m_AdvancedOptions;

    private MaterialProperty FindProperty (string name, bool propertyIsMandatory = true)
    {
        return FindProperty(name, m_Properties, propertyIsMandatory);
    }

    private static GUIContent MakeLabel (string text, string tooltip = null)
    {
        s_staticLabel.text = text;
        s_staticLabel.tooltip = tooltip;
        return s_staticLabel;
    }

    private static GUIContent MakeLabel (MaterialProperty property, string tooltip = null)
    {
        s_staticLabel.text = property.displayName;
        s_staticLabel.tooltip = tooltip;
        return s_staticLabel;
    }

    private void ReticleProperties ()
    {
        MaterialProperty reticleMap = FindProperty("_ReticleMap");

        m_Editor.TexturePropertySingleLine(MakeLabel(reticleMap), reticleMap, FindProperty("_ReticleColor"));
        m_Editor.ShaderProperty(FindProperty("_ReticleScale"), MakeLabel("Reticle Scale"));
    }

    private void ShaderProperties ()
    {
        EditorUtilities.FoldoutHeader("Reticle Inputs", ref m_ReticleInputs);

        if (m_ReticleInputs)
        {
            EditorGUI.indentLevel += 1;
            ReticleProperties();
            EditorGUI.indentLevel -= 1;
        }

        EditorUtilities.FoldoutHeader("Advanced Options", ref m_AdvancedOptions);

        if (m_AdvancedOptions)
        {
            EditorGUI.indentLevel += 1;
            m_Editor.ShaderProperty(FindProperty("_ViewmodelFOV"), MakeLabel("Field of View"));
            EditorGUI.indentLevel -= 1;
        }
    }

    public override void OnGUI (MaterialEditor _editor, MaterialProperty[] _properties)
    {
        m_Editor = _editor;
        m_Properties = _properties;
        ShaderProperties();
    }
}
