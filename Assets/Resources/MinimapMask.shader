Shader "MaskedTexture"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Mask("Culling Mask", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.1
		_Start("Start", Vector) = (0.5, 0.5, 0, 0)
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" "IgnoreProjector" = "True" }

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		sampler2D _Mask;
		float _Cutoff;
		float2 _Start;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 mainCol = tex2D(_MainTex, IN.uv_MainTex);

			fixed2 relative = IN.uv_MainTex - _Start;
			relative = relative * (512.0 / 152.0);
			fixed4 texTwoCol = tex2D(_Mask, relative);

			o.Albedo = mainCol.rgb;
			o.Alpha = texTwoCol.a;
		}
		ENDCG
	}
}