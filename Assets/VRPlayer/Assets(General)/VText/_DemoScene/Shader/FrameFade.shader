// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Virtence/FrameFade" {
	Properties {
		_Color ("Color" ,color) = (0.8, 0.7, 0.1)
		_TopLimit ("Top limit", Range(0.5, 1.0)) = 0.9
		_TopFade ("Top fade", Range(0.1, 1.0)) = 0.8
	}
	
	SubShader {
		Tags {  "Queue" = "Overlay"}
		Pass {		
			Lighting Off
			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			Fog { Mode Off }
			// Blend Zero SrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "HLSLSupport.cginc"
			
			float _TopLimit;
			float _TopFade;
			fixed3 _Color;
			
			struct vin {
				float4 v : POSITION;
			};
			struct vout {
				float4 p : SV_POSITION;
				float  yss : TEXCOORD0;
			};
			
			vout vert(vin IN)
			{
				vout OUT;
				float4 sp = UnityObjectToClipPos(IN.v);
				OUT.p = sp;
				// to range 0..1
				if(_ProjectionParams.x < 0.0) {
				OUT.yss = 1.0-(sp.y/sp.w+1.0)*0.5;
				} else {
				OUT.yss = (sp.y/sp.w+1.0)*0.5;
				}
				return OUT;
			}
			
			fixed4 frag(vout IN) : COLOR {
				fixed alpha = 1.0f;
				float screenY = IN.yss;
				if(screenY > _TopFade) {
					if(screenY > _TopLimit) {
						alpha = 0.01;
						// return fixed4(0.0, 0.0, 1.0, alpha);
						discard;
					} else {
					    float delta = _TopLimit-_TopFade;
						alpha = 1.0-((screenY-_TopFade)/delta);
						// return fixed4(0.0, 1.0, alpha, alpha);
					}
					// return fixed4(0.0, screenY, alpha, 1.0);
				}
				// return fixed4(0.0, screenY, alpha, 1.0);
				return fixed4(_Color, alpha);
			}
			ENDCG
		}
	} 
}
