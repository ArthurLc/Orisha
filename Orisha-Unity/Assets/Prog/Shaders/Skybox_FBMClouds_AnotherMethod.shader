// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: add 'Sun' elements

Shader "Skybox/FBMClouds" 
{
	Properties 
	{
		[Header(Sun)]
        _SunSize ("Sun Size", Range(0,1)) = 0.04
        _AtmosphereThickness ("Atmoshpere Thickness", Range(0,5)) = 1.0
        _SkyTint ("Sky Tint", Color) = (.5, .5, .5, 1)
        _GroundColor ("Ground", Color) = (.369, .349, .341, 1)
        _Exposure("Exposure", Range(0, 8)) = 1.3
		
        _Altitude0("Altitude (bottom)", Float) = 1500
        _FarDist("Far Distance", Float) = 30000
		
		[Header(Sky and horizon)]
		_ColorSky("Sky color", Color) = (0, 0, 1, 1)
		_ColorHorizon("Horizon fog color", Color) = (1, 1, 1, 1)
		_SkyExponent("Horizon fog : size on sky", Range(0, 10)) = 8.5
		_CloudsExponent("Horizon fog : size on FBM", Range(1, 10)) = 3

		[Header(Clouds)]
		_Color1("Color A", Color) = (0.101961, 0.619608, 0.666667, 1)
		_Color2("Color B", Color) = (0.666667, 0.666667, 0.498039, 1)
		_Color3("Color C", Color) = (0 ,0, 0.164706, 1)
		_Color4("Color D", Color) = (0.666667, 1, 1, 1)
		_Brightness("Brightness", Range(0, 10)) = 0.6
		_Speed("Speed", Range(0,10)) = 1.0
		_Density("Density", Range(-100,100)) = 1.0
	}

	CGINCLUDE
	
    float _Altitude0;
    float _FarDist;

	float4 _ColorSky;
	float _SkyExponent;
	float4 _ColorHorizon;
	float4 _Color1;
	float4 _Color2;
	float4 _Color3;
	float4 _Color4;
	float _CloudsExponent;

	float _Speed;
	float _Density;
	float _Brightness;

	struct appdata_t
	{
		float4 vertex : POSITION;
	};

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float4 uv : TEXCOORD0;
        float3 rayDir : TEXCOORD1;
        float3 groundColor : TEXCOORD2;
        float3 skyColor : TEXCOORD3;
        float3 sunColor : TEXCOORD4;
    };
	
	#include "ProceduralSky.cginc"


	float random(in float2 _st)
	{
		return frac(sin(dot(_st.xy, float2(12.9898, 78.233)))* 43758.5453123);
	}

	// Based on Morgan McGuire @morgan3d
	// https://www.shadertoy.com/view/4dS3Wd
	float noise(in float2 _st)
	{
		int2 i = floor(_st);
		float2 f = frac(_st);

		// Four corners in 2D of a tile
		float a = random(i);
		float b = random(i + float2(1.0, 0.0));
		float c = random(i + float2(0.0, 1.0));
		float d = random(i + float2(1.0, 1.0));

		float2 u = f * f * (3.0 - 2.0 * f);

		return lerp(a, b, u.x) +
			(c - a)* u.y * (1.0 - u.x) +
			(d - b) * u.x * u.y;
	}

#define NUM_OCTAVES 5

	float fbm(in float2 _st)
	{
		float v = 0.0;
		float a = 0.5;

		float2 shift = float2(100, 0);
		// Rotate to reduce axial bias
		float2x2 rot = float2x2(cos(0.5), sin(0.5),
			-sin(0.5), cos(0.50));

		for (int i = 0; i < NUM_OCTAVES; ++i)
		{
			v += a * noise(_st);
			_st = mul(rot, _st) * 2.0 + shift;
			a *= 0.5;
		}
		return v;
	}

	v2f vert(appdata_t v)
	{
		v2f output;
		
        float4 p = UnityObjectToClipPos(v.vertex);

		float4x4 modelMatrix = unity_ObjectToWorld;
		output.rayDir = mul(modelMatrix, v.vertex).xyz
			- _WorldSpaceCameraPos;
		output.vertex = UnityObjectToClipPos(v.vertex);

		output.uv = normalize(mul(unity_ObjectToWorld, v.vertex));

        vert_sky(v.vertex.xyz, output);
		
		return output;
	}


	fixed4 frag(v2f i) : SV_Target
	{
        float3 sky = frag_sky(i);
        float3 ray = -i.rayDir;
		float4 colorReturn = float4(1, 1, 1, 1);

        float3 acc = 0;
        float dist0 = _Altitude0 / ray.y;
		
		// Projection sur la sphère de la skybox
		float sphereX = i.uv.x;
		float sphereZ = i.uv.y;

		float sphereY = sqrt(1.0 - pow(sphereX, 2.0) - pow(sphereZ, 2.0));

		float uvX = sphereX / sphereZ;
		float uvY = sphereY / sphereZ;

		float3 v = normalize(i.uv);


		// Noise fbm des nuages
		float2 st = float2(uvX, uvY) * _Density;
		st += st * abs(sin(_Time*0.1)*0.1);

		float3 color = float3(0, 0, 0);

		float2 q = float2(0, 0);
		q.x = fbm(st + 0.0f* _Time * _Speed);
		q.y = fbm(st + float2(1, 0));

		float2 r = float2(0, 0);
		r.x = fbm(st + 1.0*q + float2(1.7, 9.2) + 0.15*_Time * _Speed);
		r.y = fbm(st + 1.0*q + float2(8.3, 2.8) + 0.126*_Time * _Speed);

		float f = fbm(st + r);

		color = lerp(_Color1.xyz,
			_Color2.xyz,
			clamp(f*f*4.0, 0.0, 1.0));

		color = lerp(color,
			_Color3.xyz,
			clamp(length(q), 0.0, 1.0));

		color = lerp(color,
			_Color4.xyz,
			clamp(length(r.x), 0.0, 1.0));


		// Lerp de couleur en fonction de la hauteur (ciel-horizon-nuages)
		float p = i.uv.y;
		float p1 = 1 - pow(min(1, 1 - p), _SkyExponent);
		float p3 = 1 - pow(min(1, 1 + p), _CloudsExponent);
		float p2 = 1 - p1 - p3;
		
		half3 c_sky = _ColorSky * p1 + _ColorHorizon * p2 + half3((f*f + _Brightness*f + _Brightness)*color) * p3;
		acc = lerp(acc, sky, saturate(dist0 / _FarDist));
		//half3 c_sun = _SunColor * min(pow(max(0, dot(v, _SunVector)), _SunAlpha) * _SunBeta, 1);

		// Elimination des artéfacts (pixels noirs car valeur en y parfois à 0 ou négative)
		if (c_sky.y > 0.1)
			colorReturn.y = c_sky.y;
		colorReturn.xz = c_sky.xz;

		//return colorReturn;
		return half4(colorReturn + acc, 1);
	}

	ENDCG


	SubShader 
	{
		Tags { "RenderType" = "Background" "Queue"="Background" "PreviewType"="Skybox"  }
        Cull Off ZWrite Off
		Pass
		{
			Fog{ Mode Off }
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
            #pragma target 3.0
			ENDCG
		}
	}
}

