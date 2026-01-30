using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
namespace GGJ2026
{
    // 游戏阶段
    public enum GamePhase
    {
        Stage1,
        Stage2,
        GameOver,
        Win
    }

    public enum DragPayloadType
    {
        None,
        Stage1Candidate,  // 候选剪影
        Stage2Suspect      // 待审剪影
    }
    // 笔记的阶段
    public enum NoteSource
    {
        Stage1,
        Stage2
    }

    public enum DialogueSpeaker
    {
        Player,
        Suspect,
        System
    }

    [Serializable]
    public class NoteEntry
    {
        public string Id;          // 可选：用于去重或定位
        public NoteSource Source;
        public string Text;
        public long UtcTicks;      // 排序/调试
    }

    [Serializable]
    public class DialogueLine
    {
        public DialogueSpeaker Speaker;
        public string Text;
    }

    [Serializable]
    public class DialogueOption
    {
        public string OptionId;    // 供系统识别
        public string Text;
        public bool Enabled = true;
    }

// 二阶段嫌疑人运行态
    [Serializable]
    public class SuspectRuntime
    {
        public string SuspectId;       // 对应 CaseData 里的 suspect 条目 id 或 index 映射
        public bool ShowRaw;     // 是否已开灯显示囚照
    }
    
    // SO数据
    [Serializable]
    public class CaseItem
    {
        public string CaseId;
        
    }
}