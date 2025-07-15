Shader "FPS Builder/Transparent Lit" 
{
    Properties
    {
        _BaseColor("Color", Color) = (1,1,1,1)
        _BaseColorMap("BaseColorMap", 2D) = "white" {}
        
        _Metallic ("Metallic", Range(0.0, 1.0)) = 1
        _Glossiness ("Smoothness", Range(0.0, 1.0)) = 1
        
        [NoScaleOffset] _MaskMap("MaskMap", 2D) = "white" {}
        [NoScaleOffset] _NormalMap("NormalMap", 2D) = "bump" {}
        _NormalScale("NormalScale", Range(0.0, 2.0)) = 1
        
        [HDR] _EmissiveColor("EmissiveColor", Color) = (0, 0, 0)
		[NoScaleOffset] _EmissiveColorMap("EmissiveColorMap", 2D) = "white" {}
        
        [Toggle(VIEWMODEL)] _Viewmodel ("Viewmodel", Float) = 0
		_ViewmodelFOV("Field of View", Range(1.0, 179.0)) = 60
    }
    
    CGINCLUDE
    
    #pragma multi_compile __ _VIEWMODEL
    
    ENDCG
    
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 400
 
        CGPROGRAM
        
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma target 3.0
        #pragma surface surf Standard vertex:vert alpha:fade addshadow

        fixed4  _BaseColor;
        fixed	_ViewmodelFOV;
        fixed   _NormalScale;
        fixed4	_EmissiveColor;
        half	_Metallic;
        half	_Glossiness;
        
        sampler2D _BaseColorMap;
        sampler2D _MaskMap;
        sampler2D _NormalMap;
        sampler2D _EmissiveColorMap;
 
        struct Input
        {
            float2 uv_BaseColorMap;
        };

        void vert (inout appdata_full v) 
		{
			define_fov(_ViewmodelFOV);
      	}
     
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_BaseColorMap, IN.uv_BaseColorMap) * _BaseColor;
            fixed4 mask = tex2D(_MaskMap,IN.uv_BaseColorMap);
            fixed4 emission = tex2D(_EmissiveColorMap, IN.uv_BaseColorMap) * _EmissiveColor;
     
            o.Albedo = c.rgb;
            o.Metallic = mask.r * _Metallic;
            o.Smoothness = mask.a * _Glossiness;
            o.Occlusion = mask.g;
            o.Emission = emission;
            o.Normal = normalize(UnpackScaleNormal(tex2D(_NormalMap, IN.uv_BaseColorMap), _NormalScale));
            o.Alpha = c.a;
        }
        
        ENDCG
    }
    FallBack "Diffuse"
    CustomEditor "LitShaderGUI"
}