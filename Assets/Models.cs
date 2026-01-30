using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
namespace GGJ2026
{
    
    // 游戏全局管理
    public class GameStateModel : AbstractModel
    {
        // 游戏阶段
        public BindableProperty<GamePhase> Phase { get; } = new(GamePhase.Stage1);
        // 当前 案件 的id
        public BindableProperty<string> CurrentCaseId { get; } = new(string.Empty);
        // 失败原因
        public BindableProperty<string> FailReason { get; } = new(string.Empty);
        // 全局，成功次数
        public BindableProperty<int> SolvedCount { get; } = new(0);
        public BindableProperty<int> FailedCount { get; } = new(0);

        protected override void OnInit() { }
    }
    
    // 第一阶段UI的Model    
    public class Stage1Model : AbstractModel
    {   // 候选的id
        public List<string> CandidateCaseIds { get; } = new();
        // ？
        public BindableProperty<int> CandidatesVersion { get; } = new(0);
        // 线索文本
        public BindableProperty<string> ClueText { get; } = new(string.Empty);
        // 玩家正在查看的id
        public BindableProperty<string> CheckingCandidateCaseId { get; } = new(string.Empty);
        // 确认时的id 用于校验
        public BindableProperty<string> ConfirmPickCandidateCaseId { get; } = new(string.Empty);

        protected override void OnInit() { }
    }
    // 阶段二 ui model
    public class Stage2Model : AbstractModel
    {
        // 嫌犯列表
        public List<SuspectRuntime> Suspects { get; } = new();
        public BindableProperty<int> SuspectsVersion { get; } = new(0);
        
        
        public BindableProperty<int> CorrectIndex { get; } = new(-1);
        public BindableProperty<int> SelectedIndex { get; } = new(-1);
        public BindableProperty<bool> LightOn { get; } = new(false);

        public BindableProperty<string> CurrentDialogueGraphId { get; } = new(string.Empty);
        public BindableProperty<string> CurrentNodeId { get; } = new(string.Empty);

        public List<DialogueLine> DialogueRecord { get; } = new();
        public BindableProperty<int> DialogueRecordVersion { get; } = new(0);

        public List<DialogueOption> CurrentOptions { get; } = new();
        public BindableProperty<int> OptionsVersion { get; } = new(0);

        protected override void OnInit() { }
    }
    // 笔记
    public class NotesModel : AbstractModel
    {
        public List<NoteEntry> Notes { get; } = new();
        public BindableProperty<int> NotesVersion { get; } = new(0);

        public BindableProperty<float> ScrollPos { get; } = new(1f);

        protected override void OnInit() { }
    }
    // 拖拽过程的model 暂时没啥用
    public class UIDragModel : AbstractModel 
    {
        public BindableProperty<bool> IsDragging { get; } = new(false);
        public BindableProperty<DragPayloadType> PayloadType { get; } = new(DragPayloadType.None);
        public BindableProperty<string> PayloadId { get; } = new(string.Empty);
        public BindableProperty<string> HoverZoneId { get; } = new(string.Empty);

        protected override void OnInit() { }
    }
    
    
    
    // 资产模型
    public interface ICaseLibraryModel : IModel
    {
        IReadOnlyList<string> CaseIds { get; }
        void Register(CasePackSO c);
        bool TryGet(string caseId, out CasePackSO c);
    }

    public class CaseLibraryModel : AbstractModel, ICaseLibraryModel
    {
        private readonly List<string> _ids = new();
        private readonly Dictionary<string, CasePackSO> _map = new();

        public IReadOnlyList<string> CaseIds => _ids;

        public void Register(CasePackSO c)
        {
            if (c == null || string.IsNullOrWhiteSpace(c.caseId)) return;

            if (!_map.ContainsKey(c.caseId))
                _ids.Add(c.caseId);

            _map[c.caseId] = c;
        }

        public bool TryGet(string caseId, out CasePackSO c)
            => _map.TryGetValue(caseId, out c);

        protected override void OnInit() { }
    }
}