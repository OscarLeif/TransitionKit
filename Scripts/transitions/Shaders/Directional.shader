Shader "prime[31]/Transitions/Directional"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Progress("Progress", Range(0.0,1.0)) = 0.0
		_Direction("Direction", Vector) = (1.0,0.0,0.0,0.0)
		_Color("Background Color", Color) = (0, 0, 0, 0)
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma vertex vert_img
				#pragma fragment frag
				#include "UnityCG.cginc"

				//uniforms
				sampler2D _MainTex;
				uniform fixed4 _Color;
				uniform float _Progress;
				uniform float _Opening;
				uniform float2 _Direction;				

				fixed4 frag(v2f_img i):COLOR
				{
					float2 p = i.uv  + _Progress * sign(_Direction);
					float2 f = (p.xy - floor(p.xy)); // fract(p){return p  - floor(p)}
					//float m = smoothstep(0.0, p.y);
					//return lerp(tex2D(_MainTex, i.uv), fixed4(0, 0, 0, 0), m);
					float m = step(0.0, p.y) * step(p.y, 1.0) * step(0.0, p.x) * step(p.x, 1.0);
					return lerp( _Color, fixed4(0, 0, 0, 0), m);
				}
				ENDCG
			} // end pass
		} //end SubShader
		FallBack "Diffuse"
}
