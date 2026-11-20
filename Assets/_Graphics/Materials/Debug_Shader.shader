Shader "Custom/Checkered" {
    Properties {
        _Color("Main Color", Color) = (1,1,1,1)
        _Color2("Secondary Color", Color) = (0,0,0,1)
        _Scale("Checker Scale", Float) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;
            fixed4 _Color2;
            float _Scale;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 UV;
                float3 n = abs(i.worldNormal);
                if (n.x > n.y && n.x > n.z)
                    UV = i.worldPos.yz;
                else if (n.z > n.x && n.z > n.y)
                    UV = i.worldPos.xy;
                else
                    UV = i.worldPos.xz;

                UV *= _Scale;
                float checker = fmod(abs(floor(UV.x) + floor(UV.y)), 2.0);
                float3 color = checker < 1.0 ? _Color.rgb : _Color2.rgb;
                return fixed4(color, _Color.a);
            }

            ENDCG
        }
    }
    Fallback "Unlit/Color"
}
