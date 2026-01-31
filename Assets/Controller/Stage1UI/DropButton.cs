using UnityEngine;
using UnityEngine.EventSystems; // 必须引用事件系统

public class Stage1UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // 建议给按钮加个 Canvas Group 组件，拖拽时可以变半透明
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f; // 拖拽时变透明
        canvasGroup.blocksRaycasts = false; // 允许射线穿过自己，这样才能检测到下方的“放置目标”
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 让 UI 跟随鼠标/手指移动
        rectTransform.anchoredPosition += eventData.delta / transform.lossyScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // 恢复不透明
        canvasGroup.blocksRaycasts = true; // 恢复射线检测

        // 如果没有成功放到目标区域，可以考虑让它弹回原位
        // rectTransform.anchoredPosition = originalPosition; 
    }
}