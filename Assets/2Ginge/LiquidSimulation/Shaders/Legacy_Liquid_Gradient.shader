Shader "2Ginge/Potion/Legacy_Liquid_Gradient"
{
    Properties
    {
        [HDR]_SEmissionColor("Surface Emission Color", Color) = (1, 1, 1, 1)
        [HDR]_GradientTopColor("Gradient Top Color", Color) = (1,0,0,1)
        [HDR]_GradientBottomColor("Gradient Bottom Color", Color) = (0,0,1,1)
        _TopLine("Top Line (Local Y)", Float) = 1
        _BottomLine("Bottom Line (Local Y)", Float) = 0
        _EmissionScalar("Emission Intensity", Float) = 1
    }
    SubShader
    {
        // ---------- Front Cull Pass ----------
        Pass
        {
            Tags{ "RenderType"="Opaque" "Queue"="AlphaTest+100" }
            LOD 200
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #pragma target 2.0

            float _EmissionScalar;
            float4 _SEmissionColor;
            float4 _GradientTopColor;
            float4 _GradientBottomColor;
            float _TopLine;
            float _BottomLine;
            float4 _anchor;
            float4 _point;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 localPos : TEXCOORD5;
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.localPos = v.vertex; // 로컬 좌표 그대로 저장
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            float remap(float value, float minSrc, float maxSrc)
            {
                return saturate((value - minSrc) / (maxSrc - minSrc));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (dot(normalize(i.localPos.xyz - _point), normalize(_point - _anchor)) > 0)
                    discard;

                // Gradient emission (local Y 기준)
                float v = remap(i.localPos.y, _BottomLine, _TopLine);
                fixed4 gradCol = lerp(_GradientBottomColor, _GradientTopColor, v);

                fixed4 col = gradCol * _EmissionScalar;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }

        // ---------- Back Cull Pass ----------
        Pass
        {
            Tags{ "RenderType"="Opaque" "Queue"="AlphaTest+100" }
            LOD 200
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #pragma target 2.0

            float _EmissionScalar;
            float4 _GradientTopColor;
            float4 _GradientBottomColor;
            float _TopLine;
            float _BottomLine;
            float4 _anchor;
            float4 _point;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 localPos : TEXCOORD5;
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.localPos = v.vertex; // 로컬 좌표
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            float remap(float value, float minSrc, float maxSrc)
            {
                return saturate((value - minSrc) / (maxSrc - minSrc));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (dot(normalize(i.localPos.xyz - _point), normalize(_point - _anchor)) > 0)
                    discard;

                float v = remap(i.localPos.y, _BottomLine, _TopLine);
                fixed4 gradCol = lerp(_GradientBottomColor, _GradientTopColor, v);

                fixed4 col = gradCol * _EmissionScalar;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
