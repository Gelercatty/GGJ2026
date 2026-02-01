
using UnityEngine;
using UnityEngine.EventSystems;

public class Stage1UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private RectTransform parentRect;
    private CanvasGroup canvasGroup;

    private Vector2 originalPosition;
    private Vector2 pointerOffset; // 记录鼠标抓取点偏移

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRect = rectTransform.parent as RectTransform;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();

        originalPosition = rectTransform.anchoredPosition;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // 鼠标屏幕坐标 -> 父节点本地坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out var localPointerPos);

        // 记录偏移：UI位置 - 鼠标在父节点的本地位置
        pointerOffset = rectTransform.anchoredPosition - localPointerPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out var localPointerPos);

        // 保持抓取点一致：鼠标位置 + 偏移
        rectTransform.anchoredPosition = localPointerPos + pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 如果需要：没放到目标上就回原位（你自己在 Drop 逻辑里决定）
        // rectTransform.anchoredPosition = originalPosition;
    }
}
