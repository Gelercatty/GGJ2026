using System.Text;
using UnityEngine;
using QFramework;

namespace GGJ2026
{
    public class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            // 1) 加载 CaseDatabase（CaseRepositorySystem 里会 Register 到 ICaseLibraryModel）
            var repo = GameApp.Interface.GetSystem<ICaseRepositorySystem>();
            repo.LoadAll("Content/CaseDatabase"); // 
            GameApp.Interface.SendCommand(new StartNewGameCommand());
            
            // 2) 打印所有 case 资产
            var lib = GameApp.Interface.GetModel<ICaseLibraryModel>();

            var sb = new StringBuilder();
            sb.AppendLine($"[Bootstrap] Loaded Case Count = {lib.CaseIds.Count}");

            foreach (var caseId in lib.CaseIds)
            {
                var c = repo.Get(caseId);
                if (c == null) continue;

                sb.AppendLine(
                    $"- {c.caseId} | assetName={c.name} | " +
                    $"silhouette={(c.silhouette != null ? c.silhouette.name : "null")} | " +
                    $"prisonShots=[{(c.prisonShots?[0] ? c.prisonShots[0].name : "null")}, " +
                                  $"{(c.prisonShots?[1] ? c.prisonShots[1].name : "null")}, " +
                                  $"{(c.prisonShots?[2] ? c.prisonShots[2].name : "null")}] | " +
                    $"correctIndex={c.correctIndex}"+
                    $"stage1ClueText={c.stage1ClueText}"
                );
            }

            Debug.Log(sb.ToString());

            // 3) 顺便演示：启动一轮（见下文业务示例）
            StartOneRoundExample();
        }

        private void StartOneRoundExample()
        {
            // 这里演示“业务代码怎么用资产”：
            // 选一个 Case 作为本轮数据，写进 Model，并打印关键信息
            var repo = GameApp.Interface.GetSystem<ICaseRepositorySystem>();
            var lib  = GameApp.Interface.GetModel<ICaseLibraryModel>();

            if (lib.CaseIds.Count == 0) return;

            var pickedId = lib.CaseIds[Random.Range(0, lib.CaseIds.Count)];
            var picked = repo.Get(pickedId);

            // 写入全局状态（假设你有这些 Model 接口；没有就按你的实际 Model 改字段名）
            var game = GameApp.Interface.GetModel<GameStateModel>();
            game.CurrentCaseId.Value = pickedId;
            game.Phase.Value = GamePhase.Stage1;
            
            var stage1 = GameApp.Interface.GetModel<UIStage_1_Model>();
            stage1.ClueText.Value = picked.stage1ClueText;
            
            Debug.Log($"[Round] Picked Case = {picked.caseId}\nClueText = {picked.stage1ClueText}");
            
            // 业务里直接用资产（例如 UI 需要图）
            // 举例：显示剪影、显示囚照（第二阶段开灯后）
            // （这里只 Debug，真实项目里你会把 sprite 赋给 Image 组件）
            Debug.Log($"[UseAsset] silhouette sprite = {picked.silhouette}");
            Debug.Log($"[UseAsset] prison shot 0 = {picked.prisonShots[0]}");
        }
    }
}
