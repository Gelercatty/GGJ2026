using QFramework;
namespace GGJ2026
{
    
    public class GameApp : Architecture<GameApp> 
    {
        // 在这里注册Model,
        protected override void Init()
        {
            // this.RegisterModel<ICounterModel>(new CounterModel());
            
            RegisterModel<ICaseLibraryModel>(new CaseLibraryModel());
            RegisterModel<GameStateModel>(new GameStateModel());
            RegisterModel<UIStage_1_Model>(new UIStage_1_Model());
            RegisterModel<UIStage_2_Model>(new UIStage_2_Model());
             
            RegisterSystem<DebugGameStateSystem>(new DebugGameStateSystem());
            RegisterSystem<ICaseRepositorySystem>(new CaseRepositorySystem());
            RegisterSystem<IGameFlowSystem>(new GameFlowSystem());
        }
    }
}