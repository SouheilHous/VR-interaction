//  $Id: Chrome.shader 172 2015-03-13 14:05:02Z dirk $
//
// Virtence VFont package
// Copyright 2014 by Virtence GmbH
// http://www.virtence.com
//

Shader "Virtence/Chrome" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float2 SpUV;
		};
		
		void vert (inout appdata_full v, out Input o) {
           	UNITY_INITIALIZE_OUTPUT(Input, o);
           	float4 mvv = mul(UNITY_MATRIX_MV, v.vertex);
           	float3 Nn = normalize(COMPUTE_VIEW_NORMAL);
			float3 Neye = normalize(-mvv.xyz/mvv.z);           	
           	float3 refl = Neye - Nn * 2.0 * dot(Neye,Nn);
           	refl.z += 1.0;
           	float m = 1.0/(2.0*sqrt(dot(refl,refl)));
          	o.SpUV = refl.xy*m + float2(0.5,0.5);
        }
        
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.SpUV);
			o.Albedo = c.rgb;
			//o.Albedo = half3(abs(IN.SpUV), 0.0);
			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
