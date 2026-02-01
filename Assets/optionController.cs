using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using GGJ2026;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using Button = UnityEngine.UI.Button;
using System;
using System.Reflection;

public class optionController : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture() => GameApp.Interface;

    // 建议直接拖入 Button 组件，省去 GetComponent
    public Button[] options;

    public void Awake()
    {
        var model = this.GetModel<UIStage_2_Model>();
        // 1. 更新文本逻辑
        this.RegisterEvent<DiagnolLoaded>(e =>
        {
            options[0].GetComponentInChildren<TextMeshProUGUI>().text = model.ButtonText_0;
            options[1].GetComponentInChildren<TextMeshProUGUI>().text = model.ButtonText_1;
            options[2].GetComponentInChildren<TextMeshProUGUI>().text = model.ButtonText_2;
        }).UnRegisterWhenGameObjectDestroyed(gameObject);


        options[0].onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(model.ButtonText_0))
            {
                this.SendCommand(new StartDialogueCommand(0));
            }

        });
        options[1].onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(model.ButtonText_1))
            {
                this.SendCommand(new StartDialogueCommand(1));
            }

        });
        options[2].onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(model.ButtonText_2))
            {
                this.SendCommand(new StartDialogueCommand(2));
            }

        });


        //});
        //// 2. 绑定处理函数 (核心请求)
        //for (int i = 0; i < options.Length; i++)
        //{
        //    int index = i; // 闭包陷阱：必须存一个本地变量
        //    options[i].onClick.AddListener(() =>
        //    {
        //        // 如果需要判断值非 Null，可以从 Model 里拿数据判断
        //        var model = this.GetModel<UIStage_2_Model>();

        //        // 示例：假设对应的文本不为空才触发 Command
        //        if (!string.IsNullOrEmpty(model.GetTextByIndex(index)))
        //        {
        //            this.SendCommand(new YourCommand(index));
        //        }
        //    });
        //}
    }
}
