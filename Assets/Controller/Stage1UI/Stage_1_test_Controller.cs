using QFramework;
using TMPro;
using UnityEditor.UI;
using UnityEngine;

namespace GGJ2026
{
    public class Stage1Panel : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture() => GameApp.Interface;
        public TextMeshProUGUI _text;
        private void OnEnable()
        {
            this.RegisterEvent<OnRoundCaseSelectedEvent>(OnCaseSelected).UnRegisterWhenGameObjectDestroyed(gameObject);
            _text =  this.GetComponent<TextMeshProUGUI>();
        }

        private void OnCaseSelected(OnRoundCaseSelectedEvent e)
        {
            // 这里你可以：根据 e.CaseId 去拿 CasePackSO，填 UI 或初始化 Stage1Model
   
            var model = this.GetModel<UIStage_1_Model>();
            _text.text = model.ClueText.Value;
    
            
        }
    }
}