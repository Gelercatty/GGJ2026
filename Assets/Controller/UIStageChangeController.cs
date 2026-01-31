using QFramework;
using UnityEngine;
namespace GGJ2026
{
    public class UIStageChangeController : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture() => GameApp.Interface;
        public GameObject State1Ref;
        public GameObject State2Ref;
        void Awake()
        {
            var game = GameApp.Interface.GetModel<GameStateModel>();
            game.Phase.RegisterWithInitValue(phase =>
            {
                OnPhaseChanged(phase);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        void OnPhaseChanged(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.Stage1:
                    State1Ref.SetActive(true);
                    State2Ref.SetActive(false);
                    break;
                case GamePhase.Stage2:
                    State1Ref.SetActive(false);
                    State2Ref.SetActive(true);
                    break;
                default:
                    break;
            }

        }
    }
}