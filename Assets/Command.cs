using QFramework;
using System.Diagnostics;
using UnityEditor.Tilemaps;
using UnityEngine;
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
    public class BuildStage1Candidate : AbstractCommand
    {
        protected override void OnExecute()
        {
            // TODO： 设置第一阶段UI Model
        }
    }
    
    // Stage1InspectCandidateCommand(string caseId)
    //
    public class Stage1AddClue2Notes : AbstractCommand
    {
        public string clue_add_id;
        protected override void OnExecute()
        {
            // TODO: 
        }
    }
    public class Stage1ConfirmCommand: AbstractCommand
    {
        protected override void OnExecute()
        {
            // TODO: 判断第一阶段结果
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