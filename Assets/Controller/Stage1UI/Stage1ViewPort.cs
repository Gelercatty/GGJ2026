using GGJ2026;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stage1ViewPort : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // eventData.pointerDrag 是当前正在被拖拽的那个物体
        if (eventData.pointerDrag != null)
        {
            Debug.Log($"{eventData.pointerDrag.name} 被丢入了 {gameObject.name}");
            // 触发你想要的处理函数
            HandleCandidate(eventData.pointerDrag);
        }
    }

    void HandleCandidate(GameObject candidate)
    {
        // 在这里写你的逻辑，比如获取候选人的数据
        // var ctrl = candidate.GetComponent<Test_Script_Controller>();
        // ... 执行后续操作
        Debug.Log(candidate.GetComponent<Stage1DropButtonFProperty>().CaseId);
        GameApp.Interface.SendCommand(new SetStage1SelectedCaseID(candidate.GetComponent<Stage1DropButtonFProperty>().CaseId));
    }
}