using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace GGJ2026
{
    public interface ICaseRepositorySystem : ISystem
    {
        /// <summary>从 Resources 路径加载全部 CasePackSO（默认 Content/Case）</summary>
        void LoadAll(string resourcesPath = "Content/Case");

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

        public void LoadAll(string resourcesPath = "Content/Case")
        {
            var lib = this.GetModel<ICaseLibraryModel>();

            var all = Resources.LoadAll<CasePackSO>(resourcesPath);
            if (all == null || all.Length == 0)
            {
                Debug.LogWarning($"[CaseRepositorySystem] No CasePackSO found at Resources/{resourcesPath}. " +
                                 $"请确认路径为 Assets/Resources/{resourcesPath}/ 并放入 CasePackSO 资产。");
                _loaded = true;
                return;
            }

            // 注册 + 基础校验
            var seen = new HashSet<string>();
            int okCount = 0;

            foreach (var c in all)
            {
                if (c == null) continue;

                if (string.IsNullOrWhiteSpace(c.caseId))
                {
                    Debug.LogError($"[CaseRepositorySystem] CasePackSO({c.name}) caseId 为空，已跳过。");
                    continue;
                }

                if (!seen.Add(c.caseId))
                {
                    Debug.LogError($"[CaseRepositorySystem] caseId 重复：{c.caseId}（资产名：{c.name}），后加载的会覆盖前者。");
                }

                lib.Register(c);
                okCount++;
            }

            _loaded = true;
            Debug.Log($"[CaseRepositorySystem] Loaded {okCount} case assets from Resources/{resourcesPath}.");
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
}