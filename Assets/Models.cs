using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
namespace GGJ2026
{
    
    // 游戏全局管理
    public class GameStateModel : AbstractModel
    {
        public BindableProperty<int> Round { get; } = new(1);
        public BindableProperty<GamePhase> Phase { get; } = new(GamePhase.Stage1);
        // 当前 案件 的id
        public BindableProperty<string> CurrentCaseId { get; } = new(string.Empty);
        // 失败原因
        public BindableProperty<string> FailReason { get; } = new(string.Empty);
        // 全局，成功次数
        public BindableProperty<int> SolvedCount { get; } = new(0);
        public BindableProperty<int> FailedCount { get; } = new(0);
        public int HaJiMiRound = 1; // HAJIMI周目数
        protected override void OnInit() { }
    }

    
    // 第一阶段UI的Model    
    public class UIStage_1_Model : AbstractModel
    {   
        public BindableProperty<string> ClueText { get; } = new(string.Empty);
        public List<string> CaseIds = new List<string>();
        public string SelectedCaseId =  string.Empty;

        
        // 已点击的超链接ID集合（用于去重）
        public HashSet<string> ClickedHyperlinkIds { get; } = new HashSet<string>();
        
        // 已收集的线索文本列表
        public List<string> CollectedClueTexts { get; } = new List<string>();
        protected override void OnInit()
        {
       
        }
    }
    // 阶段二 ui model
    public class UIStage_2_Model : AbstractModel
    {
        // 嫌犯列表
       
        public List<bool> IsShown = new List<bool>();

        public BindableProperty<int> Selected_idx; // 选中的那个角色的idx 
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
    
    
    
    // Case model
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