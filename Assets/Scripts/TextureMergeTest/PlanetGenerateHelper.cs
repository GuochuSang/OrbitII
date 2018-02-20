using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Manager;

public class PlanetGenerateHelper : MonoSingleton<PlanetGenerateHelper>
{
    public Shader areaShader;
    public Shader pixelRotShader;
    public Shader merge;
    public Shader clearAll;


    /// <summary>
    /// 裁剪, 输入土地数量和要显示的土地(float = 1)
    /// </summary>
    public Texture2D CutArea(Texture2D areaTex,int areaCount, float[] landIndexToShow)
    {
        // 临时用的RenderTex
        RenderTexture temp =new RenderTexture(areaTex.width, areaTex.height, 0, RenderTextureFormat.ARGB32);
        Material areaMat = new Material(areaShader);
        areaMat.SetFloat("areaCount", areaCount);
        areaMat.SetFloatArray("areaToShow", landIndexToShow);
        Graphics.Blit(areaTex, temp, areaMat);
        // 暂存
        Texture2D returnTex = ToTex(temp);
        // 释放
        RenderTexture.Destroy(temp);
        return returnTex;
    }
    /// <summary>
    /// 合并, 先显示top. 再显示bottom
    /// </summary>
    public Texture2D Merge(Texture2D top,Texture2D bottom)
    {
        if (top == null)
            return bottom;
        if (bottom == null)
            return top;
        RenderTexture temp = new RenderTexture(top.width, top.height, 0, RenderTextureFormat.ARGB32);
        Material mergeMat = new Material(merge);
        // 两张图片经过shader合并
        mergeMat.SetTexture("_SubTex", bottom);
        Graphics.Blit(top, temp,mergeMat);
        Texture2D returnTex = ToTex(temp);
        RenderTexture.Destroy(temp);
        return returnTex;
    }
    // 像素旋转
    public Texture2D PixelRotate(Texture2D tex, float angle, bool flipX)
    {
        RenderTexture temp = new RenderTexture(tex.width, tex.height, 0, RenderTextureFormat.ARGB32);
        Material rotMat = new Material(pixelRotShader);
        if(flipX)
            rotMat.SetInt("_IsFlip", 1);
        else 
            rotMat.SetInt("_IsFlip", 0);
        rotMat.SetFloat("_PixelRotate", angle);
        Graphics.Blit(tex, temp, rotMat);
        Texture2D returnTex = ToTex(temp);
        RenderTexture.Destroy(temp);
        return returnTex;
    }
    // RenderTex转Tex
    public Texture2D ToTex(RenderTexture temp)
    {
        Texture2D myTexture2D = new Texture2D(temp.width,temp.height);
        RenderTexture tempActive = RenderTexture.active;
        RenderTexture.active = temp;
        myTexture2D.ReadPixels(new Rect(0, 0, temp.width, temp.height), 0, 0);
        myTexture2D.Apply();
        RenderTexture.active = tempActive;
        RenderTexture.active = null;
        return myTexture2D;
    }
}