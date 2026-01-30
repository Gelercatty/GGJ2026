using QFramework;
namespace DefaultNamespace
{
    public class ConterApp : Architecture<ConterApp> 
    {
        // 在这里注册Model,
        protected override void Init()
        {
            // this.RegisterModel<ICounterModel>(new CounterModel());
        }
    }
}