using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Diagnostics;
namespace GGJ2026
{
    public class FMODsoundcontroller : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture() => GameApp.Interface;
        public FMODUnity.EventReference MusicStateEvent;

        FMOD.Studio.EventInstance MusicState;
        FMOD.Studio.PARAMETER_ID sessionParameterId;//,SE_specialParameterId;

        public FMODUnity.EventReference winEvent;
        FMOD.Studio.EventInstance WinEventInstance;
        FMOD.Studio.PARAMETER_ID wincaseParameterId;

        public FMODUnity.EventReference loseEvent;
        void Start()
        {
            MusicState = FMODUnity.RuntimeManager.CreateInstance(MusicStateEvent);
            MusicState.start();
            FMOD.Studio.EventDescription musicDescription;
            MusicState.getDescription(out musicDescription);
            FMOD.Studio.PARAMETER_DESCRIPTION sessionParameterDescription;
            musicDescription.getParameterDescriptionByName("session", out sessionParameterDescription);
            sessionParameterId = sessionParameterDescription.id;

            WinEventInstance = FMODUnity.RuntimeManager.CreateInstance(winEvent);
            FMOD.Studio.EventDescription WinEventDescription;
            WinEventInstance.getDescription(out WinEventDescription);
            FMOD.Studio.PARAMETER_DESCRIPTION wincaseParameterDescription;
            WinEventDescription.getParameterDescriptionByName("case", out wincaseParameterDescription);
            wincaseParameterId = wincaseParameterDescription.id;
        }

        // Update is called once per frame
        void Update()
        {

        }
        void OnDestroy()
        {
            StopAllPlayerEvents();
            MusicState.release();
        }
        private void OnEnable()
        {
            var gameState = this.GetModel<GameStateModel>();
            gameState.Phase.RegisterWithInitValue(OnRoundChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
            var uiStage2Model = this.GetModel<UIStage_2_Model>();
            //uiStage2Model.IsLightOn.RegisterWithInitValue(OnLightOn).UnRegisterWhenGameObjectDestroyed(gameObject);
            //uiStage2Model.DialogueGiven.RegisterWithInitValue(OnDialogueGiven).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnRoundChanged(GamePhase phase)
        {
            if (phase == GamePhase.Win_stage2)
            {
                GameApp.Interface.GetModel<GameStateModel>().Round.Value++;
                switch (GameApp.Interface.GetModel<GameStateModel>().CurrentCaseId.Value)
                {
                    case "Case_0001":
                        WinEventInstance.setParameterByID(wincaseParameterId, 1);
                        break;
                    case "Case_0002":
                        WinEventInstance.setParameterByID(wincaseParameterId, 2);
                        break;
                    case "Case_0003":
                        WinEventInstance.setParameterByID(wincaseParameterId, 3);
                        break;
                    case "Case_0005":
                        WinEventInstance.setParameterByID(wincaseParameterId, 5);
                        break;
                    case "Case_0006":
                        WinEventInstance.setParameterByID(wincaseParameterId, 4);
                        break;
                    case "Case_0007":
                        MusicState.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        WinEventInstance.setParameterByID(wincaseParameterId, 6);
                        break;
                    default:
                        WinEventInstance.setParameterByID(wincaseParameterId, 0);
                        break;
                }
                WinEventInstance.start();
            }
            else if(phase == GamePhase.GameOver_2 || phase == GamePhase.GameOver_1)
            {
                FMODUnity.RuntimeManager.PlayOneShot(loseEvent, transform.position);
            }
            if (phase == GamePhase.GameOver_2 && GameApp.Interface.GetModel<GameStateModel>().Round.Value <= 2)
            {
                //GameApp.Interface.GetModel<GameStateModel>().Round.Value = 0;
            }
            var Round = GameApp.Interface.GetModel<GameStateModel>().Round.Value;
            UnityEngine.Debug.Log(Round);
            if (Round <= 2)
            {
                MusicState.setParameterByID(sessionParameterId, 0);
                //newClip = musicForSession1;
            }
            else if (Round <= 5)
            {
                MusicState.setParameterByID(sessionParameterId, 1);
                //newClip = musicForSession2;

            }
            else
            {
                MusicState.setParameterByID(sessionParameterId, 2);
                //newClip = musicForSession3;
            }


            //if (newClip != null && musicSource != null)
            //{
            //    ChangeAndPlayMusic(newClip);
            //}
        }

        private void OnLightOn()
        {
            var model = GameApp.Interface.GetModel<UIStage_2_Model>();
            var repo = GameApp.Interface.GetSystem<ICaseRepositorySystem>();
            string id = model.Selectedidx.Value.ToString();
            CasePackSO pack = repo.Get(id);
            var audioClip = pack.audioClip1;
            if (audioClip != null)
            {
                //ChangeAndPlaySFX(audioClip);
            }
            else
            {
                //ChangeAndPlaySFX(lightSwitchClip);
            }
        }
        void StopAllPlayerEvents()
        {
            FMOD.Studio.Bus playerBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            playerBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}