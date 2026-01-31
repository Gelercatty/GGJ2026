using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026
{


    [CreateAssetMenu(menuName = "GGJ2026/Dialogue Sheet", fileName = "DialogueSheet")]
    public class DialogueSheetSO : ScriptableObject
    {
        public List<DialogueEntry> entries = new();
    }

    [Serializable]
    public class DialogueEntry
    {
        public string prompt; // 询问选项（按钮文本）
        public List<DialogueSegment> answer = new(); // 对应回答（解析后的段）
        public List<DialogueSegment> reply = new(); // 询问者回应（解析后的段）
    }

    public enum DialogueSegmentType
    {
        Text,
        Image
    }

    [Serializable]
    public struct DialogueSegment
    {
        public DialogueSegmentType type;
        public string value; // Text 内容 or Image key（img:xxx 的 xxx）

        public static DialogueSegment Text(string t) =>
            new DialogueSegment { type = DialogueSegmentType.Text, value = t };

        public static DialogueSegment Image(string key) => new DialogueSegment
            { type = DialogueSegmentType.Image, value = key };
    }
}