Shader "Custom/LiquidSplit"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1,0,0,1)   // 빨강
        _BottomColor ("Bottom Color", Color) = (0,0,1,1) // 파랑
        _SplitRatio ("Split Ratio", Range(0,1)) = 0.7
        _Level ("Level", Range(0,1)) = 0.5
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _TopColor;
            float4 _BottomColor;
            float _SplitRatio;
            float _Level;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                // 현재 픽셀이 액체 높이보다 아래인지 체크
                float relativeY = saturate((_Level - 0.0) / 1.0); // 0~1
                float splitPoint = _SplitRatio * _Level; // 70% 지점

                if (relativeY > splitPoint) {
                    return _TopColor;   // 빨강
                } else {
                    return _BottomColor; // 파랑
                }
            }
            ENDCG
        }
    }
}
