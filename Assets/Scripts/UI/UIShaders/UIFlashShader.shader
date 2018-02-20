Shader "Hidden/UIFlashShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex ("Bling Mask", 2D) = "white" {}
		_LightStrength("Light Strength", float) = 0.5
		_UVSpeed("UV Speed",float) = 1
		_LightBuff("Light Buff",float) = 2

	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MaskTex;
			float _UVSpeed;
			float _LightStrength;
			float _LightBuff;
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
				fixed4 colMain = tex2D(_MainTex, i.uv);
				fixed4 colMask = tex2D(_MaskTex, i.uv*0.3f+float2(_UVSpeed*_Time.x,0));
				float len = length(colMain.rgb);
				//colMain.rgb += step(_LightLimit,len)*colMask.a*colMain.rgb*_LightStrength;
				colMain.rgb += colMask.a*colMain.rgb*clamp((len+_LightBuff),0,10)*_LightStrength;
				return colMain;
			}
			ENDCG
		}
	}
}
