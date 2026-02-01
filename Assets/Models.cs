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
        public BindableProperty<int> Selectedidx = new(-1); // 选中的哪个 

        public string ButtonText_1 = string.Empty;
        public string ButtonText_2 = string.Empty;
        public string ButtonText_3 = string.Empty;

        public List<string> Dialogue_1 = new List<string>();
        public List<string> Dialogue_2 = new List<string>();
        public List<string> Dialogue_3 = new List<string>();


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