using UnityEngine;
using UnityEngine.EventSystems; //

public class Stage1UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // �������ť�Ӹ� Canvas Group �������קʱ���Ա��͸��
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f; // ��קʱ��͸��
        canvasGroup.blocksRaycasts = false; // �������ߴ����Լ����������ܼ�⵽�·��ġ�����Ŀ�ꡱ
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �� UI �������/��ָ�ƶ�
        rectTransform.anchoredPosition += eventData.delta / transform.lossyScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // �ָ���͸��
        canvasGroup.blocksRaycasts = true; // �ָ����߼��

        // ���û�гɹ��ŵ�Ŀ�����򣬿��Կ�����������ԭλ
        // rectTransform.anchoredPosition = originalPosition; 
    }
}
