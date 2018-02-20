Shader "Hidden/Merge"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SubTex ("Texture to Merge", 2D) = "white" {}
	}
	SubShader
	{
	    ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _SubTex;	

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				float4 col2 = tex2D(_SubTex, i.uv);
				col = col*col.a + (1-col.a)*col2;
				return col;
			}
			ENDCG
		}
	}
}
