using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
namespace Universe 
{
    public class CameraController : MonoSingleton<CameraController>
    {
        [Header("正常飞行时的状态参数")]
        float defaultFreeSize = 50f;
        public float DefaultFreeSize
        {
            get { return defaultFreeSize; }
            set 
            { 
                defaultFreeSize = Mathf.Clamp(value, cameraSizeLimit.x, cameraSizeLimit.y);
            }
        }

        public float positionMoveSpeed = 10f;
        [Range(0f,1f)]
        public float positionLerpRatio = 0.2f;
        [Range(0f,1f)]
        public float rotationLerpRatio = 0.2f;
        [Range(0f,1f)]
        public float sizeLerpRatio = 0.2f;
        [Header("相机缩放限制")]
        public Vector2 cameraSizeLimit = new Vector2(10,300);


        [Header("需要移动的Camera(若添加父节点,不必添加子节点)")]
        public List<Camera> translateCameras;
        [Header("需要旋转的Camera(若添加父节点,不必添加子节点)")]
        public List<Camera> rotateCameras;
        [Header("需要改变CameraSize的Camera")]
        public List<Camera> sizeCameras;
    	
        public Transform objectTarget = null; // 跟随绑定的物体
        public Vector3 posTarget = Vector3.zero;
        public Vector3 rotTarget = Vector3.zero;
        public float sizeTarget = 0;

        void Start()
        {
            Debug.Assert(translateCameras != null && translateCameras.Count != 0);
        }

        public void LookAt(Transform obj)
        {
            objectTarget = obj;
            // 2D游戏z坐标不会取值到10000,用于停止协程
            posTarget = new Vector3(0,0,10000);
            //StartCoroutine(LookAtTarget());
        }
        void LateUpdate()
        {
            if (objectTarget != null)
                LookAtTarget();
        }
        void LookAtTarget()
        {
            Vector3 camPos = translateCameras[0].transform.position;
            //在长期观看一个物体时, 最好不用 Lerp, 否则Camera的移动会出现时快时慢的现象
            //camPos = Vector3.Lerp(camPos, objectTarget.position, positionLerpRatio); 
            camPos = Vector3.MoveTowards(camPos, objectTarget.position, positionMoveSpeed*Time.deltaTime);
            foreach (var cam in translateCameras)
            {
                cam.transform.position = new Vector3(camPos.x, camPos.y, -10);
            }
        }
        /*
        IEnumerator LookAtTarget()
        {
            while (objectTarget != null)
            {
                Vector3 camPos = translateCameras[0].transform.position;
                camPos = Vector3.Lerp(camPos, objectTarget.position, positionLerpRatio);
                foreach (var cam in translateCameras)
                {
                    cam.transform.position = new Vector3(camPos.x, camPos.y, -10);
                }
                    yield return new WaitForFixedUpdate();
            }
        }
        */


        public void SetSize(float size)
        {
            size = Mathf.Clamp(size, cameraSizeLimit.x, cameraSizeLimit.y);
            sizeTarget = size;
            StartCoroutine(LerpSize(size));
        }
        public void SetDeltaSize(float deltaSize)
        {
            float size = sizeCameras[0].orthographicSize + deltaSize;
            size = Mathf.Clamp(size, cameraSizeLimit.x, cameraSizeLimit.y);
            foreach (var cam in sizeCameras)
            {
                cam.orthographicSize = size;
            }
        }
        public IEnumerator LerpSize(float sizeTarget)
        {
            while (sizeTarget.Equals(this.sizeTarget))
            {
                float camSize = sizeCameras[0].orthographicSize;
                camSize = Mathf.Lerp(camSize, sizeTarget, sizeLerpRatio);
                foreach (var cam in sizeCameras)
                {
                    cam.orthographicSize = camSize;
                }
                if (Mathf.Approximately(sizeTarget,camSize))
                    break;
                yield return null;
            }
        }

        /// <summary>
        /// 将取消当前追踪的目标
        /// </summary>
        public void SetPosition(Vector3 pos)
        {
            objectTarget = null;
            posTarget = pos;
            StartCoroutine(LerpPosition(pos));
        }
        /// <summary>
        /// 将取消当前追踪的目标
        /// </summary>
        public void Translate(Vector3 translation)
        {
            objectTarget = null;
            foreach (var cam in translateCameras)
            {
                cam.transform.position += new Vector3(translation.x,translation.y,0);
            }
        }
        IEnumerator LerpPosition(Vector3 posTarget)
        {
            while (posTarget.Equals(this.posTarget))
            {
                Vector3 camPos = translateCameras[0].transform.position;
                camPos = Vector3.Lerp(camPos, posTarget, positionLerpRatio);
                foreach (var cam in translateCameras)
                {
                    cam.transform.position = new Vector3(camPos.x,camPos.y,-10);
                }
                if (Vector3.Distance(posTarget,camPos) < 0.001f)
                    break;
                yield return null;
            }
        }
            
            
        public void SetRotation(Vector3 euler)
        {
            rotTarget = euler;
            StartCoroutine(LerpRotation(euler));
        }
        public void SetDeltaRotation(Vector3 deltaEuler)
        {
            foreach (var cam in rotateCameras)
            {
                cam.transform.Rotate(deltaEuler);
            }
        }
        public IEnumerator LerpRotation(Vector3 rotTarget)
        {
            while (rotTarget.Equals(this.rotTarget))
            {
                Vector3 camRot = translateCameras[0].transform.rotation.eulerAngles;
                camRot = Quaternion.Lerp(Quaternion.Euler(camRot),Quaternion.Euler(rotTarget), rotationLerpRatio).eulerAngles;
                foreach (var cam in rotateCameras)
                {
                    cam.transform.rotation = Quaternion.Euler(new Vector3(0,0,camRot.z));
                }
                float deltaAngle = Mathf.Abs(camRot.z - rotTarget.z) / 360f;
                if (Mathf.Abs(deltaAngle - Mathf.Round(deltaAngle)) < 0.001f)
                    break;
                yield return null;
            }
        }
     }
}