using QFramework;
using UnityEditor.Tilemaps;

namespace GGJ2026
{
    // 加载所有的资源
    public class LoadCaseDataBaseCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            // TODO LOAD EVERY THING
        }
    }
    /// <summary>开始一局游戏：随机选 Case，写入 GameStateModel</summary>
    public class StartNewGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            // 业务放到 System 里，Command 只负责触发
            this.GetSystem<IGameFlowSystem>().StartNewGame();
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