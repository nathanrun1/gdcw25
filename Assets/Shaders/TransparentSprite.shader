Shader "Custom/TransparentSpriteShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Main texture of the sprite
        _Transparency ("Transparency", Range(0, 1)) = 1.0 // Control transparency (0 = fully transparent, 1 = fully opaque)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Enable alpha blending
        ZWrite Off // Disable depth writing for transparency
        LOD 200

        Pass
        {
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
            float _Transparency;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 color = tex2D(_MainTex, i.uv);

                // Apply transparency to the alpha channel
                color.a *= _Transparency;

                return color;
            }
            ENDCG
        }
    }
}