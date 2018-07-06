Shader "ItemClicked"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "Queue" = "Transparent+2" "IgnoreProjector" = "False" "RenderType" = "Opaque" }

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 mainCol = tex2D(_MainTex, IN.uv_MainTex);

			o.Albedo = mainCol.rgb;
			o.Alpha = mainCol.a;
		}
		ENDCG
	}
}