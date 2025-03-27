using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商店关卡选关面板
/// </summary>
public class PanelStore : BasePanel
{

    private bool onShow;
    private RectTransform rt;
    private float duration; //已经经过的时间
    private float showTime = 0.3f; //展示的时间

    public override void HideMe()
    {

    }

    //展示的时候，画面从小到大
    public override void ShowMe()
    {
        onShow = true;
        rt = GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;
        duration = 0;
        ShowBlock(2);
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
                Level.Instance.EnterLevel(2, 0);
                UIMgr.Instance.HidePanel<PanelStore>();
                EnterLevel();
                break;
            case "Button2":
                MusicMgr.Instance.PlaySound("Yes Button");
                Level.Instance.EnterLevel(2, 1);
                UIMgr.Instance.HidePanel<PanelStore>();
                EnterLevel();
                break;
            case "Button3":
                MusicMgr.Instance.PlaySound("Yes Button");
                Level.Instance.EnterLevel(2, 2);
                UIMgr.Instance.HidePanel<PanelStore>();
                EnterLevel();
                break;
            case "ButtonBack":
                MusicMgr.Instance.PlaySound("Yes Button");
                UIMgr.Instance.HidePanel<PanelStore>();
                break;
        }
    }
}
