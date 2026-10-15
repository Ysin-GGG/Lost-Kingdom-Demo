Shader "Custom/PixOutLine2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowSkew ("Shadow Skew", Vector) = (0.1, 0, 0, 0)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "PreviewType"="Plane"
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
    }
}