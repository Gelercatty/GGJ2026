using System.Collections;
using System.Collections.Generic;
//using System.Runtime.InteropServices.Swift;
using UnityEngine;
using UnityEngine.UI;

public class splashacreen : MonoBehaviour
{
    public Image image2;
    public Button closeButton;
    
    private float timer = 0f;
    private const float DISPLAY_TIME = 30f;
    private bool isDisplayed = false;

    void Start()
    {
        if (image2 != null)
        {
            image2.gameObject.SetActive(false);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnClickClose);
            closeButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isDisplayed)
        {
            timer += Time.deltaTime;
            
            if (timer >= DISPLAY_TIME)
            {
                ShowImage2();
            }
        }
    }
    
    private void ShowImage2()
    {
        isDisplayed = true;
        
        if (image2 != null)
        {
            image2.gameObject.SetActive(true);
        }
        
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(true);
        }
    }
    
    private void OnClickClose()
    {
        if (image2 != null)
        {
            Destroy(image2.gameObject);
        }
        
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(false);
        }
    }
}
