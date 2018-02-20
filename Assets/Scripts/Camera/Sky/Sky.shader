// 用一个testure作底 
// Scale, 大图小图边长之比
Shader "Custom/Sky"
{
	Properties
	{
		_MainTex ("Back Sky", 2D) = "white" {}
		_FstTex ("Far Sky", 2D) = "white" {}
		_SecTex ("Mid Sky",2D)  = "white" {}
		_TrdTex ("Near Sky",2D)  = "white" {}
		_Scale("Sky Layer Scale",Vector) = (1,1,1,1)
		_Size_Pos1("Sky Size",Vector) = (1920,1080,1,1)
		_Pos2_Pos3("Sky Pos",Vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags{"Queue"="Transparent" "RenderType"="Opaque" "IgnoreProject"="True"}

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
			sampler2D _FstTex;
			sampler2D _SecTex;
			sampler2D _TrdTex;
			float4 _Scale;
			float4 _Size_Pos1;
			float4 _Pos2_Pos3;

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
				float2 pos = (i.uv-float2(0.5,0.5))*_Size_Pos1.xy; 

				float2 uv1 = pos/_Scale.x+_Size_Pos1.zw+float2(0.5,0.5);// Scale 越大, 可见区域变少
				float4 color1 = tex2D(_FstTex,uv1);

				float2 uv2 = pos/_Scale.y+_Pos2_Pos3.xy+float2(0.5,0.5);
				float4 color2 = tex2D(_SecTex,uv2);

				float2 uv3 = pos/_Scale.z+_Pos2_Pos3.zw+float2(0.5,0.5);

				float4 color3 = tex2D(_TrdTex,uv3);

				float4 oColor = color3 + (1-color3.a)*color2 + clamp((1-color3.a-color2.a),0,1)*color1;
				return oColor;
			}
			ENDCG
		}
	}
}
