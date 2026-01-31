using System;
using System.Text;
using UnityEngine;
using QFramework;

namespace GGJ2026
{
    public class GameBootstrap : MonoBehaviour
    {

        private void Awake()
        {
            GameApp.Interface.RegisterEvent<SourceLoadedEvent>(e =>
            {
                //TestHyperlinkFunctionality();
                GameApp.Interface.SendCommand(new StartNewGameCommand());
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        private void Start()
        {
            GameApp.Interface.SendCommand(new LoadCaseDataBaseCommand());
            
        }

        private void TestHyperlinkFunctionality()
        {
            var repo = GameApp.Interface.GetSystem<ICaseRepositorySystem>();
            
            // 测试是否存在linkId为15的超链接
            bool hasLink = repo.HasLinkId("15");
            Debug.Log($"是否存在linkId为15的超链接: {hasLink}");
            
            if (hasLink)
            {
                // 获取对应的线索文本
                string clueText = repo.GetClueTextByLinkId("15");
                Debug.Log($"linkId为15的线索文本: {clueText}");
            }
            else
            {
                Debug.LogWarning("未找到linkId为15的超链接");
            }
            
            // 测试不存在的linkId
            string nonExistentText = repo.GetClueTextByLinkId("999");
            Debug.Log($"不存在的linkId返回的文本: '{nonExistentText}' (应为空字符串)");
        }
    }
}
