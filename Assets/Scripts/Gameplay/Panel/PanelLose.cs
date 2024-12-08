using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelLose : BasePanel
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
            case "ButtonRestart":

                MusicMgr.Instance.PlaySound("Yes Button");
                UIMgr.Instance.HidePanel<PanelLose>();
                UIMgr.Instance.ShowPanel<PanelPlay>();
                Level.Instance.EnterThisLevel();
                break;
            case "ButtonBackToHome":

                MusicMgr.Instance.PlaySound("Yes Button");
                UIMgr.Instance.HidePanel<PanelLose>();
                UIMgr.Instance.ShowPanel<PanelPhoneHome>();
                break;

        }
    }
}
