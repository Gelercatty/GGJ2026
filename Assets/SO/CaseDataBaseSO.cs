using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026
{
    [CreateAssetMenu(menuName = "GGJ2026/Case Database", fileName = "CaseDatabase")]
    public class CaseDatabaseSO : ScriptableObject
    {
        // 只要这里引用了 CasePackSO，它们就会作为依赖一起打包进游戏
        public List<CasePackSO> cases = new();
    }
}