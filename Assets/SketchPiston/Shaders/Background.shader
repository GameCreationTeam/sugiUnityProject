Shader "Custom/Background" {
	Properties {
		_Color1 ("color1", Color) = (0.5,0.5,0.5,0.5)
		_Color2 ("color2", Color) = (0.5,0.5,0.5,0.5)
	}
 
	CGINCLUDE
		#include "UnityCG.cginc"
		fixed4 _Color1,_Color2;
		
		struct v2f {
			float4 pos : SV_POSITION;
			float4 sPos : TEXCOORD;
		};
 
		v2f vert (appdata_full v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.sPos = ComputeScreenPos(o.pos);
			return o;
		}
			
		half4 frag (v2f i) : COLOR
		{
			half2 sUV = i.sPos.xy/i.sPos.w;
			half t = frac(sUV.x*10)>0.5;
			
			return lerp(_Color1, _Color2, sUV.y+t);
		}
	ENDCG
	
	SubShader {
		Pass {
			CGPROGRAM
			#pragma glsl
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG 
		}
	}
}