using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using TMPro;
using GGJ2026;
using UnityEngine.EventSystems;
using System;

public class CluePanelController : MonoBehaviour, IController,IPointerClickHandler
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

   public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshProUGUI, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textMeshProUGUI.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();
            Debug.Log("点击了链接: " + linkId);
            
            // 发送 Command 处理超链接点击
            this.SendCommand(new HandleHyperlinkClickCommand(linkId));
        }
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
