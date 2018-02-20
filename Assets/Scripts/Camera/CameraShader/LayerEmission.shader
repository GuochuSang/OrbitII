Shader "Custom/LayerEmission" 
{
    Properties 
    {
        _MainTex ("UnEmissioned Texture", 2D) = "white" {}
        _SourceTex ("Source Texture", 2D) = "white" {}
        _PixelPerUnit("Light Area",int) = 3
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
        	sampler2D _SourceTex;
        	int _PixelPerUnit;

          	float4 _MainTex_TexelSize;

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
            	int _Iter = 5;
            	float4 unemisColor = tex2D(_MainTex,i.uv);
				float4 sourceColor = tex2D(_SourceTex,i.uv);

            	float4 finalColor = float4(0,0,0,0);

            	// 找到附近的颜色
				for(int k=-_Iter;k<=_Iter;k++)
				{
					for(int j=-_Iter;j<=_Iter;j++)
					{
						finalColor += tex2D(_MainTex, i.uv+float2(k*_MainTex_TexelSize.x*_PixelPerUnit,j*_MainTex_TexelSize.x*_PixelPerUnit));
					}
				}
				finalColor /= 4*(_Iter+1)*(_Iter+1);
				return unemisColor + (1-unemisColor.a)*finalColor + clamp((1-unemisColor.a-finalColor.a),0,1)*sourceColor;
            }

            ENDCG
        }
    }
    FallBack Off
}