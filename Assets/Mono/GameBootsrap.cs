using UnityEngine;
using QFramework;


namespace GGJ2026
{
    public class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            // 初始化并加载全部 CasePackSO
            GameApp.Interface.GetSystem<ICaseRepositorySystem>().LoadAll("Content/Case");
            Debug.Log(GameApp.Interface.GetModel<Stage1Model>());
        }
    };
};