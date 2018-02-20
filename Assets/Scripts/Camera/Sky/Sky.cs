using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;

public class Sky : MonoBehaviour  
{  
    Transform followCamera;
    Camera mainCamera;// 自动跟踪mainCamera
    float cameraSize;
    Vector3 originPos;

    public Material mat;
    [Header("限制")]
    public float maxDistance;// 可计算出什么距离对应什么移动速度
    [Header("一些系数")]
    public float scaleFactor;// 距离->uvScale
    public float speedFactor;// 基础速度*系数

    [System.Serializable]
    public class SkyLayer
    {
        public Texture2D tex;
        [Header("距离多少光年")]
        public float distance;
        [HideInInspector]
        public float scale;
        [HideInInspector]
        public Vector2 pos;
        [HideInInspector]
        public float uvSpeed;
    }
    [Header("星空(只能有三层)")]
    public List<SkyLayer> skys;

    void Start()
    {
        followCamera = GameObject.Find("Main Camera").transform;
        mainCamera = followCamera.GetComponent<Camera>();
        cameraSize = mainCamera.orthographicSize;
        originPos = followCamera.position;
        foreach (var sky in skys)
        {
            sky.distance = Mathf.Clamp(sky.distance, 0, maxDistance);
            sky.uvSpeed = (maxDistance - sky.distance)/maxDistance;
            sky.pos = Vector2.zero;
        }
    }
    void Move()
    {
        Vector3 deltaPos = followCamera.position - originPos;
        foreach (var sky in skys)
        {
            sky.pos += new Vector2(deltaPos.x, deltaPos.y)*sky.uvSpeed*speedFactor*0.001f;
        }
        originPos = followCamera.position;
    }
    void CheckSize()
    {
        cameraSize = mainCamera.orthographicSize;
        foreach (var sky in skys)
        {
            float currentDistance = sky.distance * cameraSize;
            currentDistance = Mathf.Clamp(currentDistance, 1, int.MaxValue);
            sky.scale = 1 / currentDistance*scaleFactor*10000;
        }
    }
    void Update()
    {
        CheckSize();
        Move();
        mat.SetTexture("_FstTex",skys[0].tex);
        mat.SetTexture("_SecTex",skys[1].tex);
        mat.SetTexture("_TrdTex",skys[2].tex);
        mat.SetVector("_Scale", new Vector4(
            skys[0].scale,
            skys[1].scale,
            skys[2].scale,0));
        mat.SetVector("_Size_Pos1",new Vector4(Screen.width,Screen.height,skys[0].pos.x,skys[0].pos.y));
        mat.SetVector("_Pos2_Pos3",new Vector4(skys[1].pos.x,skys[1].pos.y,skys[2].pos.x,skys[2].pos.y));

    }
}