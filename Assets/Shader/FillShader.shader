Shader "Custom/FillShader" {
	Properties{
		_Color("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
		_Filling("IsFill", float) = 1.0
		_FillMaxY("Top Value", float) = 1.0
		_FillMinY("Bot Value", float) = 0.0
		_FillAmount("Fill Value", Range(0.0, 1.0)) = 0.5
		_FillGap("Fill Gap", float) = 0.1
		_FillGapColor("Fill Gap Color", Color) = (0.5, 0.5, 0.5, 1)
	}

	SubShader{
			Tags{ "RenderType" = "Fade" }
			LOD 200
			Cull Off
			CGPROGRAM
#pragma surface surf ToonRamp

			sampler2D _Ramp;
			float4 _FillGapColor;

			struct Input {
				float2 uv_MainTex : TEXCOORD0;
				float3 worldPos;
				float3 viewDir;
			};

			float3 viewDir;
			int isGap;
			// custom lighting function that uses a texture ramp based
			// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
			inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
			{
				if (isGap)
					return _FillGapColor;

				if (dot(s.Normal, viewDir) < 0)
					return _FillGapColor;

#ifndef USING_DIRECTIONAL_LIGHT
				lightDir = normalize(lightDir);
#endif

				half d = dot(s.Normal, lightDir)*0.5 + 0.5;
				half3 ramp = tex2D(_Ramp, float2(d, d)).rgb;

				half4 c;
				c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
				c.a = 0;
				return c;
			}


			sampler2D _MainTex;
			float4 _Color;
			float _FillAmount;
			float _FillMaxY;
			float _FillMinY;
			float _FillGap;
			float _Filling;

			void surf(Input IN, inout SurfaceOutput o) {
				if(_Filling < 0){
					half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
					o.Albedo = c.rgb;
					o.Alpha = c.a;
					isGap = 0;
				}
				else 
				{
					float waveOffset = sin((IN.worldPos.x * IN.worldPos.z) * 2 + _Time[3] + o.Normal) / 15;
					float clipY = _FillMinY + (_FillMaxY - _FillMinY) * _FillAmount + _FillGap + waveOffset;
					float clipValue = clipY - IN.worldPos.y;
					if(_Filling > 0) clip(clipValue);

					viewDir = IN.viewDir;

					if (IN.worldPos.y <= clipY)
					{
						half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
						o.Albedo = c.rgb;
						o.Alpha = c.a;
						isGap = 0;
					}
					else
					{
						o.Albedo = _FillGapColor.rgb;
						o.Alpha = _FillGapColor.a;
						isGap = 1;
					}
				}				
			}
			ENDCG
		}
		Fallback "Diffuse"
}