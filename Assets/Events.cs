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
}