using QFramework;

namespace GGJ2026
{
    public struct SourceLoadedEvent
    {
    }

    public struct LoadStage1UI
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

}