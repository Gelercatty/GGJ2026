using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ImageRevealAnimation : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private RectTransform maskRect;
    [SerializeField] private Image targetImage;

    [Header("动画设置")]
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private EasingType easingType = EasingType.EaseOutCubic;
    [SerializeField] private bool startRevealed = false;

    [Header("初始显示区域设置")]
    [SerializeField] private Vector2 initialSize = new Vector2(100, 100);
    [SerializeField] private Vector2 initialPosition = Vector2.zero;

    [Header("事件")]
    public UnityEvent OnRevealStart;
    public UnityEvent OnRevealComplete;


    private Vector2 targetSize;
    private Vector2 targetPosition;
    private bool isAnimating = false;
    private bool isRevealed = false;

    public enum EasingType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic
    }

    void Awake()
    {
        if (targetImage != null)
        {
            targetSize = targetImage.rectTransform.sizeDelta;
            targetPosition = targetImage.rectTransform.anchoredPosition;
        }
    }

    void Start()
    {
        if (!startRevealed)
        {
            SetInitialState();
            ResetToInitialState();
            TriggerRevealAnimation();
        }
    }

    public void TriggerRevealAnimation()
    {
        if (!isAnimating && !isRevealed)
        {
            StartCoroutine(RevealCoroutine());
        }
    }

    public void TriggerHideAnimation()
    {
        if (!isAnimating && isRevealed)
        {
            StartCoroutine(HideCoroutine());
        }
    }

    public void ResetToInitialState()
    {
        StopAllCoroutines();
        isAnimating = false;
        isRevealed = false;
        SetInitialState();
    }

    private void SetInitialState()
    {
        if (maskRect != null)
        {
            maskRect.sizeDelta = initialSize;
            maskRect.anchoredPosition = initialPosition;
        }
    }

    private System.Collections.IEnumerator RevealCoroutine()
    {
        isAnimating = true;
        OnRevealStart?.Invoke();

        float elapsed = 0f;
        Vector2 startSize = maskRect.sizeDelta;
        Vector2 startPosition = maskRect.anchoredPosition;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / animationDuration);
            float easedProgress = ApplyEasing(progress);

            maskRect.sizeDelta = Vector2.Lerp(startSize, targetSize, easedProgress);
            maskRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedProgress);

            yield return null;
        }

        maskRect.sizeDelta = targetSize;
        maskRect.anchoredPosition = targetPosition;
        isAnimating = false;
        isRevealed = true;
        OnRevealComplete?.Invoke();
    }

    private System.Collections.IEnumerator HideCoroutine()
    {
        isAnimating = false;

        float elapsed = 0f;
        Vector2 startSize = maskRect.sizeDelta;
        Vector2 startPosition = maskRect.anchoredPosition;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / animationDuration);
            float easedProgress = ApplyEasing(1f - progress);

            maskRect.sizeDelta = Vector2.Lerp(startSize, initialSize, easedProgress);
            maskRect.anchoredPosition = Vector2.Lerp(startPosition, initialPosition, easedProgress);

            yield return null;
        }

        maskRect.sizeDelta = initialSize;
        maskRect.anchoredPosition = initialPosition;
        isAnimating = false;
        isRevealed = false;
    }

    private float ApplyEasing(float t)
    {
        switch (easingType)
        {
            case EasingType.Linear:
                return t;
            case EasingType.EaseInQuad:
                return t * t;
            case EasingType.EaseOutQuad:
                return t * (2f - t);
            case EasingType.EaseInOutQuad:
                return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
            case EasingType.EaseInCubic:
                return t * t * t;
            case EasingType.EaseOutCubic:
                return 1f - Mathf.Pow(1f - t, 3f);
            case EasingType.EaseInOutCubic:
                return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
            default:
                return t;
        }
    }

    public void SetAnimationDuration(float duration)
    {
        animationDuration = Mathf.Max(0.1f, duration);
    }

    public void SetEasingType(EasingType type)
    {
        easingType = type;
    }
}
