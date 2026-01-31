using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using TMPro;
using GGJ2026;
using System;

public class CluePanelController : MonoBehaviour, IController
{
    public TextMeshProUGUI textMeshProUGUI;
    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        this.RegisterEvent<LoadStage1UI>(OnLoadStage1UI).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnLoadStage1UI(LoadStage1UI uI)
    {
        textMeshProUGUI.text = GameApp.Interface.GetModel<UIStage_1_Model>().ClueText.Value;
    }

   

    public IArchitecture GetArchitecture() => GameApp.Interface;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
