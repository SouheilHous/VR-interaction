Shader "Matej Vanco/Mesh Deformation Package/MD_Blob" {
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainNormal ("Normal Texture", 2D) = "bump" {}
		_NormalAmount ("Bump Power", Range(0.01,2)) = 0.5
		_Specular ("Specular", Range(-1,1)) = 0.5
		_Emissive ("Emission Intensity", Range(0,5)) = 0

		[Header(Blob Settings)]
	    [KeywordEnum(NoAnimation, Fish, SoftFish, Jumping)] 
		_Anim("Animation Mode", Float) = 0
		[Space(5)]
		_Amount ("Direction Amount", Range(-20,20)) = 0.5
		_Direction ("To Direction",Vector) = (0,1,0,1)
		[Space(5)]
		_EAmount("Edges Amount", Range(-20,20)) = 0.5
		_Phase ("Edges",Vector) = (0,0,0,0)
	
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
     };

     sampler2D _MainTex;
     sampler2D _MainNormal;
     half4 _Color;
     float _Specular;
     float _NormalAmount;
     fixed _Emissive;

	 half _Anim;
     half4 _Direction;
     float _Amount;
	 float _EAmount;
     half4 _Phase;

     void vert(inout appdata_full v)
     {
		 if(_Anim == 0)
             v.vertex.xyz +=  v.vertex * v.vertex *_Amount * _Direction;
		 else  if (_Anim == 1)
			 v.vertex.xyz += sin(v.vertex.x + _Time * _Amount)* _Direction;
		 else  if (_Anim == 2)
			 v.vertex.xyz += sin(v.normal.x + _Time * _Amount)* _Direction;
		 else  if (_Anim == 3)
			 v.vertex.xyz += sin(v.vertex.y + _Time * _Amount)* _Direction;
         v.vertex.x += (v.vertex *v.vertex *_Phase.x) * _EAmount;
         v.vertex.y += (v.vertex *v.vertex *_Phase.y)*_EAmount;
         v.vertex.z += (v.vertex *v.vertex *_Phase.z)*_EAmount;
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
