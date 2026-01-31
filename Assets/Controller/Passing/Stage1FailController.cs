
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026
{
    public class Stage1FailController : MonoBehaviour
    {
        public CanvasGroup cg;
        public Button restartButton;

        private void Awake()
        {
            if (!cg) cg = GetComponent<CanvasGroup>();
            restartButton.onClick.AddListener(() =>
            {
                GameApp.Interface.SendCommand(new StartNewGameCommand());
            });
        }

        private void OnEnable()
        {
            var game = GameApp.Interface.GetModel<GameStateModel>();
            game.Phase.RegisterWithInitValue(phase =>
            {
                SetVisible(phase == GamePhase.GameOver_1);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void SetVisible(bool on)
        {
            Debug.Log("fail"+on);
            cg.alpha = on ? 1f : 0f;
            cg.blocksRaycasts = on;     
            cg.interactable = on;       
        }
    }
}
