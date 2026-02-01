
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace GGJ2026
{

    // so 存贮系统
    public interface ICaseRepositorySystem : ISystem
    {
        /// <summary>
        /// 从 Resources 加载 CaseDatabase（默认 Content/CaseDatabase）
        /// </summary>
        void LoadAll(string databaseResourcePath = "Content/CaseDatabase");

        /// <summary>按 caseId 获取 CasePackSO，找不到返回 null</summary>
        CasePackSO Get(string caseId);

        /// <summary>是否已完成加载</summary>
        bool IsLoaded { get; }

        /// <summary>
        /// 根据超链接ID在整个系统中查找对应的线索文本
        /// </summary>
        /// <param name="linkId">超链接ID</param>
        /// <returns>对应的线索文本，如果未找到返回空字符串</returns>
        string GetClueTextByLinkId(string linkId);

        /// <summary>
        /// 检查系统中是否存在指定的超链接ID
        
        /// <param name="linkId">超链接ID</param>
        /// <returns>是否存在</returns>
        bool HasLinkId(string linkId);
    }

    public class CaseRepositorySystem : AbstractSystem, ICaseRepositorySystem
    {
        private bool _loaded;
        public bool IsLoaded => _loaded;

        protected override void OnInit()
        {
        }

        public void LoadAll(string databaseResourcePath = "Content/CaseDatabase")
        {
            var lib = this.GetModel<ICaseLibraryModel>();

            var db = Resources.Load<CaseDatabaseSO>(databaseResourcePath);
            if (db == null)
            {
                Debug.LogError($"[CaseRepositorySystem] CaseDatabaseSO not found at Resources/{databaseResourcePath}. " +
                               $"请确认数据库资产位于 Assets/Resources/{databaseResourcePath}.asset");
                _loaded = true;
                return;
            }

            if (db.cases == null || db.cases.Count == 0)
            {
                Debug.LogWarning($"[CaseRepositorySystem] CaseDatabaseSO is empty: Resources/{databaseResourcePath}");
                _loaded = true;
                return;
            }

            // 注册 + 基础校验（唯一性/空引用）
            var seen = new HashSet<string>();
            int okCount = 0;

            foreach (var c in db.cases)
            {
                if (c == null)
                {
                    Debug.LogError("[CaseRepositorySystem] CaseDatabaseSO contains null CasePackSO reference.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(c.caseId))
                {
                    Debug.LogError($"[CaseRepositorySystem] CasePackSO({c.name}) caseId 为空，已跳过。");
                    continue;
                }

                if (!seen.Add(c.caseId))
                {
                    Debug.LogError($"[CaseRepositorySystem] caseId 重复：{c.caseId}（资产名：{c.name}），后者会覆盖前者。");
                }

                lib.Register(c);
                okCount++;
            }

            _loaded = true;
            Debug.Log($"[CaseRepositorySystem] Loaded {okCount} cases from CaseDatabase: Resources/{databaseResourcePath}.");
        }

        public CasePackSO Get(string caseId)
        {
            var lib = this.GetModel<ICaseLibraryModel>();
            if (lib.TryGet(caseId, out var c))
                return c;

            Debug.LogError($"[CaseRepositorySystem] Case not found: {caseId}");
            return null;
        }

        public string GetClueTextByLinkId(string linkId)
        {
            if (!IsLoaded || string.IsNullOrEmpty(linkId))
                return string.Empty;

            var lib = this.GetModel<ICaseLibraryModel>();
            
            // 遍历所有案件，查找匹配的linkId
            foreach (var caseId in lib.CaseIds)
            {
                if (lib.TryGet(caseId, out var casePack))
                {
                    string clueText = casePack.GetHyperlinkClueText(linkId);
                    if (!string.IsNullOrEmpty(clueText))
                    {
                        return clueText;
                    }
                }
            }

            Debug.LogWarning($"[CaseRepositorySystem] LinkId not found: {linkId}");
            return string.Empty;
        }

        public bool HasLinkId(string linkId)
        {
            if (!IsLoaded || string.IsNullOrEmpty(linkId))
                return false;

            var lib = this.GetModel<ICaseLibraryModel>();
            
            // 遍历所有案件，检查是否存在指定的linkId
            foreach (var caseId in lib.CaseIds)
            {
                if (lib.TryGet(caseId, out var casePack))
                {
                    if (casePack.HasHyperlinkId(linkId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }


    public interface IGameFlowSystem : ISystem
    {
        // 选择case 刷新phase
        void StartNewGame();
        // debug 用,直接跳到二阶段
        void StartStage2Game();
        // 重置所有游戏状态
        void ResetGameState();
        // void GameOver_stage1();
        // void GameOver_stage2();

        void Stage1Win();
        void Stage1Loss();
        void Stage2Win();
        void Stage2Loss();
        void ResolveStage1(string selectedCaseId); 
    }

    public class GameFlowSystem : AbstractSystem, IGameFlowSystem
    {
        protected override void OnInit() { }
        private const int MAX_CASES = 3;

        public void StartNewGame()
        {
            var repo = this.GetSystem<ICaseRepositorySystem>();
            if (!repo.IsLoaded)
            {
                Debug.LogError("[GameFlowSystem] CaseRepositorySystem not loaded yet. Call repo.LoadAll() before StartNewGame.");
                return;
            }

            var lib = this.GetModel<ICaseLibraryModel>();
            if (lib.CaseIds == null || lib.CaseIds.Count == 0)
            {
                Debug.LogError("[GameFlowSystem] No cases available in CaseLibraryModel.");
                return;
            }

            // 0) 先抽取最多 MAX_CASE 个作为候选池（不重复）
            var pickCount = Mathf.Min(MAX_CASES, lib.CaseIds.Count);

            // 拷贝一份池子并洗牌，然后取前 pickCount 个
            var pool = new List<string>(lib.CaseIds);
            Shuffle(pool);

            var candidateIds = pool.GetRange(0, pickCount);

            // 1) 从候选池中随机选一个作为目标 CaseId
            var idx = Random.Range(0, candidateIds.Count);
            var pickedCaseId = candidateIds[idx];

            // 2) 写入 GameStateModel
            var game = this.GetModel<GameStateModel>();
            game.Phase.Value = GamePhase.Stage1;
            game.CurrentCaseId.Value = pickedCaseId;

            // 3) 写入 Stage1 UI Model（候选 + 线索）
            var stage1 = this.GetModel<UIStage_1_Model>();

            stage1.CaseIds.Clear();
            stage1.CaseIds.AddRange(candidateIds);
            stage1.CollectedClueTexts.Clear();
            // 从 repo 取当前目标的 clue
            var pack = repo.Get(pickedCaseId);
            stage1.ClueText.Value = pack != null ? pack.stage1ClueText : string.Empty;
            Debug.Log($"[GameFlowSystem] New game started. Candidates={candidateIds.Count}, CurrentCaseId={pickedCaseId}");
        }

        public void StartStage2Game()
        {
            var game = this.GetModel<GameStateModel>();
            game.Phase.Value = GamePhase.Stage2;
                        
        }

        public void ResetGameState()
        {
            
        }

        public void Stage1Win()
        {
            GameApp.Interface.GetModel<GameStateModel>().Phase.Value = GamePhase.Win_stage1;
        }
        public void Stage1Loss()
        {
            GameApp.Interface.GetModel<GameStateModel>().Phase.Value = GamePhase.GameOver_1;
        }

        public void Stage2Win()
        {
            var game = this.GetModel<GameStateModel>();
            game.Phase.Value = GamePhase.Win_stage2;
            
            game.Round.Value++;
        }

        public void Stage2Loss()
        {
            GameApp.Interface.GetModel<GameStateModel>().Phase.Value = GamePhase.GameOver_2;
        }

        public void ResolveStage1(string selectedCaseId)
        {
            var game = this.GetModel<GameStateModel>();
            var current = game.CurrentCaseId.Value;

            if (selectedCaseId == current)
            {
                game.Phase.Value = GamePhase.Win_stage1;
            }
            
            else
            {    game.Phase.Value = GamePhase.GameOver_1;} 
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

    }
        public interface IDebugGameStateSystem : ISystem { }

        public class DebugGameStateSystem : AbstractSystem, IDebugGameStateSystem
        {
            private GamePhase _last;

            protected override void OnInit()
            {
                var game = this.GetModel<GameStateModel>();
                _last = game.Phase.Value;

                // 立刻打印一次当前状态 + 之后每次变化都打印
                game.Phase.RegisterWithInitValue(now =>
                {
                    Debug.Log($"[DebugGameState] Phase: {_last} -> {now}");
                    _last = now;
                });
                var stage2ui = this.GetModel<UIStage_2_Model>();
                stage2ui.Selectedidx.RegisterWithInitValue(id =>
                {
                    Debug.Log($"[DebugGameState] current Selected: {id}");
                });
            }
        }

}
