using System.Collections;
using System.Collections.Generic;
using ShipProject;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LoadBody : MonoBehaviour
{
    /// <summary>
    ///  贴图路径
    /// </summary>
    [FolderPath(ParentFolder = "Assets/Resources")]
    public string TexturePath;

    /// <summary>
    /// 预制体保存路径
    /// </summary>
    [FolderPath] //(ParentFolder = "Assets/Resources")]
    public string SavePath;

    /// <summary>
    /// 加载贴图为预制体
    /// </summary>
    [Button("LoadBodyPrefab")]
    public void LoadBodyPrefab()
    {
        GameObject Body = new GameObject("Body");
        Sprite[] Textures = Resources.LoadAll<Sprite>(TexturePath);
        GameObject[] obj = new GameObject[11];
        for (int i = 0; i <40; i++)
        {
            for(int j=0;j<11;j++)
            {
                int index = 11 * i + j;
                if (i== 0)
                {
                    obj[j] = PrefabUtility.CreatePrefab(SavePath + "/" + Textures[index].name + ".prefab", Body);
                    Block block = obj[j].AddComponent<Block>();
                    block.Id =j + 1;
                    block.BlockName = Textures[index].name;
                    obj[j].AddComponent<SpriteRenderer>().sprite = Textures[index];
                    BodyBlockStateChange change= obj[j].AddComponent<BodyBlockStateChange>();
                }
                obj[j].GetComponent<BodyBlockStateChange>().StateSprites[i] = Textures[index];
            }
        }

    }
}
