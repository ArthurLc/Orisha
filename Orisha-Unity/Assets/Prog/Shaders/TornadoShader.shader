
Shader "ShaderMan/TornadoShader"
	{

	Properties{
		_MainTex ("MainTex", 2D) = "white" {}
		}

	SubShader
		{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

			Pass
			{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct VertexInput {
				  float4 vertex : POSITION;
				  float2 uv:TEXCOORD0;
				  float4 tangent : TANGENT;
				  float3 normal : NORMAL;
				  //VertexInput
				};


			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv:TEXCOORD0;
				//VertexOutput
			};

			//Variables
			sampler2D _MainTex;

			const float pi = 3.14159;

			float3x3 xrot(float t)
			{
				  return float3x3(1.0, 0.0, 0.0,
					 0.0, cos(t), -sin(t),
					 0.0, sin(t), cos(t));
			}

			float3x3 yrot(float t)
			{
				   return float3x3(cos(t), 0.0, -sin(t),
					   0.0, 1.0, 0.0,
					   sin(t), 0.0, cos(t));
			}

			float3x3 zrot(float t)
			{
				  return float3x3(cos(t), -sin(t), 0.0,
					  sin(t), cos(t), 0.0,
					  0.0, 0.0, 1.0);
			}

			float sdCappedCylinder(float3 p, float2 h)
			{
				float2 d = abs(float2(length(p.xz),p.y)) - h;
				  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
			}

			float smin(float a, float b, float k)
			{
				float res = exp(-k*a) + exp(-k*b);
					return -log(res) / k;
			}

			float map(float3 pos, float q)
			{
				float so = q;
				float sr = atan2(pos.x,pos.z);
				so += pos.y * 0.5;
				so += sin(pos.y*75.0 + sr - _Time.y) * 0.005;
				so += sin(pos.y*125.0 + sr - _Time.x*50.0) * 0.004;

				//Vitesse de déplacement de la tornade(ondulation)
				float ro = pos.y*10.0 - _Time.y*0.1;

				pos.xz += float2(cos(ro), sin(ro)) * 0.07;
				float d = sdCappedCylinder(pos, float2(so, 10.0)) - 0.08 ;// - 0.08
				float k = pos.y;//+5.0 enlève la parti basse de la tornade
				return smin(d,k,10.0);
			}

			float3 surfaceNormal(float3 pos)
			{
				float3 delta = float3(0.01, 0.0, 0.0);
				float3 normal;
				normal.x = map(pos + delta.xyz,0.0) - map(pos - delta.xyz,0.0);
				normal.y = map(pos + delta.yxz,0.0) - map(pos - delta.yxz,0.0);
				normal.z = map(pos + delta.zyx,0.0) - map(pos - delta.zyx,0.0);
				return normalize(normal);
			}

			float trace(float3 o, float3 r, float q)
			{
				float t = 0.0;
				float ta = 0.0;
				for (int i = 0; i < 8; ++i) {
					float d = map(o + r * t, q);
					 t += d * 1.0;
				}
				return t;
			}





			VertexOutput vert(VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(VertexOutput i) : SV_Target
			{

				float2 uv = i.uv / 1;
				uv = uv * 2.0 - 1.0;

				float3 r = normalize(float3(uv, 1.0));
				float tn = tex2D(_MainTex, float2(_Time.y*0.1,0.0)).x;
				tn = tn * 2.0 - 1.0;
				
				//Clip chaotique
				//float3x3 rot = mul(zrot(sin(tn)*0.2), xrot(-pi*0.05 + sin(tn)*0.1));
				//r = mul(rot, r);

				float3 o = float3(0.0, 0.15, -0.5);

				float t = trace(o, r, 0.0);
				float3 world = o + r * t;
				float3 sn = surfaceNormal(world);

				float4 vol = float4(0.0,0.0,0.0,0.0);

				for (int i = 0; i < 3; ++i) {
					float rad = 0.2 + float(1 + i) / 3.0;
					float tt = trace(o,r,rad );
					float3 wa = o + r * tt;
					float atlu = atan2(wa.z,wa.x) - tt * 4.0 + _Time.y;
					float atlv = acos(wa.y / length(wa)) + tt * 4.0;
					float4 at = tex2D(_MainTex, float2(atlu,atlv)).xxxx;
					   vol += at / 3.0;

				}
				float prod = max(dot(sn, -r), 0.0);

				float fd = map(world, 0.0);
				float fog = 1.0 / (1.0 + t * t * 0.1 + fd * 10.0);

				//Bakcground color
				float4 sky = float4(0.0, 0.0, 0.0, 0.0) / 255.0;

				float4 fgf = float4(210.0, 180.0, 140.0, 255.0) / 255.0;
				float4 fgb = float4(139.0, 69.0, 19.0, 255.0) / 255.0;
				float4 fg = lerp(fgb, fgf, prod);

				float4 back = lerp(fg, sky, 1.0 - fog);

				float4 mmb = lerp(vol, back, 1.0);

				float4 fc = mmb * float4(1.0,1.0,1.0,1.0);
					

				fc.a = fc.a < 0.2 ? 0.0 : fc.a;

				for (float index = 0.0; index < 0.5; index += 0.01)
				{
					fc.a = fc.a < index + 0.2 ? lerp(0.0, fc.a, index * 2.0) : fc.a;
				}

				return fc;
			}

			ENDCG
			}
		}
}