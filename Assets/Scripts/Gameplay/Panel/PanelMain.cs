using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ö÷Ãæ°å
/// </summary>
public class PanelMain : BasePanel
{
    public override void HideMe()
    {
    }

    public override void ShowMe()
    {
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    protected override void ClickBtn(string btnName)
    {
        base.ClickBtn(btnName);
        switch (btnName)
        {
            case "ButtonStart":
                MusicMgr.Instance.PlaySound("Yes Button");
                UIMgr.Instance.ShowPanel<PanelPhoneHome>();
                UIMgr.Instance.HidePanel<PanelMain>();
                break;
        }
    }
}
