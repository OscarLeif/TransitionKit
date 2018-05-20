Shader "prime[31]/Transitions/CircleOpen"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Progress ("Progress", Range(0.0,1.0)) = 0.0
		_Color("Background Color", Color) = (0, 0, 0, 1)
        _Smoothness("Smoothness", Range( 0.0, 0.3)) = 0.3
        _Opening("Opening", Range(0.0, 1.0)) = 1 // 1 true
		_Ratio("Ratio",Float) = -1.0
		_X("X", Float) = 0.5
		_Y("Y", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

Pass
{
CGPROGRAM
#pragma exclude_renderers ps3 xbox360
#pragma fragmentoption ARB_precision_hint_fastest
#pragma vertex vert_img
#pragma fragment frag

#include "UnityCG.cginc"

// uniforms
sampler2D _MainTex;
uniform float _Progress;
uniform fixed4 _Color;
uniform float _Smoothness;
uniform float _Opening;
uniform float _Ratio;
uniform float _X;
uniform float _Y;

static float SQRT_2 = 1.414213562373;
static float2 center = float2(0.5,0.5);


fixed4 frag(v2f_img i):COLOR
{
    float x = _Opening == 1 ? _Progress : 1.0-_Progress;
	float2 ratio2 = float2(1.0, 1.0 / _Ratio);
    float m = smoothstep(-_Smoothness, 0.0, SQRT_2 * distance(float2(_X,_Y)* ratio2, i.uv * ratio2) - x*(1.0 + _Smoothness)  );
    return lerp( tex2D(_MainTex, i.uv), _Color, m );
}
ENDCG
} // end Pass
    } // end SubShader

    FallBack "Diffuse"
}