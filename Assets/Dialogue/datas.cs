using QFramework;
using UnityEngine;
namespace GGJ2026.Dialogue
{
    [System.Serializable]
    public class DialogueOption
    {
        public string text;     // 按钮显示
        public string nextId;   // 选了跳到哪个节点（空=结束）
    }

    [System.Serializable]
    public class DialogueNode
    {
        public string id;
        public string speaker;
        [TextArea] public string line;
        public string nextId;                 // 没选项时用这个自动跳
        public DialogueOption[] options;      // 有选项就走选项
    }
    [CreateAssetMenu(menuName="Game/DialogueDB")]
    public class DialogueDB : ScriptableObject
    {
        public DialogueNode[] nodes;
    } 
}