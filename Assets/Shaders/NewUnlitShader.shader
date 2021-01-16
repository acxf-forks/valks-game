// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/NewUnlitShader"
{
	// no Properties block this time!
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// include file that contains UnityObjectToWorldNormal helper function
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				// Read vertex color from mesh.
				fixed4 color : COLOR0;
			};

			struct v2f
			{
				// Pass interpolated vertex color to fragment.
				fixed4 color : COLOR0;
				float4 vertex : SV_POSITION;
			};

			// vertex shader: takes object space normal as input too
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color; // Pass the color through.
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}