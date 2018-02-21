using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Universe 
{
public class BuildingControlCenterHUD : MonoBehaviour 
{
    public float prepareTime = 2f;
    [Header("按住此键殖民星球(空投控制中心)")]
    public KeyCode key;

    public float lightLimit = 1f;
    public float lightStrength = 1f;

    [SerializeField]
    bool isPressing = false;
    [SerializeField]
    float totalTime = 0f;



    public Transform parentTrans;
    public Vector3 deltaPos;
    Material mat;
    RectTransform rect;
    void Start()
    {
        transform.parent.parent.localPosition = Vector3.zero;
        transform.parent.parent.localRotation = Quaternion.identity;
        transform.parent.parent.localScale = Vector3.one;
        mat = GetComponent<Image>().material;
        rect = GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(parentTrans.position);
        rect.anchoredPosition = screenPos + new Vector2(deltaPos.x, deltaPos.y);

        if(PlanetUI.Instance.watchState != PlanetUI.WatchState.LOOK)
        {
            DestroyTheHUD();
            return;
        }
        if (!isPressing && Input.GetKey(key))
        {
            StartCoroutine(CheckBuild());
        }
    }
    IEnumerator CheckBuild()
    {
        isPressing = true;
        float startTime = Time.time;
        while (Input.GetKey(key) && isPressing)
        {
            totalTime = Time.time - startTime;

            SetCompleteRatio(totalTime/prepareTime);
                
            if (Time.time - startTime > prepareTime)
            {
                Debug.Log("Drop Controll Center");
                PlanetUI.Instance.DropControllCenter(parentTrans.position);
                DestroyTheHUD();
                break;
            }
            yield return null;
        }
        SetCompleteRatio(0f);
        isPressing = false;
    }
    void SetCompleteRatio(float ratio)
    {
        mat.SetFloat("_LightLimit", lightLimit);
        mat.SetFloat("_LightStrength", lightStrength);
        mat.SetFloat("_CompleteRatio", ratio);
    }
    void DestroyTheHUD()
    {
        Destroy(parentTrans.gameObject);
    }

}
}