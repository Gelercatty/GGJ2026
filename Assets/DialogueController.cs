//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using QFramework;
//using GGJ2026;
//using Unity.VisualScripting;
//using System;
//using TMPro;
//public class DialogueController : MonoBehaviour
//{

//    public TextMeshProUGUI _name;
//    public TextMeshProUGUI _speeak;
//    public IArchitecture GetArchitecture() => GameApp.Interface;

//    // Start is called before the first frame update
//    void Start()
//    {
//        GameApp.Interface.RegisterEvent<StartDialogue>(e =>
//        {

//        }).UnRegisterWhenGameObjectDestroyed(gameObject);

//       List<Tuple<string, string>> dialog = GameApp.Interface.GetModel<UIStage_2_Model>().Show_dialogue;
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}


using System.Collections.Generic;
using UnityEngine;
using QFramework;
using GGJ2026;
using TMPro;
using UnityEngine.UI; // 必须引用 UI

public class DialogueController : MonoBehaviour, IController
{
    public TextMeshProUGUI _name;
    public TextMeshProUGUI _speeak;
    public Button clickOverlay; // 在 Inspector 拖入一个全屏透明按钮

    private List<System.Tuple<string, string>> _dialogData;
    private int _currentIndex = 0;

    public IArchitecture GetArchitecture() => GameApp.Interface;

    void Start()
    {
        // 初始隐藏对话框和遮罩
        HideDialogue();

        // 注册对话开始事件
        this.RegisterEvent<StartDialogue>(e =>
        {
            StartDialogueFlow();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        // 绑定点击事件：推进对话
        clickOverlay.onClick.AddListener(OnOverlayClicked);
    }

    private void StartDialogueFlow()
    {
        _dialogData = this.GetModel<UIStage_2_Model>().Show_dialogue;
        _currentIndex = 0;

        if (_dialogData != null && _dialogData.Count > 0)
        {
            ShowDialogue();
            UpdateUI();
        }
    }

    private void OnOverlayClicked()
    {
        _currentIndex++;
        if (_currentIndex < _dialogData.Count)
        {
            UpdateUI();
        }
        else
        {
            EndDialogue();
        }
    }

    private void UpdateUI()
    {
        var currentLine = _dialogData[_currentIndex];
        _name.text = currentLine.Item1;
        _speeak.text = currentLine.Item2;
    }

    private void ShowDialogue()
    {
        gameObject.SetActive(true); // 激活对话框物体
        clickOverlay.gameObject.SetActive(true); // 激活遮罩，拦截下方一切交互
    }

    private void EndDialogue()
    {
        HideDialogue();
        // 发送对话结束命令或事件，告诉游戏其他系统可以继续
        // this.SendCommand<ResumeGameCommand>(); 
    }

    private void HideDialogue()
    {
        clickOverlay.gameObject.SetActive(false); // 释放拦截
        // 如果这个脚本挂在 UI Panel 上，可以直接：
        // gameObject.SetActive(false); 
    }
}