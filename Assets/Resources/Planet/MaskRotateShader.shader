Shader "Custom/MaskRotateShader" 
{
    Properties 
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _PixelRotate ("Mask Rotate", float) = 0
        // 需要一张主贴图, 一张Mask, Mask重复覆盖主贴图的多个部位
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

            // 一共有多少个区域
        	uniform float areaCount;
            // 需要显示哪些区域
        	uniform float areaToShow[20];

            sampler2D _MainTex;
            float _PixelRotate;
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
            	// uv是否在不能显示的区域 
            	if(!ShowTheUV(i.uv))
            		return half4(0,0,0,0);

            	_PixelRotate = radians(_PixelRotate);
            	float4x4 rotateMat = 
            	{
            		cos(_PixelRotate),-sin(_PixelRotate),0,0,
            		sin(_PixelRotate),cos(_PixelRotate),0,0,
            		0,0,1,0,
            		0,0,0,1
            	};
            	float2 uvAfterRotate = mul(rotateMat, i.uv-float2(0.5f,0.5f))+float2(0.5f,0.5f);
            	float4 oColor = tex2D(_MainTex, uvAfterRotate);
        		return oColor;
            }
            bool ShowTheUV(float2 uv)
            {
            	float anglePerArea = 360/areaCount;
            	float2 subUV = uv - float2(0.5,0.5);
        		float2 yAxisVector = float2(0,1);
            	float includeRad = acos(dot(subUV,yAxisVector)/(length(subUV)*length(yAxisVector)));
            	// 获得y轴正方向到subUV的夹角
            	float includeAngle = degrees(includeRad);
            	float finalAngle = includeAngle+step(subUV.x,0)*(360-includeAngle*2);
            	// 获得夹角所在的区域
            	int areaIndex = (int)(finalAngle/anglePerArea);
            	return step(0.5,areaToShow[areaIndex]);
            }
            ENDCG
        }
    }
    FallBack Off
}