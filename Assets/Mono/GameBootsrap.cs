using System;
using System.Text;
using UnityEngine;
using QFramework;

namespace GGJ2026
{
    public class GameBootstrap : MonoBehaviour
    {

        private void Awake()
        {
            GameApp.Interface.RegisterEvent<SourceLoadedEvent>(e =>
            {
                GameApp.Interface.SendCommand(new StartNewGameCommand());
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        private void Start()
        {
            GameApp.Interface.SendCommand(new LoadCaseDataBaseCommand());
            
        }
    }
}
