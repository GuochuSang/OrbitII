using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using S.Serialize;

public class Test : MonoBehaviour
{
    public class Info
    {
        public string name;
        public float value;
        public List<string> nameList;
    }

    Info info = new Info();

    void Start ()
    {
        info.name = "test";
        info.value = 123.456f;

        info.nameList = new List<string>();
        info.nameList.Add("1");
        info.nameList.Add("22");
        info.nameList.Add("333");

        byte[] bytes = SerializeHelp.WriteObjectData(info);
        SerializeHelp.WriteFile(Application.dataPath+ "/SSuite/Examples/test.info", bytes);
        Debug.Log("info bytes.Length :"+bytes.Length + " bytes");

        byte[] readBytes = SerializeHelp.ReadFile(Application.dataPath + "/SSuite/Examples/test.info");
        Info readInfo= SerializeHelp.ReadObjectData<Info>(readBytes);

        Debug.Log(readInfo.name);
        Debug.Log(readInfo.value);
        foreach (string str in readInfo.nameList)   { Debug.Log(str); }

        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic["monster"] = "Slime";
        dic["wepon"] = "Gun";
        dic["state"] = "living";

        bytes = SerializeHelp.WriteObjectData(dic);
        SerializeHelp.WriteFile(Application.dataPath + "/SSuite/Examples/dic.info", bytes);

        readBytes = SerializeHelp.ReadFile(Application.dataPath + "/SSuite/Examples/dic.info");
        Dictionary<string, string> readDic = SerializeHelp.ReadObjectData<Dictionary<string, string>>(readBytes);

        Debug.Log(readDic["monster"]);
        Debug.Log(readDic["wepon"]);
        Debug.Log(readDic["state"]);
    }

}
