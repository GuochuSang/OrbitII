/// <summary>
/// Game manager.
/// 每个游戏有不同的GameManager
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Universe;

namespace Manager
{
    public class GameManager : MonoSingleton<GameManager> 
    {   
        public string mainMenuScene = "MainMenu";
        public string gameSceneScene = "GameScene";


        void Start()
        {
            SceneManager.LoadScene(mainMenuScene);
            EventManager.Instance.AddListener(this,GameEvent.ENTER_RECORD,OnEnterRecord);
            EventManager.Instance.AddListener(this,GameEvent.EXIT_RECORD_WITH_SAVE,OnExitRecord);
            Resource.InitElements();
        }
        void OnEnterRecord(GameEvent gameEvent,Component comp,object param = null)
        {
            Debug.Log("Seed : "+SaveManager.Instance.CurrentRecord.seed);
            Seed.InitSeed(SaveManager.Instance.CurrentRecord.seed);
            SceneManager.LoadScene(gameSceneScene);
        }
        void OnExitRecord(GameEvent gameEvent,Component comp,object param = null)
        {
            ChunkManager.Instance.Save();
            SceneManager.LoadScene(mainMenuScene);
            ID.ids = new HashSet<ID>();
        }
    }
}
