using QFramework;

namespace GGJ2026
{
    /// <summary>开始一局游戏：随机选 Case，写入 GameStateModel</summary>
    public class StartNewGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            // 业务放到 System 里，Command 只负责触发
            this.GetSystem<IGameFlowSystem>().StartNewGame();
        }
    }
}