Shader "Custom/PixOutLine"
{
    Properties
    {
		[Header(Basic Settings)]
		[PerRendererData] _OutlineColor("OutlineColor", Color) = (1,1,1,1) // �����ɫ
		[PerRendererData] _MainTex("Sprite", 2D) = "white" {}
		[PerRendererData] _Outline("Outline", Float) = 0 // ����0��ʾ���
		_Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
        
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		

		CBUFFER_START(UnityPerMaterial)
		float4 _MainTex_ST;
		float4 _MainTex_TexelSize;                                                                              
		half4 _OutlineColor;
		half4 _Color;
		float _Outline;
		CBUFFER_END

		TEXTURE2D(_MainTex);
		SAMPLER(sampler_MainTex);
		
		struct VertexInput {
			float4 positionOS	: POSITION;
			float4 color		: COLOR;
			float2 uv			: TEXCOORD0;
		};
		struct VertexOutput {
			float4 positionCS   : SV_POSITION;
			float4 color		: COLOR;
			float2 uv			: TEXCOORD0;
		};

		VertexOutput vert(VertexInput i) {
			VertexOutput o;
			VertexPositionInputs positionInputs = GetVertexPositionInputs(i.positionOS.xyz);
			o.positionCS = positionInputs.positionCS;
			o.uv = i.uv;
			o.color = i.color*_Color;
			return o;
		}
		
		half4 frag(VertexOutput i) :SV_Target{
			float4 upColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + _MainTex_TexelSize.xy*2*half2(0,1));
			float4 downColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + _MainTex_TexelSize.xy*2*half2(0, -1));
			float4 leftColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + _MainTex_TexelSize.xy*2*half2(-1, 0));
			float4 rightColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + _MainTex_TexelSize.xy*2*half2(1, 0));

			float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv)*i.color;

			if (_Outline > 0 && color.a == 0) {
				if (upColor.a != 0 || downColor.a != 0 || leftColor.a != 0 || rightColor.a != 0) {
					color = _OutlineColor;
				}
			}
			color.rgb *= color.a;
			//color*= _MainLightColor;
			return color;
		}
		ENDHLSL

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
    }
    FallBack "Diffuse"
}
