Shader "Matej Vanco/Mesh Deformation Package/MD_Tire"
{
Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainNormal ("Normal Texture", 2D) = "bump" {}
		_NormalAmount ("Bump Power", Range(0.01,2)) = 0.5
		_Specular ("Specular", Range(-1,1)) = 0.5
		_Emissive ("Emission Intensity", Range(0,5)) = 0

		[Header(Tire Settings)]
		_Amount ("Amount", Range(-50,50)) = 0.5
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
      #pragma surface surf Standard vertex:vert addshadow fullforwardshadows
	 #pragma target 3.0

      struct Input
     {
         float2 uv_MainTex;
         float2 uv_MainNormal;
          float3 worldPos;
     };

     sampler2D _MainTex;
     sampler2D _MainNormal;
     half4 _Color;
     float _Specular;
     float _NormalAmount;
     fixed _Emissive;

     float _Amount;

     void vert(inout appdata_full v)
     {
         v.vertex.x += v.vertex/2 *v.vertex/2*v.vertex.x *_Amount;
         v.vertex.y += v.vertex/2 *v.vertex/2*v.vertex.y *_Amount;
         v.vertex.z += v.vertex/2 *v.vertex/2*v.vertex.z *_Amount;
         v.vertex.w += v.vertex/2 *v.vertex/2*v.vertex.w *_Amount;
     }
     void surf (Input IN, inout SurfaceOutputStandard o) 
     {
         half4 c = tex2D (_MainTex, IN.uv_MainTex);
         fixed3 n = UnpackNormal( tex2D(_MainNormal,IN.uv_MainNormal));
         n.z =  n.z / _NormalAmount;
         o.Albedo = c.rgb * _Color.rgb;
         o.Normal = normalize(n);
          o.Emission = c.rgb * _Emissive;
         o.Smoothness = _Specular;
         o.Alpha = c.a * _Color.a;
     }
 
     ENDCG
	}
	FallBack "Diffuse"
}