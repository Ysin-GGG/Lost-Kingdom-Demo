// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LK"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[PerRendererData]_MainTex("素材纹理", 2D) = "white" {}
		[PerRendererData]_OutlineColor("描边颜色", Color) = (0,0,0,0)
		[PerRendererData]_Color("_Color", Color) = (0,0,0,0)
		[HideInInspector][PerRendererData]_Outline("是否描边", Float) = 0
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
			float4 _ShadowColor;
			float2 _Tilling;
			float2 _Offset;
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
            Name "Combined"
            
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineWidth;
            float4 _ShadowColor;
            float4 _ShadowSkew;
            //float4 _GlobalColor;

            v2f vert (appdata v)
            {
                v2f o;
                
                // 应用切变矩阵
                float2 offset = float2(
                    v.vertex.y * _ShadowSkew.x,  // 水平切变
                    0                            // 垂直方向不变
                );
                v.vertex.xy += offset;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texelSize = _OutlineWidth / _ScreenParams.xy;
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 检查周围像素
                fixed4 outline = 0;
                outline += tex2D(_MainTex, i.uv + float2(texelSize.x, 0));
                outline += tex2D(_MainTex, i.uv + float2(-texelSize.x, 0));
                outline += tex2D(_MainTex, i.uv + float2(0, texelSize.y));
                outline += tex2D(_MainTex, i.uv + float2(0, -texelSize.y));
                
                // 计算描边
                float outlineAlpha = outline.a * (1 - col.a);
                col = lerp(col, _OutlineColor, outlineAlpha);
                
                // 应用阴影颜色
                col *= _ShadowColor;
                
                // 应用全局颜色
                //col.rgb *= _GlobalColor.rgb;
                col *= i.color;
                
                return col;
            }
            ENDCG
        }

		
		Pass
		{
			
			Name "Sprite Normal"
			Tags { "LightMode"="NormalsRendering" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 140008


			#pragma vertex vert
			#pragma fragment frag

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS SHADERPASS_SPRITENORMAL

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "LKFunction.hlsl"
			#define ASE_NEEDS_FRAG_COLOR


			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float4 _MainTex_TexelSize;
			float4 _OutlineColor;
			float4 _ShadowColor;
			float2 _Tilling;
			float2 _Offset;
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
				float3 normalWS : TEXCOORD2;
				float4 tangentWS : TEXCOORD3;
				float3 bitangentWS : TEXCOORD4;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
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

				float3 normalWS = TransformObjectToWorldNormal( v.normal );
				o.normalWS = -GetViewForwardDir();
				float4 tangentWS = float4( TransformObjectToWorldDir( v.tangent.xyz ), v.tangent.w );
				o.tangentWS = normalize( tangentWS );
				half crossSign = (tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
				o.bitangentWS = crossSign * cross( normalWS, tangentWS.xyz ) * tangentWS.w;
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float4 color82 = ( tex2D( _MainTex, IN.texCoord0.xy ) * ( IN.color * _Color ) );
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
				float3 Normal = float3( 0, 0, 1 );

				Color *= IN.color;

				return NormalsRenderingShared( Color, Normal, IN.tangentWS.xyz, IN.bitangentWS, IN.normalWS);
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "Sprite Forward"
			Tags { "LightMode"="UniversalForward" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 140008


			#pragma vertex vert
			#pragma fragment frag


			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS SHADERPASS_SPRITEFORWARD

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			#include "LKFunction.hlsl"
			#define ASE_NEEDS_FRAG_COLOR


			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float4 _MainTex_TexelSize;
			float4 _OutlineColor;
			float4 _ShadowColor;
			float2 _Tilling;
			float2 _Offset;
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
				float3 positionWS : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.vertex.xyz );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;
				o.positionWS = vertexInput.positionWS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float3 positionWS = IN.positionWS.xyz;

				float4 color82 = ( tex2D( _MainTex, IN.texCoord0.xy ) * ( IN.color * _Color ) );
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

				#if defined(DEBUG_DISPLAY)
					SurfaceData2D surfaceData;
					InitializeSurfaceData(Color.rgb, Color.a, surfaceData);
					InputData2D inputData;
					InitializeInputData(positionWS.xy, half2(IN.texCoord0.xy), inputData);
					half4 debugColor = 0;

					SETUP_DEBUG_DATA_2D(inputData, positionWS);

					if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
					{
						return debugColor;
					}
				#endif

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif

				Color *= IN.color;

				return Color;
			}

			ENDHLSL
		}
		
        Pass
        {
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }

            Cull Off

            HLSLPROGRAM

			#define ASE_SRP_VERSION 140008


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENESELECTIONPASS 1


            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "LKFunction.hlsl"


			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float4 _MainTex_TexelSize;
			float4 _OutlineColor;
			float4 _ShadowColor;
			float2 _Tilling;
			float2 _Offset;
			float _Outline;
			CBUFFER_END


            struct VertexInput
			{
				float3 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};


            int _ObjectId;
            int _PassValue;

			
			VertexOutput vert(VertexInput v )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
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

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
				float3 positionWS = TransformObjectToWorld(v.vertex);
				o.clipPos = TransformWorldToHClip(positionWS);

				return o;
			}

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				float4 color82 = ( tex2D( _MainTex, IN.ase_texcoord.xy ) * ( IN.ase_color * _Color ) );
				float2 appendResult62 = (float2(_MainTex_TexelSize.x , _MainTex_TexelSize.y));
				float2 temp_output_63_0 = ( appendResult62 * 2 );
				float2 appendResult48 = (float2((float)0 , (float)1));
				float4 upColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult48 ) + IN.ase_texcoord.xy ) );
				float2 appendResult57 = (float2((float)0 , (float)-1));
				float4 downColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult57 ) + IN.ase_texcoord.xy ) );
				float2 appendResult58 = (float2((float)-1 , (float)0));
				float4 leftColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult58 ) + IN.ase_texcoord.xy ) );
				float2 appendResult59 = (float2((float)1 , (float)0));
				float4 rightColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult59 ) + IN.ase_texcoord.xy ) );
				float4 _OutlineColor82 = _OutlineColor;
				int _Outline82 = (int)_Outline;
				float4 localOutlineSeek82 = OutlineSeek( color82 , upColor82 , downColor82 , leftColor82 , rightColor82 , _OutlineColor82 , _Outline82 );
				float4 break84 = localOutlineSeek82;
				float3 appendResult86 = (float3(break84.x , break84.y , break84.z));
				float4 appendResult92 = (float4(( appendResult86 * break84.w ) , break84.w));
				
				float4 Color = appendResult92;

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}

            ENDHLSL
        }

		
        Pass
        {
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }

			Cull Off

            HLSLPROGRAM

			#define ASE_SRP_VERSION 140008


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENEPICKINGPASS 1


            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        	#include "LKFunction.hlsl"


			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float4 _MainTex_TexelSize;
			float4 _OutlineColor;
			float4 _ShadowColor;
			float2 _Tilling;
			float2 _Offset;
			float _Outline;
			CBUFFER_END


            struct VertexInput
			{
				float3 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            float4 _SelectionID;

			
			VertexOutput vert(VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
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

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
				float3 positionWS = TransformObjectToWorld(v.vertex);
				o.clipPos = TransformWorldToHClip(positionWS);

				return o;
			}

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				float4 color82 = ( tex2D( _MainTex, IN.ase_texcoord.xy ) * ( IN.ase_color * _Color ) );
				float2 appendResult62 = (float2(_MainTex_TexelSize.x , _MainTex_TexelSize.y));
				float2 temp_output_63_0 = ( appendResult62 * 2 );
				float2 appendResult48 = (float2((float)0 , (float)1));
				float4 upColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult48 ) + IN.ase_texcoord.xy ) );
				float2 appendResult57 = (float2((float)0 , (float)-1));
				float4 downColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult57 ) + IN.ase_texcoord.xy ) );
				float2 appendResult58 = (float2((float)-1 , (float)0));
				float4 leftColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult58 ) + IN.ase_texcoord.xy ) );
				float2 appendResult59 = (float2((float)1 , (float)0));
				float4 rightColor82 = tex2D( _MainTex, ( ( temp_output_63_0 * appendResult59 ) + IN.ase_texcoord.xy ) );
				float4 _OutlineColor82 = _OutlineColor;
				int _Outline82 = (int)_Outline;
				float4 localOutlineSeek82 = OutlineSeek( color82 , upColor82 , downColor82 , leftColor82 , rightColor82 , _OutlineColor82 , _Outline82 );
				float4 break84 = localOutlineSeek82;
				float3 appendResult86 = (float3(break84.x , break84.y , break84.z));
				float4 appendResult92 = (float4(( appendResult86 * break84.w ) , break84.w));
				
				float4 Color = appendResult92;
				half4 outColor = _SelectionID;
				return outColor;
			}

            ENDHLSL
        }
		
	}
	CustomEditor "ASEMaterialInspector"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;94;-3370.898,1409.019;Inherit;False;2710.54;1857.044;Comment;42;86;56;39;61;59;92;85;34;36;84;41;43;42;44;72;82;74;73;83;37;35;71;70;69;68;66;67;65;48;57;58;55;54;53;52;51;50;49;64;63;46;62;Outline;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-2966.976,1567.762;Inherit;False;2;2;0;FLOAT2;0,0;False;1;INT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-2714.747,1782.45;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.IntNode;51;-3104.631,1945.764;Inherit;False;Constant;_Int3;Int 3;8;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;52;-3104.631,2024.764;Inherit;False;Constant;_Int4;Int 4;8;0;Create;True;0;0;0;False;0;False;-1;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;53;-3109.631,2141.765;Inherit;False;Constant;_Int5;Int 5;8;0;Create;True;0;0;0;False;0;False;-1;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;54;-3107.631,2223.766;Inherit;False;Constant;_Int6;Int 6;8;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;55;-3106.631,2319.866;Inherit;False;Constant;_Int7;Int 7;8;0;Create;True;0;0;0;False;0;False;1;0;False;0;1;INT;0
Node;AmplifyShaderEditor.DynamicAppendNode;58;-2966.331,2164.665;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;57;-2960.631,1972.764;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;48;-2966.592,1790.859;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-2717.747,1971.45;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-2719.554,2364.544;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-2721.747,2169.452;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-2494.702,1784.576;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;69;-2481.446,1964.782;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;70;-2492.781,2176.383;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-2497.82,2366.573;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-2041.223,2874.719;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;73;-2241.775,2848.954;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1899.891,2596.254;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CustomExpressionNode;82;-1503.209,2203.617;Inherit;False; ;4;File;7;True;color;FLOAT4;0,0,0,0;In;;Inherit;False;True;upColor;FLOAT4;0,0,0,0;In;;Inherit;False;True;downColor;FLOAT4;0,0,0,0;In;;Inherit;False;True;leftColor;FLOAT4;0,0,0,0;In;;Inherit;False;True;rightColor;FLOAT4;0,0,0,0;In;;Inherit;False;True;_OutlineColor;FLOAT4;0,0,0,0;In;;Inherit;False;True;_Outline;INT;0;In;;Inherit;False;OutlineSeek;False;False;0;cd06139be12adf34dad52c166bb315f2;False;7;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;72;-2209.709,2593.974;Inherit;True;Property;_color;color;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;34;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;44;-2210.084,2366.814;Inherit;True;Property;_rightColor;rightColor;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;34;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;42;-2211.985,2156.833;Inherit;True;Property;_leftColor;leftColor;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;34;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;43;-2206.731,1941.043;Inherit;True;Property;_downColor;downColor;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;34;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;41;-2214.731,1720.043;Inherit;True;Property;_upColor;upColor;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;34;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;84;-1250.356,2208.127;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-990.3554,2267.127;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;92;-834.4255,2275.034;Inherit;False;COLOR;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;59;-2961.631,2358.768;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;61;-2731.352,2515.671;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexelSizeNode;39;-3315.198,1534.328;Inherit;False;34;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;56;-3110.468,2412.83;Inherit;False;Constant;_Int8;Int 8;8;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.DynamicAppendNode;86;-1120.142,2208.343;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IntNode;50;-3104.785,1847.707;Inherit;False;Constant;_Int2;Int 2;8;0;Create;True;0;0;0;False;0;False;1;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;49;-3104.755,1768.377;Inherit;False;Constant;_Int1;Int 1;8;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;46;-3104.164,1668.288;Inherit;False;Constant;_Int0;Int 0;8;0;Create;True;0;0;0;False;0;False;2;0;False;0;1;INT;0
Node;AmplifyShaderEditor.DynamicAppendNode;62;-3103.976,1568.762;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;37;-2281.495,3014.912;Inherit;False;Property;_Color;_Color;2;1;[PerRendererData];Create;True;1;BasicOutLine;0;0;False;0;False;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;36;-1780.828,2916.323;Inherit;False;Property;_Outline;是否描边;3;2;[HideInInspector];[PerRendererData];Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;35;-1846.928,2742.088;Inherit;False;Property;_OutlineColor;描边颜色;1;1;[PerRendererData];Create;False;1;BasicOutLine;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;34;-2225.406,1516.673;Inherit;True;Property;_MainTex;素材纹理;0;1;[PerRendererData];Create;False;1;BasicOutLine;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;286;-1066.302,3593.313;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;285;-1298.393,3791.672;Inherit;False;Property;_ShadowColor;阴影颜色;5;0;Create;False;0;0;0;False;0;False;0,0,0,0;0,0,0,0.4588235;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;273;-1677.61,4085.151;Inherit;False;Constant;_Int9;Int 9;5;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.DynamicAppendNode;276;-1513.61,3881.156;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;271;-1933.611,4085.149;Inherit;False;Property;_ShadowSkew;_ShadowSkew;4;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;274;-1511.61,3983.156;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateVertexDataNode;295;-1904.494,3902.589;Inherit;False;0;0;vertex;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;275;-1367.61,3980.156;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;278;-1226.511,3985.957;Inherit;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;272;-1685.61,3982.156;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;283;-1383.901,3595.369;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;34;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;304;-1834.907,3586.218;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;306;-2082.907,3668.218;Inherit;False;Property;_Offset;Offset;7;0;Create;True;0;0;0;False;0;False;0,0;-0.19,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;305;-2085.907,3546.218;Inherit;False;Property;_Tilling;Tilling;6;0;Create;True;0;0;0;False;0;False;0,0;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;307;-1568.859,3578.824;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;308;-1769.859,3701.824;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;1.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;310;-594.3081,2259.211;Float;False;True;-1;2;ASEMaterialInspector;0;14;LK;b87d2639bc8bd974182b054facc5350c;True;Sprite Lit;0;0;Sprite Lit;6;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;3;Vertex Position;1;0;Debug Display;0;0;External Alpha;0;0;0;6;True;True;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;312;-594.3081,2259.211;Float;False;False;-1;2;ASEMaterialInspector;0;1;New Amplify Shader;b87d2639bc8bd974182b054facc5350c;True;Sprite Normal;0;2;Sprite Normal;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=NormalsRendering;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;313;-594.3081,2259.211;Float;False;False;-1;2;ASEMaterialInspector;0;1;New Amplify Shader;b87d2639bc8bd974182b054facc5350c;True;Sprite Forward;0;3;Sprite Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;314;-594.3081,2259.211;Float;False;False;-1;2;ASEMaterialInspector;0;1;New Amplify Shader;b87d2639bc8bd974182b054facc5350c;True;SceneSelectionPass;0;4;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;315;-594.3081,2259.211;Float;False;False;-1;2;ASEMaterialInspector;0;1;New Amplify Shader;b87d2639bc8bd974182b054facc5350c;True;ScenePickingPass;0;5;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;311;-888.7628,3630.46;Float;False;False;-1;2;ASEMaterialInspector;0;14;New Amplify Shader;b87d2639bc8bd974182b054facc5350c;True;Sprite Lit2;0;1;Sprite Lit2;6;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;5;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Lit;ShaderGraphShader=true;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D2;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;63;0;62;0
WireConnection;63;1;46;0
WireConnection;64;0;63;0
WireConnection;64;1;48;0
WireConnection;58;0;53;0
WireConnection;58;1;54;0
WireConnection;57;0;51;0
WireConnection;57;1;52;0
WireConnection;48;0;49;0
WireConnection;48;1;50;0
WireConnection;65;0;63;0
WireConnection;65;1;57;0
WireConnection;67;0;63;0
WireConnection;67;1;59;0
WireConnection;66;0;63;0
WireConnection;66;1;58;0
WireConnection;68;0;64;0
WireConnection;68;1;61;0
WireConnection;69;0;65;0
WireConnection;69;1;61;0
WireConnection;70;0;66;0
WireConnection;70;1;61;0
WireConnection;71;0;67;0
WireConnection;71;1;61;0
WireConnection;83;0;73;0
WireConnection;83;1;37;0
WireConnection;74;0;72;0
WireConnection;74;1;83;0
WireConnection;82;0;74;0
WireConnection;82;1;41;0
WireConnection;82;2;43;0
WireConnection;82;3;42;0
WireConnection;82;4;44;0
WireConnection;82;5;35;0
WireConnection;82;6;36;0
WireConnection;72;1;61;0
WireConnection;44;1;71;0
WireConnection;42;1;70;0
WireConnection;43;1;69;0
WireConnection;41;1;68;0
WireConnection;84;0;82;0
WireConnection;85;0;86;0
WireConnection;85;1;84;3
WireConnection;92;0;85;0
WireConnection;92;3;84;3
WireConnection;59;0;55;0
WireConnection;59;1;56;0
WireConnection;86;0;84;0
WireConnection;86;1;84;1
WireConnection;86;2;84;2
WireConnection;62;0;39;1
WireConnection;62;1;39;2
WireConnection;286;0;283;0
WireConnection;286;1;285;0
WireConnection;276;0;295;1
WireConnection;276;1;295;2
WireConnection;274;0;272;0
WireConnection;274;1;273;0
WireConnection;275;0;276;0
WireConnection;275;1;274;0
WireConnection;278;0;275;0
WireConnection;278;2;295;3
WireConnection;272;0;295;2
WireConnection;272;1;271;0
WireConnection;283;1;307;0
WireConnection;304;0;305;0
WireConnection;304;1;306;0
WireConnection;307;0;304;0
WireConnection;307;1;308;0
WireConnection;310;1;92;0
WireConnection;311;1;286;0
ASEEND*/
//CHKSM=9C88AFA5B1B1AC1AFF427A281648B5E37F49A40C