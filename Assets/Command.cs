using QFramework;
using System.Diagnostics;
using UnityEditor.Tilemaps;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GGJ2026
{
    // 加载所有的资源
    public class LoadCaseDataBaseCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var repo = GameApp.Interface.GetSystem<ICaseRepositorySystem>();
            repo.LoadAll("Content/CaseDatabase");
            UnityEngine.Debug.Log("loaded all");
            GameApp.Interface.SendEvent<SourceLoadedEvent>();
        }
    }
    /// <summary>开始一局游戏：随机选 Case，写入 GameStateModel</summary>
    public class StartNewGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<IGameFlowSystem>().StartNewGame();
            UnityEngine.Debug.Log("start game command");
            GameApp.Interface.SendEvent<LoadStage1UI>();
        }
    }
    // BuildStage1CandidatesCommand(int n)
    public class SetStage1SelectedCaseID : AbstractCommand
    {
        public string CaseId;

        public SetStage1SelectedCaseID(string caseId)
        {
            this.CaseId = caseId;
        }
        protected override void OnExecute()
        {
            GameApp.Interface.GetModel<UIStage_1_Model>().SelectedCaseId = this.CaseId;
        }
    }
    
           
    public class Stage1ConfirmCommand: AbstractCommand
    {
        //public GameObject Win_UI;//prefab
        protected override void OnExecute()
        {
            // TODO: 判断第一阶段结果
            // TODO: 
            
            string selected = GameApp.Interface.GetModel<UIStage_1_Model>().SelectedCaseId;
            string currentCase = GameApp.Interface.GetModel<GameStateModel>().CurrentCaseId.Value;

            Debug.Log("selected:"+selected);
            Debug.Log("currentCase:"+currentCase);
            if (selected == currentCase)
            {
                Debug.Log("stage1 win");
                //todo win logic
                GameApp.Interface.GetModel<UIStage_1_Model>().IsWin=true;
                GameApp.Interface.GetModel<UIStage_1_Model>().IsLose=false;
                //freeze_command
                //var win_ui = Instantiate(Win_UI,transform);
                //clear view

                //goto stage2 command
                
            }
            else
            {
                Debug.Log("stage1 lose");
                //todo lose logic
                //got stage1 command
                GameApp.Interface.GetModel<UIStage_1_Model>().IsWin=false;
                GameApp.Interface.GetModel<UIStage_1_Model>().IsLose=true;
            }
        }
    }

    public class EnterStage2Command : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<IGameFlowSystem>().StartNewGame();
            UnityEngine.Debug.Log("start game command");
        }
    }
    // EnterStage2Command
    //
    //     Stage2SelectSuspectCommand(int index)
    //
    // Stage2TurnOnLightCommand
    //
    //     Stage2ChooseDialogueOptionCommand(string optionId)
    //
    // Stage2VerdictCommand
    //
    //     RestartGameCommand
}