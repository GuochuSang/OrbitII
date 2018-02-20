﻿/// <summary>
/// 使用ID记录SaveData
/// 继承SaveData作为每个类的数据存储类
/// 每个类继承ISaveable处理Save/Load
/// </summary>
 
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Manager
{

/// <summary>
/// Save data类.
/// 其他类如果需要存储功能, 都需要包含这样一个类, 用于读写数据
/// all child classes should also use the [System.Serializable] attribute
/// </summary>
[System.Serializable]
public abstract class SaveData 
{ 
}
/// <summary>
/// 这个 CreatableSaveData 可以创建对应的对象并返回新建的对象
/// </summary>
[System.Serializable]
public abstract class CreatableSaveData : SaveData
{
    /// <summary>
    /// 通过SaveData创建对象, 一定记得Init(id)
    /// </summary>
    public abstract T Create<T>(ID id);
}
    

    public class SaveManager : MonoSingleton<SaveManager> 
    {
        #region 路径字符串定义
        /// <summary>
        /// 保存所有存档的文件夹
        /// </summary>
        public const string SAVES_FOLDER = "saves";//       .\saves
        /// <summary>
        /// 记录单个存档的存档信息的dat文件名
        /// </summary>
        public const string RECORD_FILE = "record";//      .\saves\recordname\record.dat
        public const string SAVEDATA_FILE = "save_data";// .\saves\recordname\save_data.dat    
        /// <summary>
        /// 默认存档文件后缀
        /// </summary>
        public const string FILE_EXTENSION = ".dat";
        public string GAME_DATA; // 启动SaveManager, 在Start内生成 : Application.dataPath + "/Resources/GameData"
       #endregion
        void Awake()
        {
            GAME_DATA = Application.dataPath + "/Resources/GameData";
        }

        //管理所有SaveData
        public Dictionary<ID, SaveData> saveData = new Dictionary<ID, SaveData>();
        //是否能修改硬盘文件(在存档更改之后才可修改)
        public bool modified;

        [SerializeField]
        private Record currentRecord = null;//当前打开的 Record ! 
        public Record CurrentRecord
        {
            get { return currentRecord; }
            private set{ }
        }

        #region 按位置保存文件|从位置加载文件|删除目录|获得文件后缀路径|获得文件夹路径|
        /// <summary>
        /// 将某文件(.dat)存放在某位置
        /// </summary>
        public void SaveAtPath<T>(T saveData, string path, string extension = FILE_EXTENSION)
        {
            Debug.Log("Save At : " + path + extension);
            SaveUtility.Save(saveData, GetFolderPath(path+extension));
        }
        /// <summary>
        /// 从某位置读取某文件
        /// </summary>
        public T LoadAtPath<T>(string path, string extension = FILE_EXTENSION)
        {
            return SaveUtility.Load<T>(GetFolderPath(path+extension));
        }
        /// <summary>
        /// 删除整个目录
        /// </summary>
        public void DeleteDirectory(string folderPath) 
        {
            //Debug.Log("Delete At " + folderPath);
            Directory.Delete(GetFolderPath(folderPath),true);
        }
        /// <summary>
        /// 通过文件夹路径生成硬盘中的总路径, 不含后缀
        /// 如果没有这个路径, 当然要创建一个啊
        /// </summary>
        public string GetFolderPath(string folderPath) 
        {
            return GAME_DATA + "/" + folderPath;
        }
        #endregion

        #region 管理当前打开的存档
        /// <summary>
        /// 使用某个存档
        /// </summary>
        public void EnterRecord(Record tRecord)
        {
            Debug.Log("Enter Record " + tRecord.name);
            currentRecord = tRecord;
            LoadCurrentRecord();
            EventManager.Instance.PostEvent(GameEvent.ENTER_RECORD,this);
        }
        /// <summary>
        /// 使用某个存档
        /// </summary>
        public void EnterRecord(string recordName)
        {
            Record rec = Record.GetRecord(recordName);
            if (rec != null)
                EnterRecord(rec);
            else
                Debug.Log("Can not find " +recordName);
        }
        /// <summary>
        /// 关闭存档( 当不再进行存档内游戏时 )
        /// </summary>
        public void ExitRecord(bool saveRecord)
        {
            if (saveRecord)
            {
                EventManager.Instance.PostEvent(GameEvent.EXIT_RECORD_WITH_SAVE, this);
                SaveCurrentRecord();
            }
            Debug.Log("Exit Record " + currentRecord.name);
            saveData = new Dictionary<ID, SaveData>();
            currentRecord = null;
        }

        /// <summary>
        /// 保存当前存档(到硬盘)
        /// </summary>
        public void SaveCurrentRecord() 
        {
            if (currentRecord == null)
            {
                Debug.Log("Can't Save Record : You Have Not Enter Any Record! ");
                return;
            }
            Debug.Assert(currentRecord.folderName != null);
            if (!modified) 
                Debug.Log("You Have Not Save Any Changes! But Still Save");
            SaveAtPath(ToSaveData(), SAVES_FOLDER + "/" + currentRecord.folderName + "/" + SAVEDATA_FILE);
            currentRecord.SaveInfo();
            modified = false;
        }
        // load the SaveManager from disk(硬盘)
        private void LoadCurrentRecord() 
        {
            if (currentRecord == null)
            {
                Debug.Log("Can't Load Record : You Have Not Enter Any Record! ");
                return;
            }
            Debug.Assert(currentRecord.folderName != null);
            FromSaveData(LoadAtPath<SaveManagerData>(SAVES_FOLDER + "/" + currentRecord.folderName  + "/" + SAVEDATA_FILE));
        }

        // convert this to a SaveManagerData (to be used with SaveUtility)
        private SaveManagerData ToSaveData() 
        {
            return new SaveManagerData(this);
        }
        // set the values of this from the given SaveManagerData (which should come from SaveUtility)
        private void FromSaveData(SaveManagerData data) 
        {
            if (data == null)
            {
                Debug.Log("this is an empty record");
                return;
            }
            saveData = data.saveData;
        }
        #endregion

        #region 在打开一个存档之后,通过ID : Save|Load|GetSaveData|RemoveIDWithData
        /// <summary>
        /// 对应ID保存ISaveable对象
        /// </summary>
        public void Save(ISaveable saveable, ID id) 
        {
            saveData[id] = saveable.toSaveData();
            modified = true;
        }

        /// <summary>
        /// 通过ID加载已存在的ISaveable对象
        /// </summary>
        public void Load(ISaveable saveable, ID id) 
        {
            if (!saveData.ContainsKey(id)) 
            {
                Debug.Log("没有储存过这个ID" + id.ToString());
                return;
            }
            saveable.fromSaveData(saveData[id]);
        }
        public SaveData GetSaveData(ID id) 
        {
            if (!saveData.ContainsKey(id)) 
            {
                return null;
            }
            return saveData[id];
        }
            
        public bool RemoveIDWithData(ID id)
        {
            if (saveData.Remove(id))
            {
                id.OnDestroy();
                return true;
            }
            return false;
        }
        #endregion

    }

    [System.Serializable]
    public class SaveManagerData 
    {
        public Dictionary<ID, SaveData> saveData;

        public SaveManagerData(SaveManager obj) 
        {
            saveData = obj.saveData;
        }
    }
}