Shader "Hidden/BuildControlCenterHUD"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LightLimit("Light Limit", Range(0,1)) = 1 // 亮度阈值
		_LightStrength("Light Strength", Range(0,1)) = 1 // 强度
		_CompleteRatio("Complete Ratio",Range(0,1)) = 0 // 圆环完成度!
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _LightStrength;
			float _LightLimit;
			float _CompleteRatio;

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
				fixed4 col = tex2D(_MainTex, i.uv);

				float2 uvcenter = i.uv - float2(0.5,0.5);
				// 防止除数为0  , 结果 [0, PI]
				float angle = acos(dot(uvcenter,float2(0,1))/(0.0001f+length(uvcenter)));
				if(uvcenter.x < 0)
					angle = 2*3.1415f - angle;

				float strength = step(_LightLimit,col.a)* 	// 亮度大于阈值
								_LightStrength*				// 亮度增强
								step(angle,_CompleteRatio*2*3.1415f); // 角度小于完成度



				return col*strength;
			}
			ENDCG
		}
	}
}
