using QFramework;
namespace GGJ2026
{
    
    public class ConterApp : Architecture<ConterApp> 
    {
        // 在这里注册Model,
        protected override void Init()
        {
            // this.RegisterModel<ICounterModel>(new CounterModel());
            
            RegisterModel<ICaseLibraryModel>(new CaseLibraryModel());
            
            RegisterSystem<ICaseRepositorySystem>(new CaseRepositorySystem());

        }
    }
}