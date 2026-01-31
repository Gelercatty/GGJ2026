// using System.Collections.Generic;
// using QFramework;
// using UnityEngine;
//
// namespace GGJ2026
// {
//     public interface ICaseRepositorySystem : ISystem
//     {
//         /// <summary>从 Resources 路径加载全部 CasePackSO（默认 Content/Case）</summary>
//         void LoadAll(string resourcesPath = "Content/Case");
//
//         /// <summary>按 caseId 获取 CasePackSO，找不到返回 null</summary>
//         CasePackSO Get(string caseId);
//
//         /// <summary>是否已完成加载</summary>
//         bool IsLoaded { get; }
//     }
//
//     public class CaseRepositorySystem : AbstractSystem, ICaseRepositorySystem
//     {
//         private bool _loaded;
//         public bool IsLoaded => _loaded;
//
//         protected override void OnInit()
//         {
//         }
//
//         public void LoadAll(string resourcesPath = "Content/Case")
//         {
//             var lib = this.GetModel<ICaseLibraryModel>();
//
//             var all = Resources.LoadAll<CasePackSO>(resourcesPath);
//             if (all == null || all.Length == 0)
//             {
//                 Debug.LogWarning($"[CaseRepositorySystem] No CasePackSO found at Resources/{resourcesPath}. " +
//                                  $"请确认路径为 Assets/Resources/{resourcesPath}/ 并放入 CasePackSO 资产。");
//                 _loaded = true;
//                 return;
//             }
//
//             // 注册 + 基础校验
//             var seen = new HashSet<string>();
//             int okCount = 0;
//
//             foreach (var c in all)
//             {
//                 if (c == null) continue;
//
//                 if (string.IsNullOrWhiteSpace(c.caseId))
//                 {
//                     Debug.LogError($"[CaseRepositorySystem] CasePackSO({c.name}) caseId 为空，已跳过。");
//                     continue;
//                 }
//
//                 if (!seen.Add(c.caseId))
//                 {
//                     Debug.LogError($"[CaseRepositorySystem] caseId 重复：{c.caseId}（资产名：{c.name}），后加载的会覆盖前者。");
//                 }
//
//                 lib.Register(c);
//                 okCount++;
//             }
//
//             _loaded = true;
//             Debug.Log($"[CaseRepositorySystem] Loaded {okCount} case assets from Resources/{resourcesPath}.");
//         }
//
//         public CasePackSO Get(string caseId)
//         {
//             var lib = this.GetModel<ICaseLibraryModel>();
//             if (lib.TryGet(caseId, out var c))
//                 return c;
//
//             Debug.LogError($"[CaseRepositorySystem] Case not found: {caseId}");
//             return null;
//         }
//     }
// }



using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace GGJ2026
{
    // so 存贮系统
    public interface ICaseRepositorySystem : ISystem {
        /// <summary>
        /// 从 Resources 加载 CaseDatabase（默认 Content/CaseDatabase）
        /// </summary>
        void LoadAll(string databaseResourcePath = "Content/CaseDatabase");

        /// <summary>按 caseId 获取 CasePackSO，找不到返回 null</summary>
        CasePackSO Get(string caseId);

        /// <summary>是否已完成加载</summary>
        bool IsLoaded { get; }
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
    }
    
    
    public interface IGameFlowSystem : ISystem
    {
        void StartNewGame();
    }

    public class GameFlowSystem : AbstractSystem, IGameFlowSystem
    {
        protected override void OnInit() { }

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

            // 1) 随机选一个 CaseId
            var idx = Random.Range(0, lib.CaseIds.Count);
            var pickedCaseId = lib.CaseIds[idx];

            // 2) 写入 GameStateModel
            var game = this.GetModel<GameStateModel>();
            game.Phase.Value = GamePhase.Stage1;
            game.CurrentCaseId.Value = pickedCaseId;
            
            // 3) TODO:
            // 数据读入到stage1的ui中
            // 4) 发事件：本局案件已选定（UI/其它系统监听）
            this.SendEvent(new OnRoundCaseSelected { CaseId = pickedCaseId }); 
            Debug.Log($"[GameFlowSystem] New game started. CurrentCaseId={pickedCaseId}");
        }
        
    }

}
