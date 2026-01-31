using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.SceneManagement;
// 定义一个事件，用于传递游戏结果

namespace GGJ2026
{
    public class GameResultController :
    MonoBehaviour, IController
    {
        public GameObject Win_UI;
        public GameObject Lose_UI;
        public IArchitecture GetArchitecture() => GameApp.Interface;
       // public List<GameObject>  Buttons { get; } = new();
        private void OnEnable()
        {
            this.RegisterEvent<ResultChangeEvent>(OnLoadStage1UI).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnLoadStage1UI(ResultChangeEvent e)
        {
            
        }
        
    }

}