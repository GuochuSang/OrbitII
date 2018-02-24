Shader "Custom/PixelRotate" 
{
    Properties 
    {
        _MainTex ("Altitude Texture", 2D) = "white" {}
        _PixelRotate ("Mask Rotate", float) = 0
        _IsFlip ("Flip Tex", int) = 0
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
            float _PixelRotate;
            float4 _MainTex_TexelSize;
            int _IsFlip;
          
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
 
 
            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv.xy = v.texcoord.xy;

                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
            	float pixelSize = _MainTex_TexelSize.x;
            	_PixelRotate = radians(_PixelRotate);
            	float4x4 rotateMat = 
            	{
            		cos(_PixelRotate),-sin(_PixelRotate),0,0,
            		sin(_PixelRotate),cos(_PixelRotate),0,0,
            		0,0,1,0,
            		0,0,0,1
            	};
            	float2 pixelCountf = i.uv/pixelSize.x;
            	int2 pixelCount = int2(round(pixelCountf.x),round(pixelCountf.y));
            	float2 delta = i.uv - pixelCount*pixelSize;
            	i.uv -= delta;
            	float2 uvAfterRotate = mul(rotateMat, i.uv-float2(0.5f,0.5f))+float2(0.5f,0.5f);
            	float4 oColor;
            	if(_IsFlip == 1)
            		oColor = tex2D(_MainTex, float2(1-uvAfterRotate.x,uvAfterRotate.y));
        		else
        			oColor = tex2D(_MainTex, uvAfterRotate);
        		return oColor;
            }
            ENDCG
        }
    }
    FallBack Off
}