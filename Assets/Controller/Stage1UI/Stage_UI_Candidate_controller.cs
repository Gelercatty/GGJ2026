using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;

namespace GGJ2026
{
    public class Stage_UI_Candidate_controller : MonoBehaviour, IController
    {
        public GameObject Candidate_Button; // prefeb ref
        public IArchitecture GetArchitecture() => GameApp.Interface;
        public List<Button>  Buttons { get; } = new();
        private void OnEnable()
        {
            this.RegisterEvent<LoadStage1UI>(OnLoadStage1UI).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnLoadStage1UI(LoadStage1UI e)
        {
            var mod = GameApp.Interface.GetModel<UIStage_1_Model>();
            for (int i = mod.CaseIds.Count - 1; i >= 0; i--)
            {
                var id = mod.CaseIds[i];
                var new_button = Instantiate(Candidate_Button, transform);
                // Candidate_Button.GetComponent<Image>().sprite = c
            }
        }
    }
}