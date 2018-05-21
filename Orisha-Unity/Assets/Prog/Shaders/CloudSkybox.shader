// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CloudSkybox"
{
    Properties
    {
        [Space]
        _Altitude0("Altitude (bottom)", Float) = 1500
        _FarDist("Far Distance", Float) = 30000

        [Space]
        _SunSize ("Sun Size", Range(0,1)) = 0.04
        _AtmosphereThickness ("Atmoshpere Thickness", Range(0,5)) = 1.0
        _SkyTint ("Sky Tint", Color) = (.5, .5, .5, 1)
        _GroundColor ("Ground", Color) = (.369, .349, .341, 1)
        _Exposure("Exposure", Range(0, 8)) = 1.3
    }

    CGINCLUDE

    struct appdata_t
    {
        float4 vertex : POSITION;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 rayDir : TEXCOORD1;
        float3 groundColor : TEXCOORD2;
        float3 skyColor : TEXCOORD3;
        float3 sunColor : TEXCOORD4;
    };

    #include "ProceduralSky.cginc"

    v2f vert(appdata_t v)
    {
        float4 p = UnityObjectToClipPos(v.vertex);

        v2f o;

        o.vertex = p;
        o.uv = (p.xy / p.w + 1) * 0.5;

        vert_sky(v.vertex.xyz, o);

        return o;
    }

    float _Altitude0;
    float _FarDist;
	
    fixed4 frag(v2f i) : SV_Target
    {
        float3 sky = frag_sky(i);

        float3 ray = -i.rayDir;

        float dist0 = _Altitude0 / ray.y;
        float3 acc = 0;

        acc = lerp(acc, sky, saturate(dist0 / _FarDist));

        return half4(acc, 1);
    }

    ENDCG

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}