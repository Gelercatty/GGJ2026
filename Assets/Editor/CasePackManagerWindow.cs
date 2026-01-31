#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GGJ2026
{

public class CasePackManagerWindow : EditorWindow
{
    private const string PrefRootFolder = "CasePackManager.RootFolder";
    private const string DefaultRootFolder = "Assets/Content/Cases";

    private string _rootFolder;
    private string _search = "";
    private Vector2 _leftScroll;
    private Vector2 _rightScroll;

    private List<CasePackSO> _cases = new();
    private CasePackSO _selected;
    
    private const string DbAssetPath = "Assets/Resources/Content/CaseDatabase.asset";
    private CaseDatabaseSO _db;
    private bool _autoSyncDb = true; // 可选：是否在 RefreshList 后自动同步

    // 校验缓存
    private readonly Dictionary<CasePackSO, List<string>> _validation = new();

    // —— 导入预留：你以后可以把解析器挂到这个 delegate 上 ——
    // 返回值：true 表示导入成功（创建/更新了 CasePackSO）
    public static Func<string, bool> CustomImportHandler;

    [MenuItem("Tools/CasePack Manager")]
    public static void Open()
    {
        var win = GetWindow<CasePackManagerWindow>("CasePack Manager");
        win.minSize = new Vector2(980, 560);
        win.Show();
    }

    private void OnEnable()
    {
        _rootFolder = EditorPrefs.GetString(PrefRootFolder, DefaultRootFolder);
        _db = GetOrCreateDatabase(); 
        RefreshList();
    }

    private void OnDisable()
    {
        EditorPrefs.SetString(PrefRootFolder, _rootFolder);
    }

    private void OnGUI()
    {
        DrawToolbar();

        EditorGUILayout.BeginHorizontal();
        DrawLeftList();
        DrawRightEditor();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Root Folder", GUILayout.Width(80));
            _rootFolder = EditorGUILayout.TextField(_rootFolder);

            if (GUILayout.Button("Select...", GUILayout.Width(90)))
            {
                var abs = EditorUtility.OpenFolderPanel("Select Root Folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(abs))
                {
                    var rel = FileUtil.GetProjectRelativePath(abs);
                    if (!string.IsNullOrEmpty(rel))
                        _rootFolder = rel;
                    else
                        Debug.LogWarning("请选择项目 Assets 目录内的文件夹。");
                }
            }

            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
                RefreshList();

            GUILayout.FlexibleSpace();

            _search = EditorGUILayout.TextField("Search", _search, GUILayout.Width(260));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("New Case", GUILayout.Width(120)))
                CreateNewCase();

            if (GUILayout.Button("Duplicate", GUILayout.Width(120)))
                DuplicateSelected();

            if (GUILayout.Button("Validate All", GUILayout.Width(120)))
                ValidateAll();

            if (GUILayout.Button("Save All", GUILayout.Width(120)))
                SaveAll();

           

            if (GUILayout.Button("Import...", GUILayout.Width(120)))
                ImportCasesEntry();
            _autoSyncDb = GUILayout.Toggle(_autoSyncDb, "Auto Sync DB", GUILayout.Width(110));

            if (GUILayout.Button("Sync Database", GUILayout.Width(140)))
                SyncDatabaseFromRootFolder();

            if (GUILayout.Button("Ping DB", GUILayout.Width(90)))
            {
                _db = GetOrCreateDatabase();
                EditorGUIUtility.PingObject(_db);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawLeftList()
    {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(360)))
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField($"Cases ({_cases.Count})", EditorStyles.boldLabel);

            _leftScroll = EditorGUILayout.BeginScrollView(_leftScroll);

            foreach (var c in FilteredCases())
            {
                var title = string.IsNullOrEmpty(c.caseId) ? c.name : c.caseId;
                var status = GetStatusIcon(c, out var tooltip);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(status, GUILayout.Width(18));

                    var isSelected = _selected == c;
                    var style = new GUIStyle(EditorStyles.label);
                    if (isSelected) style.fontStyle = FontStyle.Bold;

                    if (GUILayout.Button(new GUIContent(title, tooltip), style))
                    {
                        _selected = c;
                        GUI.FocusControl(null);
                    }

                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                        EditorGUIUtility.PingObject(c);
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawRightEditor()
    {
        using (new EditorGUILayout.VerticalScope())
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            if (_selected == null)
            {
                EditorGUILayout.LabelField("选择左侧一个 Case 进行编辑。", EditorStyles.wordWrappedLabel);
                return;
            }

            _rightScroll = EditorGUILayout.BeginScrollView(_rightScroll);

            EditorGUILayout.LabelField("Selected Case", EditorStyles.boldLabel);
            EditorGUILayout.ObjectField("Asset", _selected, typeof(CasePackSO), false);

            EditorGUI.BeginChangeCheck();

            // Identity
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            _selected.caseId = EditorGUILayout.TextField("Case ID", _selected.caseId);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Auto ID", GUILayout.Width(100)))
                    _selected.caseId = GenerateNextCaseId();

                if (GUILayout.Button("Validate", GUILayout.Width(100)))
                    ValidateOne(_selected);

                GUILayout.FlexibleSpace();
            }

            // Stage1
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Stage 1", EditorStyles.boldLabel);
            _selected.silhouette = (Sprite)EditorGUILayout.ObjectField("Silhouette", _selected.silhouette, typeof(Sprite), false);

            EditorGUILayout.LabelField("Clue Text");
            _selected.stage1ClueText = EditorGUILayout.TextArea(_selected.stage1ClueText, GUILayout.MinHeight(60));

            // Stage2
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Stage 2", EditorStyles.boldLabel);

            EnsureArraySize3(_selected);

            EditorGUILayout.LabelField("Prison Shots (3)");
            for (int i = 0; i < 3; i++)
            {
                _selected.prisonShots[i] = (Sprite)EditorGUILayout.ObjectField($"Prison[{i}]", _selected.prisonShots[i], typeof(Sprite), false);
            }

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Dialogues Placeholder (3)");
            for (int i = 0; i < 3; i++)
            {
                _selected.stage2Dialogues[i] = (DialogueSheetSO)EditorGUILayout.ObjectField(
                    $"Dialogue[{i}]",
                    _selected.stage2Dialogues[i],
                    typeof(DialogueSheetSO),
                    false
                );
            }
            _selected.correctIndex = EditorGUILayout.IntSlider("Correct Index", _selected.correctIndex, 0, 2);

            // Clue Links
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Clue Links", EditorStyles.boldLabel);
            _selected.hyperlinkId = EditorGUILayout.TextField("Hyperlink ID", _selected.hyperlinkId);
            EditorGUILayout.LabelField("Hyperlink Clue Text");
            _selected.hyperlinkClueText = EditorGUILayout.TextArea(_selected.hyperlinkClueText, GUILayout.MinHeight(60));

            // Validation panel
            EditorGUILayout.Space(12);
            DrawValidationPanel(_selected);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_selected);
            }

            EditorGUILayout.Space(12);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save", GUILayout.Height(28)))
                    SaveSelected();

                if (GUILayout.Button("Open Containing Folder", GUILayout.Height(28)))
                    OpenContainingFolder(_selected);

                if (GUILayout.Button("Delete", GUILayout.Height(28)))
                    DeleteSelected();

                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private IEnumerable<CasePackSO> FilteredCases()
    {
        if (string.IsNullOrWhiteSpace(_search))
            return _cases;

        var s = _search.Trim().ToLowerInvariant();
        return _cases.Where(c =>
            (!string.IsNullOrEmpty(c.caseId) && c.caseId.ToLowerInvariant().Contains(s)) ||
            c.name.ToLowerInvariant().Contains(s));
    }

    private GUIContent GetStatusIcon(CasePackSO c, out string tooltip)
    {
        if (!_validation.TryGetValue(c, out var errs))
        {
            tooltip = "未校验";
            return new GUIContent("•", tooltip);
        }

        if (errs.Count == 0)
        {
            tooltip = "OK";
            return new GUIContent("✅", tooltip);
        }

        // 这里简单处理：有错误就 ❌；你也可以按“警告/致命”分级
        tooltip = string.Join("\n", errs);
        return new GUIContent("❌", tooltip);
    }

    private void DrawValidationPanel(CasePackSO c)
    {
        EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

        if (!_validation.TryGetValue(c, out var errs))
        {
            EditorGUILayout.HelpBox("未校验。点击上方 Validate 或工具栏 Validate All。", MessageType.Info);
            return;
        }

        if (errs.Count == 0)
        {
            EditorGUILayout.HelpBox("OK ✅", MessageType.Info);
            return;
        }

        EditorGUILayout.HelpBox(string.Join("\n", errs), MessageType.Error);
    }

    private void RefreshList()
    {
        _cases.Clear();
        _validation.Clear();

        // 扫描项目中的所有 CasePackSO
        var guids = AssetDatabase.FindAssets("t:CasePackSO");
        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            var asset = AssetDatabase.LoadAssetAtPath<CasePackSO>(path);
            if (asset != null) _cases.Add(asset);
        }

        // 排序：有 caseId 的按 caseId，否则按 name
        _cases = _cases
            .OrderBy(c => string.IsNullOrEmpty(c.caseId) ? "ZZZ_" + c.name : c.caseId)
            .ToList();

        if (_selected == null && _cases.Count > 0) _selected = _cases[0];

        Repaint();
    }

    private void CreateNewCase()
    {
        EnsureFolderExists(_rootFolder);

        var caseAsset = CreateInstance<CasePackSO>();
        caseAsset.caseId = GenerateNextCaseId();
        caseAsset.name = caseAsset.caseId;

        EnsureArraySize3(caseAsset);

        var path = AssetDatabase.GenerateUniqueAssetPath($"{_rootFolder}/{caseAsset.caseId}.asset");
        AssetDatabase.CreateAsset(caseAsset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RefreshList();
        _selected = caseAsset;
        EditorGUIUtility.PingObject(caseAsset);
    }

    private void DuplicateSelected()
    {
        if (_selected == null) return;

        EnsureFolderExists(_rootFolder);

        var srcPath = AssetDatabase.GetAssetPath(_selected);
        var newPath = AssetDatabase.GenerateUniqueAssetPath($"{_rootFolder}/{_selected.caseId}_Copy.asset");
        AssetDatabase.CopyAsset(srcPath, newPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var duplicated = AssetDatabase.LoadAssetAtPath<CasePackSO>(newPath);
        if (duplicated != null)
        {
            duplicated.caseId = GenerateNextCaseId();
            duplicated.name = duplicated.caseId;
            EditorUtility.SetDirty(duplicated);
            AssetDatabase.SaveAssets();
        }

        RefreshList();
        _selected = duplicated;
        EditorGUIUtility.PingObject(duplicated);
    }

    private void DeleteSelected()
    {
        if (_selected == null) return;

        var path = AssetDatabase.GetAssetPath(_selected);
        if (EditorUtility.DisplayDialog("Delete CasePack",
            $"确定删除：{_selected.caseId} ?\n\n{path}", "Delete", "Cancel"))
        {
            var ok = AssetDatabase.DeleteAsset(path);
            if (!ok) Debug.LogWarning("删除失败：" + path);

            _selected = null;
            RefreshList();
        }
    }

    private void SaveSelected()
    {
        if (_selected == null) return;
        EditorUtility.SetDirty(_selected);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void SaveAll()
    {
        foreach (var c in _cases)
            EditorUtility.SetDirty(c);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ValidateAll()
    {
        _validation.Clear();

        // 基础校验 + caseId 唯一性校验
        var idMap = new Dictionary<string, List<CasePackSO>>();

        foreach (var c in _cases)
        {
            var errs = c.ValidateBasic();
            _validation[c] = errs;

            if (!string.IsNullOrWhiteSpace(c.caseId))
            {
                if (!idMap.TryGetValue(c.caseId, out var list))
                    idMap[c.caseId] = list = new List<CasePackSO>();
                list.Add(c);
            }
        }

        foreach (var kv in idMap)
        {
            if (kv.Value.Count > 1)
            {
                foreach (var c in kv.Value)
                    _validation[c].Add($"caseId 重复：{kv.Key}");
            }
        }

        Repaint();
    }

    private void ValidateOne(CasePackSO c)
    {
        if (c == null) return;

        var errs = c.ValidateBasic();

        // 校验唯一性（只在当前列表中检查）
        if (!string.IsNullOrWhiteSpace(c.caseId))
        {
            var dup = _cases.Count(x => x != c && x.caseId == c.caseId);
            if (dup > 0) errs.Add($"caseId 重复：{c.caseId}");
        }

        _validation[c] = errs;
        Repaint();
    }

    private void OpenContainingFolder(UnityEngine.Object obj)
    {
        var path = AssetDatabase.GetAssetPath(obj);
        if (string.IsNullOrEmpty(path)) return;

        var abs = Path.GetFullPath(path);
        var dir = Path.GetDirectoryName(abs);
        if (!string.IsNullOrEmpty(dir))
            EditorUtility.RevealInFinder(dir);
    }

    private string GenerateNextCaseId()
    {
        // Case_0001, Case_0002...
        int max = 0;
        foreach (var c in _cases)
        {
            if (string.IsNullOrWhiteSpace(c.caseId)) continue;
            if (c.caseId.StartsWith("Case_", StringComparison.OrdinalIgnoreCase))
            {
                var tail = c.caseId.Substring("Case_".Length);
                if (int.TryParse(tail, out var n))
                    max = Mathf.Max(max, n);
            }
        }
        return $"Case_{(max + 1):0000}";
    }

    private void EnsureFolderExists(string folder)
    {
        if (AssetDatabase.IsValidFolder(folder)) return;

        // 逐级创建 Assets/...
        var parts = folder.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || parts[0] != "Assets")
            throw new Exception("RootFolder 必须在 Assets 下，例如 Assets/Content/Cases");

        var current = "Assets";
        for (int i = 1; i < parts.Length; i++)
        {
            var next = $"{current}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private void EnsureArraySize3(CasePackSO c)
    {
        if (c.prisonShots == null || c.prisonShots.Length != 3)
            c.prisonShots = ResizeTo3(c.prisonShots);

        if (c.stage2Dialogues == null || c.stage2Dialogues.Length != 3)
            c.stage2Dialogues = ResizeTo3(c.stage2Dialogues);

        static T[] ResizeTo3<T>(T[] src)
        {
            var arr = new T[3];
            if (src == null) return arr;
            for (int i = 0; i < Mathf.Min(3, src.Length); i++) arr[i] = src[i];
            return arr;
        }
    }

    // =========================
    // 导入入口（预留解析器位置）
    // =========================
    private void ImportCasesEntry()
    {
        // 选择一个目录作为导入源（外部/项目内都行）
        var abs = EditorUtility.OpenFolderPanel("Import Cases From Folder", Application.dataPath, "");
        if (string.IsNullOrEmpty(abs)) return;

        // 预留：如果你写了自定义导入器，就走自定义
        if (CustomImportHandler != null)
        {
            var ok = CustomImportHandler.Invoke(abs);
            Debug.Log(ok ? "[CasePackManager] Custom import finished." : "[CasePackManager] Custom import failed.");
            RefreshList();
            return;
        }

        // 默认导入：只留一个 hook，不做任何解析
        // 你以后可以在这里调用你的解析器：扫描 abs 下的 Case_XXXX 目录、解析 json/txt、创建/更新 CasePackSO 等
        HandleImportFromFolder(abs);

        RefreshList();
    }

    /// <summary>
    /// 预留的导入处理函数（未来把解析器写到这里，或从这里调用外部 Importer）。
    /// </summary>
    private void HandleImportFromFolder(string absoluteFolderPath)
    {
        // TODO:
        // 1) 扫描 absoluteFolderPath 下的子目录
        // 2) 识别每条 Case 的资源：silhouette.png, suspect_1.png... clue.txt, dialogue_x.json 等
        // 3) 如果项目内已有同 caseId 的 CasePackSO -> 更新引用；没有 -> 创建新 SO
        // 4) 校验并输出报告（缺文件、命名不规范等）

        Debug.Log($"[CasePackManager] Import placeholder called. Folder: {absoluteFolderPath}\n" +
                  $"你可以在 HandleImportFromFolder() 里接入解析器。");
    }
    private CaseDatabaseSO GetOrCreateDatabase()
{
    var db = AssetDatabase.LoadAssetAtPath<CaseDatabaseSO>(DbAssetPath);
    if (db != null) return db;

    EnsureFolderExists("Assets/Resources");
    EnsureFolderExists("Assets/Resources/Content");

    db = ScriptableObject.CreateInstance<CaseDatabaseSO>();
    AssetDatabase.CreateAsset(db, DbAssetPath);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    return db;
}

private void SyncDatabaseFromRootFolder()
{
    if (string.IsNullOrWhiteSpace(_rootFolder) || !AssetDatabase.IsValidFolder(_rootFolder))
    {
        Debug.LogWarning($"[CasePackManager] RootFolder 无效：{_rootFolder}");
        return;
    }

    _db = GetOrCreateDatabase();

    // 只同步 rootFolder 下的 CasePackSO
    var guids = AssetDatabase.FindAssets("t:CasePackSO", new[] { _rootFolder });

    var found = new List<CasePackSO>();
    foreach (var g in guids)
    {
        var path = AssetDatabase.GUIDToAssetPath(g);
        var asset = AssetDatabase.LoadAssetAtPath<CasePackSO>(path);
        if (asset != null) found.Add(asset);
    }

    // 基础校验：caseId 非空 & 唯一
    var errors = new List<string>();
    var map = new Dictionary<string, CasePackSO>();

    foreach (var c in found)
    {
        if (string.IsNullOrWhiteSpace(c.caseId))
        {
            errors.Add($"caseId 为空：{AssetDatabase.GetAssetPath(c)}");
            continue;
        }

        if (map.ContainsKey(c.caseId))
        {
            errors.Add(
                $"caseId 重复：{c.caseId}\n" +
                $"  A: {AssetDatabase.GetAssetPath(map[c.caseId])}\n" +
                $"  B: {AssetDatabase.GetAssetPath(c)}"
            );
            continue;
        }

        map[c.caseId] = c;
    }

    // 写入数据库（排序保证稳定）
    _db.cases = map.Values.OrderBy(x => x.caseId).ToList();

    EditorUtility.SetDirty(_db);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    if (errors.Count > 0)
    {
        Debug.LogError("[CasePackManager] Sync Database 完成，但有错误：\n" + string.Join("\n", errors));
    }
    else
    {
        Debug.Log($"[CasePackManager] Sync Database OK：{_db.cases.Count} cases -> {DbAssetPath}");
    }
}

}
}
#endif
