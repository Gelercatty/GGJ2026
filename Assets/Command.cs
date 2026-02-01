using QFramework;
using System.Diagnostics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.U2D.IK;
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

    public class FinnalConfirmCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            string caseid = GameApp.Interface.GetModel<GameStateModel>().CurrentCaseId.Value;
            if (GameApp.Interface.GetModel<UIStage_2_Model>().Selectedidx.Value == GameApp.Interface.GetSystem<ICaseRepositorySystem>().Get(caseid).correctIndex){

                GameApp.Interface.SendEvent<winStage2>();
            }else
            {
                GameApp.Interface.SendEvent<lossStage2>();
            }
        }
    }
    public class SetStage2Selectedidx : AbstractCommand
    {
        public int _selectedidx;

        public SetStage2Selectedidx(int selectedidx)
        {
            this._selectedidx = selectedidx;
        }

        protected override void OnExecute()
        {
            GameApp.Interface.GetModel<UIStage_2_Model>().Selectedidx.Value = this._selectedidx;
        }
    }
    public class StartDialogueCommand: AbstractCommand
    {
        public int _id;
        public StartDialogueCommand(int id)
        {
            _id = id;
        }

        protected override void OnExecute()
        {
            var model = GameApp.Interface.GetModel<UIStage_2_Model>();
            switch (_id)
            {
                case 0:
                    model.Show_dialogue = model.Dialogue_0;
                    break;
                case 1:
                    model.Show_dialogue = model.Dialogue_1;
                    break;
                case 2:
                    model.Show_dialogue = model.Dialogue_2;
                    break;
            }
            this.SendEvent(new StartDialogue());
        }
    }
    public class Stage1ConfirmCommand: AbstractCommand
    {
        //public GameObject Win_UI;//prefab
        protected override void OnExecute()
        {
            var selected = this.GetModel<UIStage_1_Model>().SelectedCaseId;
            this.GetSystem<IGameFlowSystem>().ResolveStage1(selected); 
            this.SendEvent<ResultChangeEvent>();
        }
    }

    public class EnterStage2Command : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<IGameFlowSystem>().StartStage2Game();
            UnityEngine.Debug.Log("start game command");
            this.SendEvent<LoadStage2UI>();
            this.SendEvent<EntryStage2Event>();
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