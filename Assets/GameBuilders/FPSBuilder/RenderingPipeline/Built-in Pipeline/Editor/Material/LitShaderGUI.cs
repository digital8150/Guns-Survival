//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using GameBuilders.FPSBuilder.Editor.Utility;
using UnityEditor;
using UnityEngine;

public class LitShaderGUI : ShaderGUI
{
    private static readonly GUIContent StaticLabel = new GUIContent();
    private MaterialEditor m_Editor;
    private MaterialProperty[] m_Properties;

    private bool m_SurfaceInputs;
    private bool m_DetailInputs;
    private bool m_EmissionInputs;
    private bool m_AdvancedOptions;

    private MaterialProperty FindProperty (string name, bool propertyIsMandatory = true)
    {
        return FindProperty(name, m_Properties, propertyIsMandatory);
    }

    private static GUIContent MakeLabel (string text, string tooltip = null)
    {
        StaticLabel.text = text;
        StaticLabel.tooltip = tooltip;
        return StaticLabel;
    }

    private void NormalProperties ()
    {
        MaterialProperty normalMap = FindProperty("_NormalMap");
        Texture tex = normalMap.textureValue;
        
        m_Editor.TexturePropertySingleLine(MakeLabel("Normal Map", "Specifies the Normal Map for this Material (BC7/BC5/DXT5(nm)) and control its strength."),
            normalMap, tex ? FindProperty("_NormalScale") : null);
    }

    private void MaskProperties ()
    {
        MaterialProperty maskMap = FindProperty("_MaskMap");
        m_Editor.TexturePropertySingleLine(MakeLabel("Mask Map", "Specifies the Mask Map for this Material" +
                                                                 " - Metallic (R), Ambient occlusion (G), " +
                                                                 "Detail mask (B), Smoothness (A)."), maskMap);
        EditorGUI.indentLevel += 1;
        m_Editor.ShaderProperty(FindProperty("_Metallic"), MakeLabel("Metallic", "Controls the scale factor for the Material's Metallic."));
        m_Editor.ShaderProperty(FindProperty("_Glossiness"), MakeLabel("Smoothness", "Controls the scale factor for the Material's Smoothness."));
        EditorGUI.indentLevel -= 1;
    }

    private void EmissionProperties ()
    {
        MaterialProperty emission = FindProperty("_EmissiveColorMap");
        m_Editor.TexturePropertySingleLine(MakeLabel("Emission Map", "Specifies the Emission Map (RGB) for this Material."), emission, FindProperty("_EmissiveColor"));
    }

    private void ShaderProperties ()
    {
        EditorUtilities.FoldoutHeader("Surface Inputs", ref m_SurfaceInputs);

        if (m_SurfaceInputs)
        {
            EditorGUI.indentLevel += 1;
            MaterialProperty mainTex = FindProperty("_BaseColorMap");
            m_Editor.TexturePropertySingleLine(MakeLabel("Base Map", "Specify the base color(RGB) and opacity(A)."), mainTex, FindProperty("_BaseColor"));
            
            EditorGUI.indentLevel += 1;
            
            MaterialProperty cutoff = FindProperty("_Cutoff", false);
            if (cutoff != null)
                m_Editor.ShaderProperty(cutoff, MakeLabel("Alpha cutoff"));
            
            EditorGUI.indentLevel -= 1;
            
            MaskProperties();
            NormalProperties();

            m_Editor.TextureScaleOffsetProperty(mainTex);
            EditorGUI.indentLevel -= 1;
        }

        EditorUtilities.FoldoutHeader("Emission Inputs", ref m_EmissionInputs);

        if (m_EmissionInputs)
        {
            EditorGUI.indentLevel += 1;
            EmissionProperties();
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
