using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.SceneManagement;

namespace GGJ2026
{
    public class Stage_UI_Candidate_controller : MonoBehaviour, IController
    {
        public GameObject Candidate_Button; // prefeb ref
        public IArchitecture GetArchitecture() => GameApp.Interface;
        public List<GameObject>  Buttons { get; } = new();
        private void OnEnable()
        {
            this.RegisterEvent<LoadStage1UI>(OnLoadStage1UI).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnLoadStage1UI(LoadStage1UI e)
        {
            print("init ui");
            var mod = GameApp.Interface.GetModel<UIStage_1_Model>();
            var repo = GameApp.Interface.GetSystem<ICaseRepositorySystem>();
            
            for (int i = mod.CaseIds.Count - 1; i >= 0; i--)
            {
                var id = mod.CaseIds[i];
                var new_button = Instantiate(Candidate_Button, transform);
                var pack = repo.Get(id);
                new_button.GetComponent<Image>().sprite = pack.silhouette;

                Buttons.Add(new_button);
                Debug.Log("addd one");
            }
        }
    }
}