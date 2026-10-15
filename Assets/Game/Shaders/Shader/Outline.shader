Shader "LK/Filter/Outline"
{
	Properties
	{
		[PerRendererData]_MainTex("MainTex", 2D) = "white" {}
		_Color0 ("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Pass
        {
            Name "White"
			Tags { "Queue"="Transparent" }
			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			
	
			#include "UnityCG.cginc"

			struct appdata
			{
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color0;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col.a ? _Color0 : col;
			}
			ENDCG
        }
	}
	CustomEditor "ASEMaterialInspector"
	Fallback "Hidden/InternalErrorShader"
}
