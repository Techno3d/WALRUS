Shader "Unlit/LaserBeamShader"
{
    Properties
    {
        _MainTex ("Texture1", 2D) = "white" {}
        _Tex ("Texture2", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            // Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Tex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv-_Time*0.4)-0.1;
                col.g = step(1.-col.g, 0.86);
                fixed4 col2 = tex2D(_Tex, i.uv-_Time*0.6)-0.1;
                col2.g = step(1.-col.g, 0.86);
                fixed4 finalCol = col+col2;
                finalCol = lerp(fixed4(1., 1., 1., 1.), finalCol, step(0.2, finalCol.g));
                finalCol.a = smoothstep(0, 0.2, finalCol.g)-0.5;
                return finalCol;
            }
            ENDCG
        }
    }
}
