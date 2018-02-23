using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universe
{
    public class UniverseChunkObject : MonoBehaviour,IChunkObject
    {
        public const float maxDistance = 196;//(maxPlanetRadius + baseHeight)*2
        public const float maxRayCastDistanceToPlanet = 70f;
        // 不能在地表飞行, 基础高度
        public const float baseHeight = 30f;
        public const float maxSelfRotateSpeed = 10f;
        public const float maxMoveSpeed = 4f;
        public const float maxPlanetRadius = 68f;
        void Start()
        {
            transform.Rotate(Vector3.forward * Random.value * 360);
            StartCoroutine(RotateSelf());
            CheckPlanet();
        }
        void CheckPlanet()
        {
            var hit = Physics2D.CircleCast(this.transform.position, maxRayCastDistanceToPlanet, Vector2.up, 0f,1<<LayerMask.NameToLayer("Planet"));
            if (hit.collider != null && hit.collider.tag.Equals("Planet"))
            {
                Planet pl = hit.collider.gameObject.GetComponent<Planet>();
                StartCoroutine(RotateAround(hit.collider.transform, (pl.Radius+baseHeight)*(1+Random.value)));
            }
        }
        IEnumerator RotateAround(Transform target,float distance)
        {
            // v^2*r = const
            float speed = Mathf.Sqrt((maxMoveSpeed * maxMoveSpeed * maxPlanetRadius)/distance)* (Random.Range(0,2)*2-1);
            // 星球到物体的向量
            Vector3 direction = (transform.position - target.position).normalized*distance;
            while (true)
            {
                direction = Matrix4x4.Rotate(Quaternion.Euler(Vector3.forward * speed * Time.deltaTime)).MultiplyPoint(direction);
                transform.position = target.position + direction;
                yield return null;
            }
        }
        IEnumerator RotateSelf()
        {
            Vector3 rotation = maxSelfRotateSpeed*Vector3.forward*(Random.value-0.5f);
            while (true)
            {
                transform.Rotate(rotation * Time.deltaTime);
                yield return null;
            }
        }

        #region IChunkObject
        public Vector2 GetPosition()
        {
            return new Vector2(transform.position.x, transform.position.y);
        }
        public Manager.ID Save()
        {
            return null;
        }
        public void EnterUpdate()
        {
            // Do nothing because it will destroy when leave view field
        }
        public void ExitUpdate()
        {
            Manager.PoolManager.Instance.ReturnCacheGameObejct(this.gameObject);
        }
        #endregion
    }
}