using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using GGJ2026;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class optionController : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture() => GameApp.Interface;
    //public Button Button_0;
    //public Button Button_1;
    //public Button Button_2;

    //public void Awake()
    //{
    //    this.RegisterEvent<>(e => {
    //        foreach (var button in Buttons)
    //        {
    //            button.GetComponent<UISilhouetteToggle>().SetSilhouette(true);
    //        }
    //    }).UnRegisterWhenGameObjectDestroyed(gameObject);

    //}

}
