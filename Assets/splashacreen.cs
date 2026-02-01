using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class splashscreen : MonoBehaviour
{

    public GameObject splashImage;
     public GameObject splashImage1;
      public GameObject splashImage2;
    
    private float timer = 0f;
    private const float DISABLE_TIME = 3f;
    private const float DISABLE_TIME1 = 0.5f;
    private const float DISABLE_TIME2 = 1.5f;
    //private bool isDisabled = false;
    
    void Start()
    {
        //splashImage = GetComponent<GameObject>("");
    }

    void Update()
    {
        
            timer += Time.deltaTime;
            
            if (timer >= DISABLE_TIME)
            {
                DisableSplashImage(splashImage);
            }
            if (timer >= DISABLE_TIME1)
            {
                DisableSplashImage(splashImage1);
            }
            if (timer >= DISABLE_TIME2)
            {
                EnableSplashImage(splashImage2);
            }

    }
    
    private void DisableSplashImage(GameObject splashImage)
    {
        //isDisabled = true;
        
        if (splashImage != null)
        {
            splashImage.SetActive(false);
        }
    }
    private void EnableSplashImage(GameObject splashImage)
    {
        //isDisabled = false;
        
        if (splashImage != null)
        {
            splashImage.SetActive(true);
        }
    }
   
}
