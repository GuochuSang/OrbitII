using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSave : MonoBehaviour {

    public void Save()
    {
        Manager.EventManager.Instance.PostEvent(Manager.GameEvent.SAVE_GAME, this);
        Manager.SaveManager.Instance.ExitRecord(true);
    }
}
