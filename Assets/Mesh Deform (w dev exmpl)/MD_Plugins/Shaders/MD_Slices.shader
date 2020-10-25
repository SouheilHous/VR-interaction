Shader "Matej Vanco/Mesh Deformation Package/MD_Slices" {

    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainNormal ("Normal Texture", 2D) = "bump" {}
		_NormalAmount ("Bump Power", Range(0.01,2)) = 0.5
		_Specular ("Specular", Range(-1,1)) = 0.5
		_Emissive ("Emission Intensity", Range(0,5)) = 0

		[Header(Slices Settings)]
      _Slices ("Slices",Range(0.1,50)) = 5
       _Size ("Size",Range(0,1)) = 0.5
       _Tilling  ("Tilling",float) = 1.0
    }

    SubShader 
    {
      Tags { "RenderType" = "Opaque" }
      Cull Off

      CGPROGRAM
      #pragma surface surf Standard addshadow fullforwardshadows
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

      float _Slices;
      float _Size;
      float _Tilling;

      void surf (Input IN, inout SurfaceOutputStandard o) 
      {
         half4 c = tex2D (_MainTex, IN.uv_MainTex);
         clip (frac((IN.worldPos.y+_Tilling+IN.worldPos.z*0.1) * _Slices) -_Size);
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
    Fallback "Diffuse"
  }
