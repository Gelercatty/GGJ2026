using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026
{
    public class Stage1CandidateButtonItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;

        [SerializeField] private Image icon;

        private string _caseId;
        private Action<string> _onClick;

        public void Init(string caseId, string displayText, Sprite iconSprite, Action<string> onClick)
        {
            _caseId = caseId;
            _onClick = onClick;

            if (label) label.text = displayText;

            if (icon)
            {
                icon.sprite = iconSprite;
                icon.enabled = iconSprite != null;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => _onClick?.Invoke(_caseId));
        }
    }
}

