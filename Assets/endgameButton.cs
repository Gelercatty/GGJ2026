using GGJ2026;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
namespace GGJ2026
{
    public class endgameButton : MonoBehaviour
    {
        public IArchitecture GetArchitecture() => GameApp.Interface;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void sendEndCommend()
        {
            GameApp.Interface.SendCommand<FinnalConfirmCommand>( new FinnalConfirmCommand());
        }
    }
}