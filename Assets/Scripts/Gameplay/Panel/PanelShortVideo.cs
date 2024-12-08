using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelShortVideo : BasePanel
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
        ShowBlock(3);
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
                Level.Instance.EnterLevel(3, 0);
                UIMgr.Instance.HidePanel<PanelShortVideo>();
                EnterLevel();
                break;
            case "Button2":
                MusicMgr.Instance.PlaySound("Yes Button");
                Level.Instance.EnterLevel(3, 1);
                UIMgr.Instance.HidePanel<PanelShortVideo>();
                EnterLevel();
                break;
            case "Button3":
                MusicMgr.Instance.PlaySound("Yes Button");
                Level.Instance.EnterLevel(3, 2);
                UIMgr.Instance.HidePanel<PanelShortVideo>();
                EnterLevel();
                break;
            case "ButtonBack":
                MusicMgr.Instance.PlaySound("Yes Button");
                UIMgr.Instance.HidePanel<PanelShortVideo>();
                break;

        }
    }
}
