using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ��ʾ�ؿ�ѡ�����
/// </summary>
public class PanelTip : BasePanel, IPointerUpHandler
{

    private bool onShow;
    private RectTransform rt;
    private float duration; //�Ѿ�������ʱ��
    private float showTime = 0.3f; //չʾ��ʱ��

    public override void HideMe()
    {

    }

    //չʾ��ʱ�򣬻����С����
    public override void ShowMe()
    {
        onShow = true;
        rt = GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;
        duration = 0;
        ShowBlock(0);
        base.GetNpc(transform.name);
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (onShow)
        {
            rt.localScale = Vector3.one * Mathf.Lerp(0, 1, duration / showTime);
        }
        duration += Time.deltaTime;
    }

    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "Button1":
                MusicMgr.Instance.PlaySound("Yes Button");
                Level.Instance.EnterLevel(0, 0);
                UIMgr.Instance.HidePanel<PanelTip>(true);
                EnterLevel();
                break;
            case "Button2":
                MusicMgr.Instance.PlaySound("Yes Button");
                Level.Instance.EnterLevel(0, 1);
                UIMgr.Instance.HidePanel<PanelTip>(true);
                EnterLevel();
                break;
            case "Button3":
                MusicMgr.Instance.PlaySound("Yes Button");
                Level.Instance.EnterLevel(0, 2);
                UIMgr.Instance.HidePanel<PanelTip>(true);
                EnterLevel();
                break;
            case "Button4":
                MusicMgr.Instance.PlaySound("Yes Button");
                Level.Instance.EnterLevel(0, 3);
                UIMgr.Instance.HidePanel<PanelTip>(true);
                EnterLevel();
                break;
            case "ButtonBack":
                MusicMgr.Instance.PlaySound("Yes Button");
                UIMgr.Instance.HidePanel<PanelTip>();
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIMgr.Instance.HidePanel<PanelTip>();
    }
}
