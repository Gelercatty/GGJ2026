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

            GameApp.Interface.SendEvent<ResultChangeEvent>();
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
    
    /// <summary>处理超链接点击</summary>
    public class HandleHyperlinkClickCommand : AbstractCommand
    {
        public string LinkId;

        public HandleHyperlinkClickCommand(string linkId)
        {
            this.LinkId = linkId;
        }
        
        protected override void OnExecute()
        {
            var stage1Model = this.GetModel<UIStage_1_Model>();
            var repo = this.GetSystem<ICaseRepositorySystem>();
            
            // 检查是否已经点击过
            if (stage1Model.ClickedHyperlinkIds.Contains(LinkId))
            {
                // 已经点击过，发送事件但标记为非新收集
                string clueText = repo.GetClueTextByLinkId(LinkId);
                this.SendEvent(new ClueTextChangedEvent(LinkId, clueText, false));
                return;
            }
            
            // 检查超链接是否存在
            if (!repo.HasLinkId(LinkId))
            {
                Debug.LogWarning($"超链接不存在: {LinkId}");
                return;
            }
            
            // 获取对应的线索文本
            string newClueText = repo.GetClueTextByLinkId(LinkId);
            if (string.IsNullOrEmpty(newClueText))
            {
                Debug.LogWarning($"超链接 {LinkId} 对应的线索文本为空");
                return;
            }
            
            // 记录点击并收集文本
            stage1Model.ClickedHyperlinkIds.Add(LinkId);
            stage1Model.CollectedClueTexts.Add(newClueText);
            
            Debug.Log($"成功收集超链接 {LinkId} 的线索文本: {newClueText}");
            
            // 发送事件，标记为新收集
            this.SendEvent(new ClueTextChangedEvent(LinkId, newClueText, true));
        }
    }
}