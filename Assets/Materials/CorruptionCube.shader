Shader "Unlit/CorruptionCube"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(
                    _MainTex, i.uv+
                        sin(_Time/10)
                );
                float value = (col.r + col.b + col.g)/3.0;
                col.rgb = (1-smoothstep(0.05, 0.12, value))/10.;
                value = (col.r + col.b + col.g)/3.0;
                float stupid = sin(_Time)*2.0;
                float stupid2 = cos(_Time)*2.0;
                float2 aberate = float2(2.0*i.uv.x+stupid, 2.0*i.uv.y+stupid2);
                col = lerp(fixed4(0.01, 0.01, 0.01, 1), fixed4(1+aberate.x, 1, 1+aberate.y, 1), value);
                // col = fixed4(0.01, 0.01, 0.01, 1);


                return col;
            }
            ENDCG
        }
    }
}
