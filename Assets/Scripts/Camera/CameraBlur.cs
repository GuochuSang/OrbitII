using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;


    public class CameraBlur : MonoBehaviour   
    {  
        public Shader cameraShader;  
        private Material cameraMat = null;  

        int DownSampleNum = 2;  
        [SerializeField]
        float BlurSpreadSize = 0.0f;  
        int BlurIterations = 3;  

        float blurStartSpeed = 17f;
        float blurStopSpeed = 30f;
        float maxSpreadSize = 2f;
       
        void Start ()   
        {  
            BlurSpreadSize = 0.0f;  
            cameraMat = new Material(cameraShader);  
        } 
        /// <summary>
        /// Strength Range(0, 5)
        /// </summary>
        public void StartBlur(float blurStrength)
        {
            maxSpreadSize = Mathf.Clamp(blurStrength, 0f, 5f);
            StopCoroutine("StopBlurInSeconds");
            StartCoroutine(StartBlurInSeconds());
        }
        public void StopBlur()
        {
            StopCoroutine("StartBlurInSeconds");
            StartCoroutine(StopBlurInSeconds());
        }
        IEnumerator StartBlurInSeconds()
        {
            DownSampleNum = 2;
            BlurSpreadSize = 0f;
            BlurIterations = 2;
            while (BlurSpreadSize < maxSpreadSize)
            {
                BlurSpreadSize += blurStartSpeed * Time.deltaTime;
                yield return null;
            }
        }
        IEnumerator StopBlurInSeconds()
        {
            while (BlurSpreadSize >= 0f)
            {
                BlurSpreadSize -= blurStopSpeed * Time.deltaTime;
                if (BlurSpreadSize <= 0f)
                    break;
                yield return null;
            }
        }
        void OnRenderImage(RenderTexture sourceTexture, RenderTexture destination)   
        {  
            if (BlurSpreadSize<=0f)
            {
                Graphics.Blit(sourceTexture, destination);  
                return;
            }
            //【0】参数准备  
            //根据向下采样的次数确定宽度系数。用于控制降采样后相邻像素的间隔  
            float widthMod = 1.0f / (1.0f * (1 << DownSampleNum));  
            //Shader的降采样参数赋值  
            cameraMat.SetFloat("_DownSampleValue", BlurSpreadSize * widthMod);  
            //设置渲染模式：双线性  
            sourceTexture.filterMode = FilterMode.Bilinear;  
            //通过右移，准备长、宽参数值  
            int renderWidth = sourceTexture.width >> DownSampleNum;  
            int renderHeight = sourceTexture.height >> DownSampleNum;  

            // 【1】处理Shader的通道0，用于降采样 ||Pass 0,for down sample  
            //准备一个缓存renderBuffer，用于准备存放最终数据  
            RenderTexture renderBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);  
            //设置渲染模式：双线性  
            renderBuffer.filterMode = FilterMode.Bilinear;  
            //拷贝sourceTexture中的渲染数据到renderBuffer,并仅绘制指定的pass0的纹理数据  
            Graphics.Blit(sourceTexture, renderBuffer, cameraMat, 0);  

            //【2】根据BlurIterations（迭代次数），来进行指定次数的迭代操作  
            for (int i = 0; i < BlurIterations; i++)  
            {  
                //【2.1】Shader参数赋值  
                //迭代偏移量参数  
                float iterationOffs = (i * 1.0f);  
                //Shader的降采样参数赋值  
                cameraMat.SetFloat("_DownSampleValue", BlurSpreadSize * widthMod + iterationOffs);  

                // 【2.2】处理Shader的通道1，垂直方向模糊处理 || Pass1,for vertical blur  
                // 定义一个临时渲染的缓存tempBuffer  
                RenderTexture tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);  
                // 拷贝renderBuffer中的渲染数据到tempBuffer,并仅绘制指定的pass1的纹理数据  
                Graphics.Blit(renderBuffer, tempBuffer, cameraMat, 1);  
                //  清空renderBuffer  
                RenderTexture.ReleaseTemporary(renderBuffer);  
                // 将tempBuffer赋给renderBuffer，此时renderBuffer里面pass0和pass1的数据已经准备好  
                renderBuffer = tempBuffer;  

                // 【2.3】处理Shader的通道2，竖直方向模糊处理 || Pass2,for horizontal blur  
                // 获取临时渲染纹理  
                tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);  
                // 拷贝renderBuffer中的渲染数据到tempBuffer,并仅绘制指定的pass2的纹理数据  
                Graphics.Blit(renderBuffer, tempBuffer, cameraMat, 2);  

                //【2.4】得到pass0、pass1和pass2的数据都已经准备好的renderBuffer  
                // 再次清空renderBuffer  
                RenderTexture.ReleaseTemporary(renderBuffer);  
                // 再次将tempBuffer赋给renderBuffer，此时renderBuffer里面pass0、pass1和pass2的数据都已经准备好  
                renderBuffer = tempBuffer;  
            }  

            //拷贝最终的renderBuffer到目标纹理，并绘制所有通道的纹理到屏幕  
            Graphics.Blit(renderBuffer, destination);  
            //清空renderBuffer  
            RenderTexture.ReleaseTemporary(renderBuffer);  
        }
    }
