//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using System;
using UnityEditor;
using UnityEngine;

public class TelescopicSightShaderGUI : ShaderGUI
{
    private static readonly GUIContent s_StaticLabel = new GUIContent();
    private MaterialEditor m_Editor;
    private MaterialProperty[] m_Properties;

    private MaterialProperty FindProperty (string name, bool propertyIsMandatory = true)
    {
        return FindProperty(name, m_Properties, propertyIsMandatory);
    }

    private static GUIContent MakeLabel (string text, string tooltip = null)
    {
        s_StaticLabel.text = text;
        s_StaticLabel.tooltip = tooltip;
        return s_StaticLabel;
    }

    private static GUIContent MakeLabel (MaterialProperty property, string tooltip = null)
    {
        s_StaticLabel.text = property.displayName;
        s_StaticLabel.tooltip = tooltip;
        return s_StaticLabel;
    }

    private void ReticleProperties ()
    {
        MaterialProperty reticleMap = FindProperty("_ReticleMap");
        m_Editor.TexturePropertySingleLine(MakeLabel(reticleMap), reticleMap, FindProperty("_ReticleColor"), FindProperty("_ReticleOpacity"));
        m_Editor.ShaderProperty(FindProperty("_Aperture"), MakeLabel("Aperture"));
    }

    private void ShaderProperties ()
    {
        MaterialProperty mainTex = FindProperty("_MainTex");
        m_Editor.TexturePropertySingleLine(MakeLabel("Base Map", "Specify the base color(RGB) and opacity(A)."), mainTex, FindProperty("_Color"));
        ReticleProperties();

        m_Editor.ShaderProperty(FindProperty("_ViewmodelFOV"), MakeLabel("Field of View"));
    }

    public override void OnGUI (MaterialEditor _editor, MaterialProperty[] _properties)
    {
        m_Editor = _editor;
        m_Properties = _properties;
        ShaderProperties();
    }
}
