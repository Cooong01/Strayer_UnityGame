using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelStaff : BasePanel
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
            case "ButtonBack":
                MusicMgr.Instance.PlaySound("Yes Button");
                UIMgr.Instance.HidePanel<PanelStaff>();
                UIMgr.Instance.ShowPanel<PanelPhoneHome>();
                break;
        }
    }
}
