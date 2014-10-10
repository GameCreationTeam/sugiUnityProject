Shader "Custom/Color" {
	Properties {
		_Color ("color", Color) = (0.5,0.5,0.5,0.5)
	}
 
	CGINCLUDE
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		fixed4 _Color;
		
		struct v2f {
			float4 pos : SV_POSITION;
		};
 
		v2f vert (appdata_full v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			return o;
		}
			
		half4 frag (v2f i) : COLOR
		{
			return _Color;
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