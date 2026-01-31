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
            // 监听阶段变化：只要不在 Stage1，就清理按钮
            var game = GameApp.Interface.GetModel<GameStateModel>();
            game.Phase.RegisterWithInitValue(phase =>
            {
                if (phase != GamePhase.Stage1)
                {
                    ClearButtons();
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }


        private void OnLoadStage1UI(LoadStage1UI e)
        {
            Debug.Log("[Stage_UI] init ui");

            // ✅ 双保险：生成前先清掉旧的
            ClearButtons();

            var mod = GameApp.Interface.GetModel<UIStage_1_Model>();
            var repo = GameApp.Interface.GetSystem<ICaseRepositorySystem>();

            for (int i = 0; i < mod.CaseIds.Count; i++)
            {
                var id = mod.CaseIds[i];

                var newButton = Instantiate(Candidate_Button, transform);

                var pack = repo.Get(id);
                newButton.GetComponent<Image>().sprite = pack.silhouette;

                var prop = newButton.GetComponent<Stage1DropButtonFProperty>();
                if (prop == null) prop = newButton.AddComponent<Stage1DropButtonFProperty>();
                prop.CaseId = id;

                Buttons.Add(newButton);
                Debug.Log("[Stage_UI] add one, CaseId: " + prop.CaseId);
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

            Debug.Log("[Stage_UI] cleared candidate buttons");
        }
    }
}
