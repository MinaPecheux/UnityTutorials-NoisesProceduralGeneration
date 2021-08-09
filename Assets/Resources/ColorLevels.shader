Shader "Custom/Color Levels"
{
    Properties
    {
        _MaxHeight ("Max Height", float) = 50.0
        _BandWidth ("Band Width", float) = 10.0
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

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 worldPos : POSITION2;
            };

            float _MaxHeight;
            float _BandWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float v = round((i.worldPos.y / _MaxHeight) * _BandWidth) / _BandWidth;
                return fixed4(v, v, v, 1);
            }
            ENDCG
        }
    }
}
