using GGJ2026;
using UnityEngine;
using UnityEngine.EventSystems;
using QFramework;
//using FMODUnity;
namespace GGJ2026
{
    public class Stage2ViewPort : MonoBehaviour, IDropHandler, IController
    {
        public IArchitecture GetArchitecture() => GameApp.Interface;

        // fmod 灯光开关事件
        public FMODUnity.EventReference LightOnEvent;
        FMOD.Studio.EventInstance LightOnEventInstance;
        FMOD.Studio.PARAMETER_ID SE_specialParameterId;

        public void OnDrop(PointerEventData eventData)
        {
            // eventData.pointerDrag 是当前正在被拖拽的那个物体
            if (eventData.pointerDrag != null)
            {
                Debug.Log($"{eventData.pointerDrag.name} 被丢入了 {gameObject.name}");
                // 触发你想要的处理函数
                HandleCandidate(eventData.pointerDrag);
                FMOD_LightOnEvent();
            }
        }
        void Start()
        {
            LightOnEventInstance = FMODUnity.RuntimeManager.CreateInstance(LightOnEvent);
            FMOD.Studio.EventDescription LightOnEventDescription;
            LightOnEventInstance.getDescription(out LightOnEventDescription);
            FMOD.Studio.PARAMETER_DESCRIPTION SE_specialParameterDescription;
            LightOnEventDescription.getParameterDescriptionByName("SE_special", out SE_specialParameterDescription);
            SE_specialParameterId = SE_specialParameterDescription.id;

            // example
            //FMOD.Studio.EventDescription healthEventDescription;
            //playerState.getDescription(out healthEventDescription);
            //FMOD.Studio.PARAMETER_DESCRIPTION healthParameterDescription;
            //healthEventDescription.getParameterDescriptionByName("health", out healthParameterDescription);
            //healthParameterId = healthParameterDescription.id;
        }
        void HandleCandidate(GameObject candidate)
        {
            // 在这里写你的逻辑，比如获取候选人的数据
            // var ctrl = candidate.GetComponent<Test_Script_Controller>();
            // ... 执行后续操作
            Debug.Log(candidate.GetComponent<Stage2DropButtonFProperty>().idx);
            GameApp.Interface.SendCommand(
                new SetStage2Selectedidx(candidate.GetComponent<Stage2DropButtonFProperty>().idx));
            candidate.GetComponent<UISilhouetteToggle>().SetSilhouette(false);

        }
        void FMOD_LightOnEvent()
        {
            switch (GameApp.Interface.GetModel<GameStateModel>().CurrentCaseId.Value)
            {
                case "Case_0001":
                    switch (GameApp.Interface.GetModel<UIStage_2_Model>().Selectedidx.Value)
                    {
                        case 0:
                            LightOnEventInstance.setParameterByID(SE_specialParameterId, 1);
                            break;
                        case 1:
                            LightOnEventInstance.setParameterByID(SE_specialParameterId, 2);
                            break;
                        case 2:
                            LightOnEventInstance.setParameterByID(SE_specialParameterId, 3);
                            break;
                    }
                    break;
                case "Case_0002":
                    switch (GameApp.Interface.GetModel<UIStage_2_Model>().Selectedidx.Value)
                    {
                        case 0:
                            LightOnEventInstance.setParameterByID(SE_specialParameterId, 0);
                            break;
                        case 1:
                            LightOnEventInstance.setParameterByID(SE_specialParameterId, 0);
                            break;
                        case 2:
                            LightOnEventInstance.setParameterByID(SE_specialParameterId, 0);
                            break;
                    }
                    break;
                case "Case_0003":
                    switch (GameApp.Interface.GetModel<UIStage_2_Model>().Selectedidx.Value)
                    {
                        case 0:
                        case 1:
                            LightOnEventInstance.setParameterByID(SE_specialParameterId, 0);
                            break;
                        case 2:
                            LightOnEventInstance.setParameterByID(SE_specialParameterId, 4);
                            break;
                    }
                    break;
                default:
                    break;
            }
            LightOnEventInstance.start();
        }
    }
}