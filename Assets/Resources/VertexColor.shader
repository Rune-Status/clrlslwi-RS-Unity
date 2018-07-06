Shader "Custom/Vertex Colored" {
	Properties{
	}
	SubShader {
		Pass {
			ColorMaterial AmbientAndDiffuse
			Lighting On
		}
	}
	Fallback "VertexLit", 1
}