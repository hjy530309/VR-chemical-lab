Shader "Custom/TextureTopBottomColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorBottom ("Bottom Color", Color) = (0.6,0.3,0,1) // 갈색
        _ColorTop ("Top Color", Color) = (0.5,0.5,0.5,1)     // 회색
        _Split ("Split Ratio", Range(0,1)) = 0.75           // 하단 비율
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
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _ColorBottom;
            float4 _ColorTop;
            float _Split;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // UV.y 기준으로 색 결정
                fixed4 overlayColor = (i.uv.y < _Split) ? _ColorBottom : _ColorTop;

                // 텍스처와 색 곱하기
                return texColor * overlayColor;
            }
            ENDCG
        }
    }
}
