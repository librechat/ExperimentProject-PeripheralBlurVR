// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CustomShader/GaussianBlur"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_InnerRadius("Inner Radius", Range(0.0, 1.0)) = 0.1
		_OuterRadius("Outer Radius", Range(0.0, 1.0)) = 0.3
		_BlurMaskCenter("Blur Mask Center", Vector) = (0.5, 0.5, 0, 0) 
		_IndicatorColor("Indicator Color", COLOR) = (1.0, 0.0, 0, 0) 
	}
 
	SubShader
	{
		ZWrite Off
		Blend Off
 
		// Pass 0: Down Sample Pass
		Pass
		{
			ZTest Off
			Cull Off
 
			CGPROGRAM
 
			#pragma vertex vert_DownSmpl
			#pragma fragment frag_DownSmpl
 
			ENDCG
 
		}
 
		// Pass 1: Vertical Pass
		Pass
		{
			ZTest Always
			Cull Off
 
			CGPROGRAM
 
			#pragma vertex vert_BlurVertical
			#pragma fragment frag_Blur
 
			ENDCG
		}
 
		// Pass 2: Horizontal Pass
		Pass
		{
			ZTest Always
			Cull Off
 
			CGPROGRAM
 
			#pragma vertex vert_BlurHorizontal
			#pragma fragment frag_Blur
 
			ENDCG
		}
	}
 
  
	CGINCLUDE
 
	#include "UnityCG.cginc"
 
	sampler2D _MainTex;
	float _InnerRadius;
	float _OuterRadius;
	half4 _BlurMaskCenter; 
	float4 _IndicatorColor;

	// The size of a texel of the texture
	uniform half4 _MainTex_TexelSize;
	
	uniform half _DownSampleValue;
 


	// ====================== Pass 0 ========================= //
	struct VertexInput
	{
		float4 vertex : POSITION;
		half2 texcoord : TEXCOORD0;
	};
 
	struct VertexOutput_DownSmpl
	{
		
		float4 pos : SV_POSITION;	// (screen) pixel position
				
		half2 uv20 : TEXCOORD0;		// right up tex coordinate
		half2 uv21 : TEXCOORD1;		// left down tex coord
		half2 uv22 : TEXCOORD2;		// right down tex coord
		half2 uv23 : TEXCOORD3;		// left up tex coord

		half2 uv: TEXCOORD4;
	};
 
 	static const half4 GaussWeight[7] =
	{
		half4(0.0205,0.0205,0.0205,0),
		half4(0.0855,0.0855,0.0855,0),
		half4(0.232,0.232,0.232,0),
		half4(0.324,0.324,0.324,1),
		half4(0.232,0.232,0.232,0),
		half4(0.0855,0.0855,0.0855,0),
		half4(0.0205,0.0205,0.0205,0)
	}; 
 
	VertexOutput_DownSmpl vert_DownSmpl(VertexInput v)
	{
		VertexOutput_DownSmpl o;
 
		o.pos = UnityObjectToClipPos(v.vertex);
		
		// down sampling: pick the texels around center
		o.uv20 = v.texcoord + _MainTex_TexelSize.xy * half2(0.5h, 0.5h);;
		o.uv21 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, -0.5h);
		o.uv22 = v.texcoord + _MainTex_TexelSize.xy * half2(0.5h, -0.5h);
		o.uv23 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, 0.5h);
		
		o.uv = v.texcoord;

		return o;
	}
 
	fixed4 frag_DownSmpl(VertexOutput_DownSmpl i) : SV_Target
	{
		fixed4 color = (0,0,0,0);
 
		color += tex2D(_MainTex, i.uv20);
		color += tex2D(_MainTex, i.uv21);
		color += tex2D(_MainTex, i.uv22);
		color += tex2D(_MainTex, i.uv23);
 
		//return color / 4;
		return tex2D(_MainTex, i.uv); // no more down sampling
	}

	// ====================== Pass 1 & 2 ========================= //
 
	struct VertexOutput_Blur
	{
		float4 pos : SV_POSITION;
		half4 uv : TEXCOORD0;
		half2 offset : TEXCOORD1;
	};
 
	VertexOutput_Blur vert_BlurHorizontal(VertexInput v)
	{
		VertexOutput_Blur o;
 
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = half4(v.texcoord.xy, 1, 1);
		
		o.offset = _MainTex_TexelSize.xy * half2(1.0, 0.0) * _DownSampleValue;

		return o;
	}

	float getBlurDistance(half2 centerCoords, half2 pointCoords){
		float a = (centerCoords.x - pointCoords.x);
        float b = (centerCoords.y - pointCoords.y);
		float x = sqrt(a*a+b*b);
		return x;
	}
 
	VertexOutput_Blur vert_BlurVertical(VertexInput v)
	{
		VertexOutput_Blur o;
 
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = half4(v.texcoord.xy, 1, 1);

		o.offset = _MainTex_TexelSize.xy * half2(0.0, 1.0) * _DownSampleValue;
 
		return o;
	}
 
 	half4 frag_Blur(VertexOutput_Blur i) : SV_Target
	{
		half2 uv = i.uv.xy;
 
		half2 OffsetWidth = i.offset;		
		
		// TODO: change offset size not only be downsamplevalue but also centerdistance
		
		float blurDistance = getBlurDistance(half2(_BlurMaskCenter.x, _BlurMaskCenter.y), i.uv);

		float blurRatio = (blurDistance-_InnerRadius) / (_OuterRadius - _InnerRadius);
		blurRatio = clamp(blurRatio, 0.0, 1.0);		//0~1 between inner radius and outer radius							
		
		float indicateRate = 0.25;
		float indicateRatio = 1.0 - blurRatio;

		half4 color = 0;

		if(blurDistance < _InnerRadius) {
			color = tex2D(_MainTex, uv) + _IndicatorColor * indicateRate;
		}
		else {
			
			OffsetWidth *= (blurDistance < _OuterRadius)? blurRatio: 1.0;
			half2 uv_withOffset = uv - OffsetWidth * 3.0;
			
			for (int j = 0; j< 7; j++)
			{
				half4 texCol = tex2D(_MainTex, uv_withOffset);
				color += texCol * GaussWeight[j];
				uv_withOffset += OffsetWidth;
			}

			color += _IndicatorColor * indicateRate * indicateRatio;
		}	
 
		return color;
	}
 
	ENDCG
 
	FallBack Off
}
