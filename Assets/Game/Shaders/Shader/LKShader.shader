Shader "Custom/LKStanderShader"
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

            v2f vert (appdata v)
            {
                v2f o;
                float2 offset = float2(
                    v.vertex.y * _ShadowSkew.x,  
                    0                            
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
                col *= _ShadowColor;
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
