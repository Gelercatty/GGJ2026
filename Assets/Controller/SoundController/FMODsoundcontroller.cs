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
    // Start is called before the first frame update
    //--------------------------------------------------------------------
    // 1: Using the EventReference type will present the designer with
    //    the UI for selecting events.
    //--------------------------------------------------------------------
    //public FMODUnity.EventReference PlayerStateEvent;
    public FMODUnity.EventReference MusicStateEvent;
    //--------------------------------------------------------------------
    // 2: Using the EventInstance class will allow us to manage an event
    //    over its lifetime, including starting, stopping and changing 
    //    parameters.
    //--------------------------------------------------------------------
    FMOD.Studio.EventInstance MusicState;

    //--------------------------------------------------------------------
    // 3: These two events represent one-shot sounds. They are sounds that 
    //    have a finite length. We do not store an EventInstance to
    //    manage the sounds. Once started they will play to completion and
    //    then all resources will be released.
    //--------------------------------------------------------------------
    //public FMODUnity.EventReference DamageEvent;
    //public FMODUnity.EventReference HealEvent;
    

    //--------------------------------------------------------------------
    //    This event is also one-shot, but we want to track its state and
    //    take action when it ends. We could also change parameter
    //    values over the lifetime.
    //--------------------------------------------------------------------
    //public FMODUnity.EventReference PlayerIntroEvent;
    //FMOD.Studio.EventInstance playerIntro;

    //public int StartingHealth = 100;
    //int health;
    //FMOD.Studio.PARAMETER_ID healthParameterId, fullHealthParameterId;
    FMOD.Studio.PARAMETER_ID sessionParameterId;//,SE_specialParameterId;

    //Rigidbody cachedRigidBody;
    void Start()
    {
         MusicState = FMODUnity.RuntimeManager.CreateInstance(MusicStateEvent);
         MusicState.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     void OnDestroy() {
        StopAllPlayerEvents();

        //--------------------------------------------------------------------
        // 6: This shows how to release resources when the Unity object is 
        //    disabled.
        //--------------------------------------------------------------------
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
            AudioClip newClip = null;

            if(phase == GamePhase.Win_stage2)
            {
                GameApp.Interface.GetModel<GameStateModel>().Round.Value++;
            }
            if(phase == GamePhase.GameOver_2 && GameApp.Interface.GetModel<GameStateModel>().Round.Value <= 2 )
            {
                GameApp.Interface.GetModel<GameStateModel>().Round.Value = 0;
            }
            var Round = GameApp.Interface.GetModel<GameStateModel>().Round.Value;
            UnityEngine.Debug.Log(Round);
            if(Round <= 2)
            {
                //newClip = musicForSession1;
            }
            else if(Round <= 5)
            {
                //newClip = musicForSession2;
            }
            else
            {
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
             if(audioClip != null)
             {
                 //ChangeAndPlaySFX(audioClip);
             }else{
                 //ChangeAndPlaySFX(lightSwitchClip);
             }
        }
        private void OnDialogueGiven()
        {
            //根据DialogueGraphId播放对应音效
            var model = GameApp.Interface.GetModel<UIStage_2_Model>();
            var repo =  GameApp.Interface.GetSystem<ICaseRepositorySystem>();
           CasePackSO pack = repo.Get(model.Selectedidx.Value.ToString());
                if(pack != null){
                   // ChangeAndPlaySFX(pack.audioClip2);
                }
        }

        void StopAllPlayerEvents()
    {
        FMOD.Studio.Bus playerBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        playerBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
}