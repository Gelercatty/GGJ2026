using UnityEngine;
using QFramework;
using System.Collections.Generic;
using System;

namespace GGJ2026
{
    class Stage2UIManager : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture() => GameApp.Interface;
        public Stage2ViewPort CandidateArea;
        void Awake(){
            var UIModel = GameApp.Interface.GetModel<UIStage_2_Model>();
            UIModel.Selectedidx.Register(idx =>
            {
                reFreshModel(idx);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        void reFreshModel(int idx){
            // TODO: 刷新model显示的内容。
            string CaseId = GameApp.Interface.GetModel<GameStateModel>().CurrentCaseId.Value;
            CasePackSO pack = GameApp.Interface.GetSystem<ICaseRepositorySystem>().Get(CaseId);
            var UIModel = GameApp.Interface.GetModel<UIStage_2_Model>();
            List<DialogueEntry> options = pack.stage2Dialogues[idx].entries;

            //List<DialogueEntry> dialogET_0 = digsheet[0].entries; // 第一个人的三个选项
            //List<DialogueEntry> dialogET_1 = digsheet[1].entries; // 第二个人的三个选项
            //List<DialogueEntry> dialogET_2 = digsheet[2].entries; // 第三个人的三个选项

            UIModel.ButtonText_0 = options[0].prompt;
            UIModel.ButtonText_1 = options[1].prompt;
            UIModel.ButtonText_2 = options[2].prompt;

            string name = pack.name;
            UIModel.Dialogue_0 = ResolveDiag(options[0], name);
            UIModel.Dialogue_1 = ResolveDiag(options[1], name);
            UIModel.Dialogue_2 = ResolveDiag(options[2], name);

            Debug.Log("Stage2UIModel dialogue0 : " + UIModel.Dialogue_0);
        }

        List<Tuple<string, string>> ResolveDiag(DialogueEntry dilogStruct, string name)
        {
            List<Tuple<string, string>> res = new List<Tuple<string, string>>();
            
            if(dilogStruct.prompt != null)
            {
                res.Add(Tuple.Create("你", dilogStruct.prompt));
            }
            if (dilogStruct.answer != null) {
                res.Add(Tuple.Create(name, dilogStruct.answer));
            }
            if (dilogStruct.reply != null)
            {
                res.Add(Tuple.Create(name, dilogStruct.reply));
            }

            return res;
        }

    }


}