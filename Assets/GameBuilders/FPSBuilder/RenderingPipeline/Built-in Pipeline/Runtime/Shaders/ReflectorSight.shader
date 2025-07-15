//=========== Copyright (c) GameBuilders, All rights reserved. ================//

Shader "FPS Builder/Sights/Reflector Sight" 
{
	Properties 
	{
		[HDR]_ReticleColor("Reticle Color", Color) = (1,1,1,1)
		_ReticleMap("Reticle", 2D) = "white" {}
		_ReticleScale ("Reticle Scale", Float) = 0.05

		[Toggle(VIEWMODEL)] _Viewmodel ("Viewmodel", Float) = 0
		_ViewmodelFOV("Field of View", Range(1.0, 179.0)) = 60
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite false
		LOD 300

		CGPROGRAM
		#pragma multi_compile __ _VIEWMODEL

		#pragma surface surf Standard fullforwardshadows vertex:vert alpha
		#pragma target 3.0

		sampler2D _ReticleMap;
		
		fixed4 	_ReticleColor;
		half 	_ReticleScale;
		fixed	_ViewmodelFOV;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		};

		void vert (inout appdata_full v) 
		{
			define_fov(_ViewmodelFOV);
      	}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float dst = dot(_WorldSpaceCameraPos - IN.worldPos, IN.worldNormal);
			float3 cp = _WorldSpaceCameraPos - (dst * IN.worldNormal);
			float2 uv_Reticle = (mul((float3x3)unity_WorldToObject, IN.worldPos).xy - mul((float3x3)unity_WorldToObject, cp)).xy * (1 / _ReticleScale);

			fixed4 reticleTex = tex2D(_ReticleMap, float2(0.5, 0.5) + uv_Reticle / dst);
			o.Emission = (reticleTex.a * _ReticleColor.rgb * _ReticleColor.a);
			o.Albedo = reticleTex.a * _ReticleColor.rgb;
			o.Alpha = reticleTex.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
	CustomEditor "ReflectorSightShaderGUI"
}
