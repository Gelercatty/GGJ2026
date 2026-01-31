using QFramework;
using UnityEngine;
namespace GGJ2026.Dialogue
{
    public class DialogueModel : AbstractModel
    {
        public BindableProperty<bool> IsOpen = new(false);
        public BindableProperty<string> Speaker = new("");
        public BindableProperty<string> Line = new("");

        // 选项列表本身不是 BindableProperty（省事），用版本号通知 UI 重建按钮
        public readonly System.Collections.Generic.List<DialogueOption> Options = new();
        public BindableProperty<int> OptionsVersion = new(0);

        protected override void OnInit()
        {
        }
    }
}