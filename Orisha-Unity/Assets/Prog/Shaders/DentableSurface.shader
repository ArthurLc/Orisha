Shader "IndentSurface/DentableSurface" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _Smoothness("Smoothness", 2D) = "white" {}
        _Indentmap ("Indentation", 2d) = "white" {}
        _Tess("Tessellation", Range(1,128)) = 4
        _IndentDepth ("Indentation Depth", Range(0,0.5)) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM


		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:vert tessellate:tessDistance nolightmap
		#include "Tessellation.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 5.0

			sampler2D _MainTex;
        sampler2D _Indentmap;
        sampler2D _Smoothness;
        sampler2D _BumpMap;
		struct Input {
            float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_Smoothness;
            float3 normal;
		};

        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
        };

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        float _Tess;
        float _IndentDepth;


        float4 tessFixed()
        {
            return _Tess;
        }

        float4 tessDistance(appdata v0, appdata v1, appdata v2) {
			float minDist = 0.001;
            float maxDist = 10000000.0;
            return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
        }

        float3 calcNormal(float2 texcoord) 
        {
            const float3 off = float3(-0.001f, 0, 0.001f); // texture resolution to sample exact texels
            const float2 size = float2(0.01, 0.0); // size of a single texel in relation to world units

            float s01 = tex2Dlod(_Indentmap, float4(texcoord.xy - off.xy, 0, 0)).x * _IndentDepth;
            float s21 = tex2Dlod(_Indentmap, float4(texcoord.xy - off.zy, 0, 0)).x * _IndentDepth;
            float s10 = tex2Dlod(_Indentmap, float4(texcoord.xy - off.yx, 0, 0)).x * _IndentDepth;
            float s12 = tex2Dlod(_Indentmap, float4(texcoord.xy - off.yz, 0, 0)).x * _IndentDepth;

            float3 va = normalize(float3(size.xy, s21 - s01));
            float3 vb = normalize(float3(size.yx, s12 - s10));
            
            //return float3(s01, s12, 0);
            return normalize(cross(va, vb));
        }


        void vert(inout appdata v) {
            //v.vertex.x += sin(_Time.y * 1.0f + v.vertex.y * 1.0f) * 1.0f;
            float height = tex2Dlod(_Indentmap, float4(v.texcoord.xy, 0, 0));
            v.vertex.z += height * _IndentDepth;
			v.normal = normalize(calcNormal(v.texcoord) + v.normal);
        }

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

            // temp normal debug
            //o.Albedo = IN.normal;
			//o.Albedo = float4(IN.uv_MainTex, 0, 1) * 5;


            //o.Smoothness = tex2D(_Smoothness, IN.uv_Smoothness);

			o.Alpha = c.a;

			o.Normal = float3(0, 1, 0) * 0.1f + 0.9f * UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)).rbg;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
