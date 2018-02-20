Shader "Custom/Cloud" 
{
    Properties 
    {
        _MainTex ("Mask Texture", 2D) = "white" {}
        _NoiseTex1 ("Noise Texture", 2D) = "white" {}
        _NoiseTex2 ("Noise Texture 2", 2D) = "white" {}
    }
    SubShader{
        Tags{"Queue"="Transparent" "RenderType"="Opaque" "IgnoreProject"="True"}
        Pass{
            Tags{"LightMode"="ForwardBase"}
 
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
 
            CGPROGRAM
			#pragma enable_d3d11_debug_symbols 
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
          	sampler2D _NoiseTex1;
          	sampler2D _NoiseTex2;

            struct a2v 
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
 
 			bool ShowTheUV(float2 uv);

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
            	float speed = 0.3f;
            	float sinTime = sin(_Time.x*speed);
            	float cosTime = cos(_Time.x*speed);
            	float4x4 rotateMat1 = 
            	{
            		cosTime,-sinTime,0,0,
            		sinTime,cosTime,0,0,
            		0,0,1,0,
            		0,0,0,1
            	};
            	float4x4 rotateMat2 = 
            	{
            		cosTime,sinTime,0,0,
            		-sinTime,cosTime,0,0,
            		0,0,1,0,
            		0,0,0,1
            	};
            	float2 rotateUV1 = mul(rotateMat1,(i.uv-float2(0.5,0.5)))+float2(0.5,0.5);
            	float2 rotateUV2 = mul(rotateMat2,(i.uv-float2(0.5,0.5)))+float2(0.5,0.5);

            	float4 noiseColor1 = tex2D(_NoiseTex1,rotateUV1);
            	float4 noiseColor2 = tex2D(_NoiseTex2,rotateUV2);

            	float4 maskColor = tex2D(_MainTex, i.uv);

				maskColor.a *= noiseColor1.r*noiseColor2.r;
				maskColor.a = clamp((floor(maskColor.a/0.1f))*0.3f,0.0f,1.0f);

				// 加上星球发光
				float distanceToBoarder = clamp(0.5 - distance(i.uv,float2(0.5f,0.5f)),0,0.5);// [0f, 0.5f]
				float planetLight = distanceToBoarder*1.0f;// [0f, 1f]
				maskColor.rgb = float3(1,1,1);
				float4 oColor = maskColor + float4(1,1,1,1)*planetLight;

        		return oColor;
            }

            ENDCG
        }
    }
    FallBack Off
}