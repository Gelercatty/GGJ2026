
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GGJ2026
{


    public class DialogueSheetBuilderWindow : EditorWindow
    {
        private const string SAVE_DIR = "Assets/Resources/Content/Dialogue";

        [Serializable]
        private class Row
        {
            public string prompt;
            public string answerRaw;
            public string replyRaw;
        }

        [SerializeField] private List<Row> rows = new();

        // 新增：资产名字输入框
        [SerializeField] private string assetName = "DialogueSheet";

        private Vector2 _scroll;

        [MenuItem("Tools/GGJ2026/Dialogue Sheet Builder")]
        public static void Open() => GetWindow<DialogueSheetBuilderWindow>("Dialogue Sheet Builder");

        private void OnGUI()
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("每行三列：询问选项 / 对应回答 / 询问者回应（用 $ 分句，可用 img 或 img:xxx）",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(8);

            // 新增：名字输入
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Asset Name", GUILayout.Width(80));
                assetName = EditorGUILayout.TextField(assetName);
            }

            EditorGUILayout.HelpBox($"保存目录固定：{SAVE_DIR}", MessageType.Info);

            EditorGUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("+ Add Row", GUILayout.Width(120)))
                    rows.Add(new Row());

                if (GUILayout.Button("Clear", GUILayout.Width(120)))
                    rows.Clear();
            }

            EditorGUILayout.Space(8);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];

                EditorGUILayout.BeginVertical("box");
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField($"Row {i + 1}", GUILayout.Width(60));
                    if (GUILayout.Button("X", GUILayout.Width(22)))
                    {
                        rows.RemoveAt(i);
                        i--;
                        continue;
                    }
                }

                r.prompt = EditorGUILayout.TextField("询问选项", r.prompt);

                EditorGUILayout.LabelField("对应回答（$ 分句）");
                r.answerRaw = EditorGUILayout.TextArea(r.answerRaw, GUILayout.MinHeight(40));

                EditorGUILayout.LabelField("询问者回应（$ 分句）");
                r.replyRaw = EditorGUILayout.TextArea(r.replyRaw, GUILayout.MinHeight(40));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            bool canGenerate = rows.Count > 0 && !string.IsNullOrWhiteSpace(assetName);
            using (new EditorGUI.DisabledScope(!canGenerate))
            {
                if (GUILayout.Button("Generate ScriptableObject Asset", GUILayout.Height(36)))
                    GenerateAsset();
            }
        }

        private void GenerateAsset()
        {
            EnsureFoldersExist(SAVE_DIR);

            var safeName = SanitizeFileName(assetName);
            if (string.IsNullOrWhiteSpace(safeName))
            {
                Debug.LogError("[DialogueSheetBuilder] Invalid asset name.");
                return;
            }

            var rawPath = $"{SAVE_DIR}/{safeName}.asset";
            var path = AssetDatabase.GenerateUniqueAssetPath(rawPath); // 避免覆盖同名资产

            var asset = ScriptableObject.CreateInstance<DialogueSheetSO>();
            asset.entries = new List<DialogueEntry>();

            foreach (var r in rows)
            {
                if (string.IsNullOrWhiteSpace(r.prompt)) continue;

                var entry = new DialogueEntry
                {
                    prompt = r.prompt.Trim(),
                    answer = r.answerRaw,
                    reply = r.replyRaw,
                };

                asset.entries.Add(entry);
            }

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(asset);
            Selection.activeObject = asset;
            Debug.Log($"[DialogueSheetBuilder] Generated: {path} entries={asset.entries.Count}");
        }

        private static void EnsureFoldersExist(string assetPath)
        {
            // 例如 "Assets/Resources/Content/Dialogue"
            var parts = assetPath.Split('/');
            if (parts.Length < 2 || parts[0] != "Assets")
            {
                Debug.LogError($"[DialogueSheetBuilder] SAVE_DIR must start with 'Assets': {assetPath}");
                return;
            }

            string current = "Assets";
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        private static string SanitizeFileName(string name)
        {
            name = (name ?? "").Trim();

            // 替换非法字符为下划线
            var invalid = System.IO.Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
                name = name.Replace(c, '_');

            // 也可以顺手把空格换成下划线（看你喜好）
            name = name.Replace(' ', '_');

            return name;
        }

        private static List<DialogueSegment> ParseCell(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new List<DialogueSegment>();

            // 支持输入 \$ 表示字面量 $
            raw = raw.Replace("\\$", "__DOLLAR__");

            var parts = raw.Split('$')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.Replace("__DOLLAR__", "$"))
                .ToList();

            var segs = new List<DialogueSegment>(parts.Count);
            foreach (var p in parts)
            {
                // img 或 img:xxx
                if (p.Equals("img", StringComparison.OrdinalIgnoreCase))
                {
                    segs.Add(DialogueSegment.Image(""));
                }
                else if (p.StartsWith("img:", StringComparison.OrdinalIgnoreCase))
                {
                    var key = p.Substring(4).Trim();
                    segs.Add(DialogueSegment.Image(key));
                }
                else
                {
                    segs.Add(DialogueSegment.Text(p));
                }
            }

            return segs;
        }
    }
}