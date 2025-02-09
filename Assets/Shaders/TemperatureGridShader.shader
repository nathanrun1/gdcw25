Shader "Unlit/TemperatureGridShader"
{
    Properties
    {
        _GridSize ("Grid Size", Float) = 1.0 // Size of each grid square in world units
        _TemperatureTex ("Temperature Texture", 2D) = "white" {} // Texture for temperature data
        _MinTemp ("Minimum Temperature", Float) = 0
        _MaxTemp ("Maximum Temperature", Float) = 1
        _Transparency ("Transparency", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Enable alpha blending
            ZWrite Off // Disable depth writing for transparency

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // UV coordinates for temperature texture
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0; // World position for grid calculation
                float2 uv : TEXCOORD1; // UV coordinates for temperature texture
            };

            float _GridSize;
            sampler2D _TemperatureTex;
            float _MinTemp;
            float _MaxTemp;
            float _Transparency;
            int _OffsetX;
            int _OffsetY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Transform to world space
                o.uv = v.uv; // Pass UV coordinates to fragment shader
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate grid pattern based on world position
                float2 gridCoords = i.worldPos.xy / _GridSize;
                int x = (int)floor(gridCoords.x) - (int)tex2D(_TemperatureTex, float2(0,100)).b;
                int y = (int)floor(gridCoords.y) - (int)tex2D(_TemperatureTex, float2(1,100)).b;
                //bool isDark = (x + y) % 2 == 0;

                // Sample temperature data from texture
                float temperature = tex2D(_TemperatureTex, float2(x,y)).b; // Assuming grayscale texture
                //float temperature = 0.5;
                temperature = clamp(temperature, _MinTemp, _MaxTemp); // Clamp to min/max range

                // Map temperature to a color (e.g., blue for cold, red for hot)
                fixed4 coldColor = fixed4(0, 0, 1, 1); // Blue
                fixed4 midColor = fixed4(1, 1, 1, 1); // White
                fixed4 hotColor = fixed4(1, 0, 0, 1); // Red
                fixed4 temperatureColor = (temperature < 0.5) ? lerp(coldColor, midColor, temperature * 2) : lerp(midColor, hotColor, (temperature - 0.5) * 2);

                // Combine grid pattern with temperature color
                //fixed4 gridColor = isDark ? fixed4(0.2, 0.2, 0.2, 1) : fixed4(0.8, 0.8, 0.8, 1);
                //fixed4 finalColor = lerp(gridColor, temperatureColor, 0.5)
                fixed4 finalColor = temperatureColor;

                finalColor.a *= _Transparency;

                return finalColor;
            }
            ENDCG
        }
    }
}