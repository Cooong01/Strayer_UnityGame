using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̵�ؿ�ѡ�����
/// </summary>
public class PanelStore : BasePanel
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
