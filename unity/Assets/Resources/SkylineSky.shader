// THE SKY IS THE WEBSITE'S HEADER (kanban #968): "wenn man als spectator raus zoomt kommt nur grau", which was the lower half of
// Unity's default sky under a world that ends. Above the horizon the panorama of tools/skyline-panorama.py; over its top edge the
// plain colour it ends in; below the horizon the header's orange grid, worked out per pixel so that it is sharp at any height.
Shader "Arena/SkylineSky"
{
    Properties
    {
        _MainTex ("Panorama", 2D) = "black" {}
        _Top ("Elevation of the panorama's top row, radians", Float) = 1.178097
        _GridScale ("Grid lines per eye height", Float) = 0.45
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _Top, _GridScale;
            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float3 dir : TEXCOORD0; };
            v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.dir = v.vertex.xyz; return o; }
            fixed4 frag(v2f i) : SV_Target
            {
                float3 d = normalize(i.dir);
                float2 uv = float2(atan2(d.x, d.z) / 6.2831853 + 0.5, saturate(asin(clamp(d.y, -1.0, 1.0)) / _Top));
                float3 sky = tex2Dlod(_MainTex, float4(uv, 0.0, 0.0)).rgb;
                float below = max(-d.y, 0.0008);
                float2 p = d.xz / below * _GridScale;
                float2 w = fwidth(p) + 0.0001;
                float2 g = abs(frac(p - 0.5) - 0.5) / w;
                float lit = (1.0 - saturate(min(g.x, g.y))) * saturate(below * 7.0);
                float3 ground = lerp(float3(0.106, 0.051, 0.110), float3(0.059, 0.031, 0.071), saturate(below * 2.0));
                ground = lerp(ground, float3(1.0, 0.54, 0.24), lit * 0.65);
                ground += float3(0.55, 0.20, 0.10) * exp(-below * 16.0);
                return fixed4(d.y >= 0.0 ? sky : ground, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
