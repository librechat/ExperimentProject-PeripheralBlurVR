Shader "PersonalPost/Blur"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
//		_MaskTex ("Mask texture", 2D) = "white" {}

		_InnerRadius ("Inner Radius", Float ) = 0.1
		_OuterRadius ("Outer Radius", Float ) = 0.3
		

        _CoeffStep ("Coefficent step", Float) = 0.32
		_MaskFactor ("Mask Factor", Range(1, 2000)) = 500
		_KernelRadius ("Kernel Radius", Range(1, 60)) = 3
		_BlurMaskCenter("Blur Mask Center", Vector) = (0.5, 0.5, 0, 0) 
		_IndicatorColor("Indicator Color", COLOR) = (1.0, 0.0, 0, 0) 
	}
	
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"
		
			uniform sampler2D _MainTex;
			uniform sampler2D _MaskTex;
			float _OuterRadius;
			float _InnerRadius;

			float _CoeffStep;
			int _MaskFactor;
			int _KernelRadius;	//max kernel radius
			half4 _BlurMaskCenter; 
			float4 _IndicatorColor;
			

			float getBlurDistance(half2 centerCoords, half2 pointCoords){
				float a = (centerCoords.x - pointCoords.x);
                float b = (centerCoords.y - pointCoords.y);
				float x = sqrt(a*a+b*b);
				return x;
			}

			fixed4 _Blur(v2f_img i, sampler2D mainTex, int kernelRadius){
				fixed4 targetColor = fixed4(0,0,0,0);

				float rate = 1.0 / (kernelRadius * kernelRadius * 4);

				[unroll(20)]
				for(int x=0; x < kernelRadius; x++){
					[unroll(20)]
					for(int y=0; y < kernelRadius; y++){
						targetColor += tex2D(mainTex, float2(i.uv.x + x, i.uv.y + y)) * rate;
						targetColor += tex2D(mainTex, float2(i.uv.x - x, i.uv.y + y)) * rate;
						targetColor += tex2D(mainTex, float2(i.uv.x + x, i.uv.y - y)) * rate;
						targetColor += tex2D(mainTex, float2(i.uv.x - x, i.uv.y - y)) * rate;
					}
				}
				return targetColor;
			}

			fixed4 frag (v2f_img i): COLOR {
				float IndicateRate = 0.5;
				float blurDistance = getBlurDistance(half2(_BlurMaskCenter.x, _BlurMaskCenter.y), i.uv);

				float blurRatio = (blurDistance-_InnerRadius) / (_OuterRadius - _InnerRadius);
				blurRatio = clamp(blurRatio, 0.0, 1.0);												//0~1 between inner radius and outer radius
				
				float indicateRatio = 1.0 - blurRatio;

				int kernelRadius = (int) (blurRatio * (float)_KernelRadius);
				if(kernelRadius < 1) kernelRadius = 1;

				if(blurDistance < _InnerRadius){
					return tex2D(_MainTex, float2(i.uv.x, i.uv.y));
					//return tex2D(_MainTex, float2(i.uv.x, i.uv.y)) + _IndicatorColor * IndicateRate;
				}
				else if (blurDistance < _OuterRadius) {
					//return _Blur(i, _MainTex, kernelRadius) + _IndicatorColor * IndicateRate * indicateRatio;
					return _Blur(i, _MainTex, kernelRadius);
				}
				else {
					return _Blur(i, _MainTex, _KernelRadius);
				}
			}

			

			ENDCG
		}
	}
}
