﻿﻿using QFramework;

namespace GGJ2026
{
    public struct SourceLoadedEvent
    {
    }

    public struct LoadStage1UI
    {
        
    }
    public struct LoadStage2UI
    {
        
    }
    public struct SetEveryThingSil
    {

    }
    public struct EntryStage2Event
    {
        
    }
    public struct OnRoundCaseSelectedEvent
    {
        public string CaseId;
    }
    
    public struct ClueTextChangedEvent
    {
        /// <summary>超链接ID</summary>
        public string LinkId;
        
        /// <summary>收集到的线索文本</summary>
        public string ClueText;
        
        /// <summary>是否是新收集的（不是重复点击）</summary>
        public bool IsNewCollection;
        
        public ClueTextChangedEvent(string linkId, string clueText, bool isNewCollection)
        {
            LinkId = linkId;
            ClueText = clueText;
            IsNewCollection = isNewCollection;
        }
    }


    public struct ResultChangeEvent
    {
    // 是否胜利（或者你也可以只用一个枚举 GameResult 字段）
    //public readonly bool IsWin;
    // 是否失败（通常 IsWin 和 IsLose 是互斥的，但这样设计更清晰）
    //public readonly bool IsLose;
        public bool isWin ;// =  GameApp.Interface.GetModel<UIStage_1_Model>().IsWin;
        public bool isLose ;//=  GameApp.Interface.GetModel<UIStage_1_Model>().IsLose; 


    }

    // 对话系统事件
    public struct OnLightedEvent
    {
        // 点亮事件，激活对话按钮
    }
    
    public struct NextTextEvent
    {
        // 显示下一句文本
    }
    
    public struct DialogueOptionSelectedEvent
    {
        public int OptionIndex;
        
        public DialogueOptionSelectedEvent(int optionIndex)
        {
            OptionIndex = optionIndex;
        }
    }
    
    public struct DialogueStartedEvent
    {
        public int SuspectIndex;
        public int OptionIndex;
        
        public DialogueStartedEvent(int suspectIndex, int optionIndex)
        {
            SuspectIndex = suspectIndex;
            OptionIndex = optionIndex;
        }
    }
    
    public struct DialogueFinishedEvent
    {
        public int SuspectIndex;
        public int OptionIndex;
        
        public DialogueFinishedEvent(int suspectIndex, int optionIndex)
        {
            SuspectIndex = suspectIndex;
            OptionIndex = optionIndex;
        }
    }

}