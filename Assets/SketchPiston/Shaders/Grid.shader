Shader "Custom/Grid" {
	Properties{
		_Color1("color1", Color) = (0,0,0,1)
		_Color2("color2", Color) = (1,1,1,1)
	}
	CGINCLUDE
		#include "UnityCG.cginc"
		half4 _Color1,_Color2;
		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			half3 normal : TEXCOORD1;
		};
 
		v2f vert (appdata_full v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord;
			o.normal = SCALED_NORMAL;
			return o;
		}
			
		half4 frag (v2f i) : COLOR
		{
			half
				t1 = frac(distance(i.uv, half2(0.5,0.5))*10)<0.5,
				t2 = frac(i.uv.x*10)>0.5,
				t = lerp(t2,t1,i.normal.y);
			return lerp(_Color1,_Color2,t);
		}
	ENDCG
	
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG 
		}
	}
}