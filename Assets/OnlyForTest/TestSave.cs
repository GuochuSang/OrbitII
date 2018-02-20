using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSave : MonoBehaviour {

    public void Save()
    {
        Manager.SaveManager.Instance.ExitRecord(true);
    }
}
