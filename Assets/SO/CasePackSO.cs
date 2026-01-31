using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026
{
    
[CreateAssetMenu(menuName = "CasePack/Case Pack", fileName = "CasePack_0001")]
public class CasePackSO : ScriptableObject
{
    [Header("Identity")]
    public string caseId;

    public string name;
    [Header("Stage 1")]
    public Sprite silhouette;                 // 剪影
    [TextArea(3, 10)]
    public string stage1ClueText;             // 一阶段文本线索（先用 string 占位）

    [Header("Stage 2")]
    [Tooltip("固定 3 个囚照")]
    public Sprite[] prisonShots = new Sprite[3];       // 囚照 x3

    [Tooltip("二阶段审问对话占位")]
    public DialogueSheetSO[] stage2Dialogues = new DialogueSheetSO[3]; // 对话占位 x3

    [Header("Answer")]
    [Range(0, 2)]
    public int correctIndex = 0;

    [Header("Clue Links")]
    [Tooltip("超链接的唯一标识ID")]
    public string hyperlinkId;
    
    [Tooltip("超链接对应的线索文本")]
    [TextArea(3, 10)]
    public string hyperlinkClueText;
    
    private void OnValidate()
    {
        if (prisonShots == null || prisonShots.Length != 3) prisonShots = ResizeTo3(prisonShots);
        if (stage2Dialogues == null || stage2Dialogues.Length != 3) stage2Dialogues = ResizeTo3(stage2Dialogues);

        correctIndex = Mathf.Clamp(correctIndex, 0, 2);
    }

    private static T[] ResizeTo3<T>(T[] src)
    {
        var arr = new T[3];
        if (src == null) return arr;
        for (int i = 0; i < Mathf.Min(3, src.Length); i++) arr[i] = src[i];
        return arr;
    }

    /// <summary>
    /// </summary>
    public List<string> ValidateBasic()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(caseId))
            errors.Add("caseId 为空");

        if (silhouette == null)
            errors.Add("剪影 silhouette 未设置");

        if (prisonShots == null || prisonShots.Length != 3)
            errors.Add("囚照数组 prisonShots 长度不是 3");

        if (prisonShots != null)
        {
            for (int i = 0; i < 3; i++)
                if (prisonShots[i] == null) errors.Add($"囚照 prisonShots[{i}] 未设置");
        }

        // 线索/对话先只做存在性提示
        if (string.IsNullOrWhiteSpace(stage1ClueText))
            errors.Add("一阶段文本线索 stage1ClueText 为空（可允许，但建议填写）");

        // 对话占位允许为空（你也可以改成强制）
        // for (int i = 0; i < 3; i++) if (stage2Dialogues[i] == null) errors.Add($"对话占位 stage2Dialogues[{i}] 未设置");

        return errors;
    }

    /// <summary>
    /// 获取超链接对应的线索文本
    /// </summary>
    /// <returns>超链接对应的线索文本</returns>
    public string GetHyperlinkClueText()
    {
        return hyperlinkClueText ?? string.Empty;
    }

    /// <summary>
    /// 获取超链接ID
    /// </summary>
    /// <returns>超链接ID</returns>
    public string GetHyperlinkId()
    {
        return hyperlinkId ?? string.Empty;
    }
}

}
