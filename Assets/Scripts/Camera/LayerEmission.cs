// 如果要提高发光效果, 请手动到 layerEmission 这个shader中提高迭代次数 = = 谨慎提高

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class LayerEmission : MonoBehaviour
    {  
        public Camera subCamera;
        public Shader layerEmission;
        public int pixelPerUnit = 1;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            RenderTexture emisSource = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

            subCamera.targetTexture = emisSource;
            subCamera.Render();

            Material emisMat = new Material(layerEmission);
            emisMat.SetInt("_PixelPerUnit", pixelPerUnit);
            emisMat.SetTexture("_SourceTex", source);
            Graphics.Blit(emisSource, destination, emisMat);
            //释放申请的RT
            RenderTexture.ReleaseTemporary(emisSource);
        }
    }
