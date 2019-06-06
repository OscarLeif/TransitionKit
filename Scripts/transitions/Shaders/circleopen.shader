Shader "prime[31]/Transitions/CircleOpen"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Progress("Progress", Range(0.0,1.0)) = 0.0
		_Color("Background Color", Color) = (0, 0, 0, 1)
		_Smoothness("Smoothness", Range(0.0, 0.3)) = 0.03
		_AspectRatio("Aspect Ratio",Float) = 1
		_X("X", Float) = 0.5
		_Y("Y", Float) = 0.5
	}

		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Lighting Off

			Pass
			{
				ZTest Always Cull Off ZWrite Off
				Fog { Mode off }
				CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"

				// uniforms
				sampler2D _MainTex;
				uniform fixed _Progress;
				uniform fixed4 _Color;
				uniform half _Smoothness;
				uniform fixed _Opening;
				uniform half _AspectRatio;
				uniform half _X;
				uniform half _Y;

				static float SQRT_2 = 1.414213562373;

				half4 frag(v2f_img i) :COLOR
				{
					float x = _Opening == 1 ? _Progress : 1.0 - _Progress;
					float2 ratio2 = float2(1.0, 1 / _AspectRatio);
					float m = smoothstep(-_Smoothness, 0.0, SQRT_2 * distance(float2(_X,_Y) *ratio2 , i.uv*ratio2) - x * (_AspectRatio < 1 ? 1 / _AspectRatio : _AspectRatio + _Smoothness));
					return lerp(tex2D(_MainTex, i.uv), _Color, m);
				}
				ENDCG
			} // end Pass
		} // end SubShader
		FallBack off
}