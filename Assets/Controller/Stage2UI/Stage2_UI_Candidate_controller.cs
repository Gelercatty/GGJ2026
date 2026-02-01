using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026
{
    public class Stage2_UI_Candidate_controller : MonoBehaviour, IController
    {
        public GameObject Candidate_Button; // prefab ref
        public IArchitecture GetArchitecture() => GameApp.Interface;

        public List<GameObject> Buttons { get; } = new();

        private void Awake()
        {
            // 监听阶段变化：只要不在 Stage2，就清理按钮
            var game = GameApp.Interface.GetModel<GameStateModel>();
            game.Phase.RegisterWithInitValue(phase =>
            {
                if (phase != GamePhase.Stage2)
                {
                    ClearButtons();
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

   
            this.RegisterEvent<SetEveryThingSil>(e => {
                foreach (var button in Buttons) { 
                    button.GetComponent<UISilhouetteToggle>().SetSilhouette(true);
                    Debug.Log("try to set sil");
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
           
        }

        private void OnEnable()
        {
            this.RegisterEvent<LoadStage2UI>(OnLoadStage2UI).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnLoadStage2UI(LoadStage2UI e)
        {
            Debug.Log("[Stage2_UI] init ui");

            ClearButtons();

            var repo = GameApp.Interface.GetSystem<ICaseRepositorySystem>();
            string id = GameApp.Interface.GetModel<GameStateModel>().CurrentCaseId.Value;
            CasePackSO pack = repo.Get(id);
            for (int i = 0; i < 3; i++)
            {

                var newButton = Instantiate(Candidate_Button, transform);

                newButton.GetComponent<Image>().sprite = pack.prisonShots[i];
                var toggle = newButton.GetComponent<UISilhouetteToggle>();
                toggle.silhouette = true;
                newButton.AddComponent<Stage2DropButtonFProperty>().idx = i;
                Buttons.Add(newButton);
            }
        }

        private void ClearButtons()
        {
            if (Buttons.Count == 0) return;

            for (int i = Buttons.Count - 1; i >= 0; i--)
            {
                if (Buttons[i] != null)
                    Destroy(Buttons[i]);
            }
            Buttons.Clear();

            Debug.Log("[Stage2_UI] cleared candidate buttons");
        }
    }
}
