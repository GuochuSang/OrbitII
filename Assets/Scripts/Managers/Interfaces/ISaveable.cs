using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Manager
{
public interface ISaveable 
{
    SaveData toSaveData();
    void fromSaveData(SaveData saveData);
}
}