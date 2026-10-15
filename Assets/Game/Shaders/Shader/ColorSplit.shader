Shader "LK/Filter/ColorSplit"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[PerRendererData]_MainTex("Àÿ≤ƒŒ∆¿Ì", 2D) = "white" {}
		[PerRendererData]_OutlineColor("√Ë±ﬂ—’…´", Color) = (0,0,0,0)
		[PerRendererData]_Color("_Color", Color) = (0,0,0,0)
		[HideInInspector][PerRendererData]_Outline(" «∑Ò√Ë±ﬂ", Float) = 0
		_Color0 ("Color", Color) = (0,0,0,0.5)
        _Offset ("Offset", Vector) = (1, 1,0,0)
		
		_ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowSkew ("Shadow Skew", Vector) = (0.1, 0, 0, 0)


		[HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Lit" "ShaderGraphShader"="true" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
		ENDHLSL

		Pass
		{
			
			Name "Sprite Lit"
			Tags { "LightMode"="Universal2D" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 140008


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_0
			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_1
			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_2
			#pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_3

			#define _SURFACE_TYPE_TRANSPARENT 1

			#define SHADERPASS SHADERPASS_SPRITELIT
			#define SHADERPASS_SPRITELIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

			#if USE_SHAPE_LIGHT_TYPE_0
			SHAPE_LIGHT(0)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_1
			SHAPE_LIGHT(1)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_2
			SHAPE_LIGHT(2)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_3
			SHAPE_LIGHT(3)
			#endif

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

			#include "LKFunction.hlsl"


			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float4 _MainTex_TexelSize;
			float4 _OutlineColor;
			float2 _Tilling;
			float _Outline;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float4 screenPosition : TEXCOORD2;
				float3 positionWS : TEXCOORD3;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D(_AlphaTex); SAMPLER(sampler_AlphaTex);
				float _EnableAlphaTexture;
			#endif

			
			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_color = v.color;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;
				o.screenPosition = vertexInput.positionNDC;
				o.positionWS = vertexInput.positionWS;
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				float3 positionWS = IN.positionWS.xyz;

				float4 color82 = ( tex2D( _MainTex, IN.texCoord0.xy ) * ( IN.ase_color * _Color ) );
				float2 appendResult62 = (float2(_MainTex_TexelSize.x , _MainTex_TexelSize.y));
				float2 temp_output_63_0 = ( appendResult62 * 2 );
				float2 appendResult48 = (float2((float)0 , (float)1));
				float4 upColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult48 ) + IN.texCoord0.xy ) );
				float2 appendResult57 = (float2((float)0 , (float)-1));
				float4 downColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult57 ) + IN.texCoord0.xy ) );
				float2 appendResult58 = (float2((float)-1 , (float)0));
				float4 leftColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult58 ) + IN.texCoord0.xy ) );
				float2 appendResult59 = (float2((float)1 , (float)0));
				float4 rightColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult59 ) + IN.texCoord0.xy ) );
				float4 _OutlineColor82 = _OutlineColor;
				int _Outline82 = (int)_Outline;
				float4 localOutlineSeek82 = OutlineSeek( color82 , upColor82 , downColor82 , leftColor82 , rightColor82 , _OutlineColor82 , _Outline82 );
				float4 break84 = localOutlineSeek82;
				float3 appendResult86 = (float3(break84.x , break84.y , break84.z));
				float4 appendResult92 = (float4(( appendResult86 * break84.w ) , break84.w));
				
				float4 Color = appendResult92;
				float4 Mask = float4(1,1,1,1);
				float3 Normal = float3( 0, 0, 1 );

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, IN.texCoord0.xy);
					Color.a = lerp ( Color.a, alpha.r, _EnableAlphaTexture);
				#endif

				Color *= IN.color;

				SurfaceData2D surfaceData;
				InitializeSurfaceData(Color.rgb, Color.a, Mask, surfaceData);
				InputData2D inputData;
				InitializeInputData(IN.texCoord0.xy, half2(IN.screenPosition.xy / IN.screenPosition.w), inputData);
				SETUP_DEBUG_DATA_2D(inputData, positionWS);
				return CombinedShapeLightShared(surfaceData, inputData);
			}

			ENDHLSL
		}

		Pass
        {
            Name "Right"
			Tags { "Queue"="Transparent" }
			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert_r
            #pragma fragment frag
            ENDCG
        }

		Pass
        {
            Name "Up"
			Tags { "Queue"="Transparent" }
			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert_u
            #pragma fragment frag
            ENDCG
        }

		Pass
        {
            Name "Left"
			Tags { "Queue"="Transparent" }
			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert_l
            #pragma fragment frag
            ENDCG
        }
		
		Pass
        {
            Name "Down"
			Tags { "Queue"="Transparent" }
			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert_d
            #pragma fragment frag
            ENDCG
        }
	}
	
	CGINCLUDE
    #include "UnityCG.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        float4 color : COLOR;
    };

    struct v2f
    {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
        float4 color : COLOR;
    };

    sampler2D _MainTex;
	float4 _MainTex_TexelSize;
    float4 _MainTex_ST;
    float4 _Offset;
    float4 _Color0;

    v2f vert_r (appdata v)
    {
        v.vertex.x += _MainTex_TexelSize.x * _Offset.x;

        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        o.color = v.color;
        return o;
    }

    v2f vert_u (appdata v)
    {
        v.vertex.y += _MainTex_TexelSize.y * _Offset.y;
        
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        o.color = v.color;
        return o;
    }

    v2f vert_l (appdata v)
    {
        v.vertex.x -= _MainTex_TexelSize.x * _Offset.z;
        
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        o.color = v.color;
        return o;
    }

    v2f vert_d (appdata v)
    {
        v.vertex.y -= _MainTex_TexelSize.y * _Offset.w;
        
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        o.color = v.color;
        return o;
    }

    fixed4 frag (v2f i) : SV_Target
    {
        fixed4 col = tex2D(_MainTex, i.uv);
        return col.a ? _Color0 : col;
    }

	ENDCG
	CustomEditor "ASEMaterialInspector"
	Fallback "Hidden/InternalErrorShader"
	
}
