using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using TMPro;
using GGJ2026;

public class cNotePanelController : MonoBehaviour, IController
{
    public TextMeshProUGUI noteText;
    
    public IArchitecture GetArchitecture() => GameApp.Interface;
    
    private void Start()
    {
        // 确保 TextMeshPro 组件已赋值
        if (noteText == null)
            noteText = GetComponent<TextMeshProUGUI>();
        
        // 监听 ClueTextChangedEvent
        this.RegisterEvent<ClueTextChangedEvent>(OnClueTextChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        
        // 初始化显示已收集的线索文本
        UpdateNoteDisplay();
    }
    
    private void OnClueTextChanged(ClueTextChangedEvent e)
    {
        // 只有当是新收集的线索时才更新显示
        if (e.IsNewCollection)
        {
            UpdateNoteDisplay();
        }
    }
    
    private void UpdateNoteDisplay()
    {
        var stage1Model = this.GetModel<UIStage_1_Model>();
        
        if (noteText != null && stage1Model.CollectedClueTexts.Count > 0)
        {
            // 将收集到的线索文本按行显示
            noteText.text = string.Join("\n", stage1Model.CollectedClueTexts);
            Debug.Log($"笔记面板更新，显示 {stage1Model.CollectedClueTexts.Count} 条线索");
        }
        else if (noteText != null)
        {
            noteText.text = "暂无收集的线索，请从下方信息中找出并点选关键词吧，大侦探!  < UWU > ~";
        }
    }
}
