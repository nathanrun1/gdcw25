Shader "Unlit/GridVisualizer"
{
    Properties
    {
        _GridSize ("Grid Size", Float) = 1.0 // Size of each grid square in world units
        _DarkColor ("Dark Color", Color) = (0.2, 0.2, 0.2, 1)
        _LightColor ("Light Color", Color) = (0.8, 0.8, 0.8, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Enable alpha blending
            ZWrite Off

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
                float3 worldPos : TEXCOORD0; // Pass world position to fragment shader
            };

            float _GridSize;
            fixed4 _DarkColor;
            fixed4 _LightColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Transform to world space
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate grid pattern based on world position
                float2 gridCoords = i.worldPos.xy / _GridSize; // Use X and Z for a flat grid
                int x = (int)floor(gridCoords.x);
                int y = (int)floor(gridCoords.y);
                bool isDark = (x + y) % 2 == 0;
                return isDark ? _DarkColor : _LightColor;
            }
            ENDCG
        }
    }
}