using System;
using System.Text;
using UnityEngine;
using QFramework;
using System.Collections;

namespace GGJ2026
{
    public class GameBootstrap : MonoBehaviour
    {
        public GameObject Winpage;
        public GameObject LossPage;




        private void Awake()
        {
            GameApp.Interface.RegisterEvent<SourceLoadedEvent>(e =>
            {
                //TestHyperlinkFunctionality();
                GameApp.Interface.SendCommand(new StartNewGameCommand());
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            GameApp.Interface.RegisterEvent<lossStage2>(e =>
            {
                StartCoroutine(HandleEndGameSequence(LossPage));
            });


            GameApp.Interface.RegisterEvent<winStage2>(e =>
            {
                StartCoroutine(HandleEndGameSequence(Winpage));
            });

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


        /// <summary>
        /// 处理游戏结束的通用协程
        /// </summary>
        /// <param name="pageToShow">要显示的UI面板</param>
        private IEnumerator HandleEndGameSequence(GameObject pageToShow)
        {
            // 1. 显示对应的页面
            if (pageToShow != null)
            {
                pageToShow.SetActive(true);
            }

            // 2. 等待 3 秒
            yield return new WaitForSeconds(3);

            // 3. 隐藏页面（如果重启游戏不销毁该物体的话）
            if (pageToShow != null)
            {
                pageToShow.SetActive(false);
            }

            // 4. 发送重启游戏指令
            GameApp.Interface.SendCommand(new StartNewGameCommand());
        }
    }
}
