using UnityEngine;
using UnityEngine.UI;

public class ImageRevealAnimationExample : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private ImageRevealAnimation revealAnimation;
    [SerializeField] private Button revealButton;
    [SerializeField] private Button hideButton;
    [SerializeField] private Button resetButton;

    void Start()
    {
        if (revealButton != null)
        {
            revealButton.onClick.AddListener(OnRevealButtonClick);
        }

        if (hideButton != null)
        {
            hideButton.onClick.AddListener(OnHideButtonClick);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetButtonClick);
        }
    }

    void OnDestroy()
    {
        if (revealButton != null)
        {
            revealButton.onClick.RemoveListener(OnRevealButtonClick);
        }

        if (hideButton != null)
        {
            hideButton.onClick.RemoveListener(OnHideButtonClick);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(OnResetButtonClick);
        }
    }

    public void OnRevealButtonClick()
    {
        if (revealAnimation != null)
        {
            revealAnimation.TriggerRevealAnimation();
        }
    }

    public void OnHideButtonClick()
    {
        if (revealAnimation != null)
        {
            revealAnimation.TriggerHideAnimation();
        }
    }

    public void OnResetButtonClick()
    {
        if (revealAnimation != null)
        {
            revealAnimation.ResetToInitialState();
        }
    }
}
