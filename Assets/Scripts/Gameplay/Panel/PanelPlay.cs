using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PanelPlay : BasePanel
{
    
    private int onShowMirrorNumber;
    private bool isDecreasingHP; //是否正在减少HP
    private float decreasingTime = 1; //减少一个HP时的总时间
    private int decreasingPool; //减少HP的缓存标记。当玩家受击多次，而当前血条正在减少的时候，就令缓存次数+1
    private int NowHPNumber = 3; //现在正在减少的HP条的序号
    private float decreasingDuration; //减少一个血条时已经经过的时间
    public Button jump;
    public Button rush;
    

    void Start()
    {
        jump = transform.Find("ButtonJump").gameObject.GetComponent<Button>();
        EventTrigger trigger = jump.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { OnButtonPressed(); });
        trigger.triggers.Add(pointerDown);

        rush = transform.Find("ButtonRush").gameObject.GetComponent<Button>();
        EventTrigger trigger2 = rush.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDown2 = new EventTrigger.Entry();
        pointerDown2.eventID = EventTriggerType.PointerDown;
        pointerDown2.callback.AddListener((data) => { OnButtonRushPressed(); });
        trigger2.triggers.Add(pointerDown2);



        //注册了：放镜子、镜子放成功、回收镜子的相关事件回调
        EventCenter.Instance.AddEventListener(E_EventType.E_EnterMirrorInteraction,ShowMirrorControlButtons);
        EventCenter.Instance.AddEventListener(E_EventType.E_LeaveMirrorPlace,HideButtonCloseMirrorPlace);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_CloseMirrorPlace, ShowButtonCloseMirrorPlace);
        EventCenter.Instance.AddEventListener(E_EventType.E_OutMirrorModeChange, ShowButtonChangeMirrorMode);
        EventCenter.Instance.AddEventListener(E_EventType.E_OutPlayerModeChange, ShowButtonChangePlayerMode);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_EnterMirrorRecycle, ShowButtonMirrorRecycle);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_OutMirrorRecycle, HideButtonMirrorRecycle);

        //剧情对话框的的事件
        EventCenter.Instance.AddEventListener(E_EventType.E_TalkWithOther, StartTalk);
        EventCenter.Instance.AddEventListener(E_EventType.E_CloseTalk, ShowPlot);
        EventCenter.Instance.AddEventListener(E_EventType.E_LeaveTalk, HidePlot);

        //玩家受击的事件
        EventCenter.Instance.AddEventListener(E_EventType.E_PlayerAttacked, Dead);
        EventCenter.Instance.AddEventListener(E_EventType.E_EnterLevel, ResetHP);
        EventCenter.Instance.AddEventListener(E_EventType.E_RealFail, ShowFail);


    }

    public void OnButtonPressed()
    {
        print("按下了跳跃");
        //跳跃时触发跳跃事件
        EventCenter.Instance.EventTrigger(E_EventType.E_PressJumpButton);
    }

    public void OnButtonRushPressed()
    {

        EventCenter.Instance.EventTrigger(E_EventType.E_PressDashButton);
    }



    void Update()
    {
        if (isDecreasingHP)
        {
            DecreasingHP();
        }
        if (!isDecreasingHP && decreasingPool > 0) //检测缓存池
        {
            isDecreasingHP = true;
            decreasingPool--;
            decreasingDuration = decreasingTime;
        }

    }

    //摇杆以外的事件，写在这里。摇杆的逻辑，写在摇杆类里。
    protected override void ClickBtn(string btnName)
    {
        base.ClickBtn(btnName);
        switch (btnName)
        {
            case "ButtonJump":

                break;
            case "ButtonSetting":
                EnterSuspendMode();
                break;
            case "ButtonBack2Game":
                OutSuspendMode();
                break;
            case "ButtonRestart":
                Level.Instance.DestroyLevel();
                Level.Instance.EnterThisLevel();
                ReStart();
                Reset();
                OutSuspendMode();
                break;
            case "ButtonBack2Home":
                //摧毁本关
                OutSuspendMode();
                Level.Instance.DestroyLevel();
                UIMgr.Instance.HidePanel<PanelPlay>(true);
                UIMgr.Instance.ShowPanel<PanelPhoneHome>();
                ResetHP();
                MusicMgr.Instance.StopBKMusic();
                MusicMgr.Instance.PlayBKMusic("Magical Brittlethorn Cut 60");
                break;

            case "ButtonRush":
                break;

            case "ButtonMirrorTransform":
                EventCenter.Instance.EventTrigger(E_EventType.E_PressChangeMirrorModeButton);
                HideButtonChangeMirrorMode();
                break;

            case "ButtonPlayerTransform":
                EventCenter.Instance.EventTrigger(E_EventType.E_PressChangePlayerModeButton);
                HideButtonChangeplayerMode();
                break;

            case "ButtonCloseMirrorPlace1":
                EventCenter.Instance.EventTrigger(E_EventType.E_ControlMirror,1);
                onShowMirrorNumber = 1;
                HideButtonCloseMirrorPlace();
                break;
            case "ButtonCloseMirrorPlace2":
                EventCenter.Instance.EventTrigger(E_EventType.E_ControlMirror,2);
                onShowMirrorNumber = 2;
                HideButtonCloseMirrorPlace();
                break;
            case "ButtonCloseMirrorPlace3":
                EventCenter.Instance.EventTrigger(E_EventType.E_ControlMirror,3);
                onShowMirrorNumber = 3;
                HideButtonCloseMirrorPlace();
                break;

            case "ButtonMirrorRecycle1":
                EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 1);
                HideButtonMirrorRecycle(1);
                break;
            case "ButtonMirrorRecycle2":
                EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 2);
                HideButtonMirrorRecycle(2);
                break;
            case "ButtonMirrorRecycle3":
                EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 3);
                HideButtonMirrorRecycle(3);
                break;

            case "ButtonMirrorOrientation":
                EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorOrientationButton);
                break;
            case "ButtonMirrorSet":
                EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorSetButton);
                HideMirrorControlButtons();
                break;
            case "ButtonPlot":
                EventCenter.Instance.EventTrigger(E_EventType.E_TalkWithOther);
                break;
        }
    }


    //进入镜子交互范围，根据剩余镜子的编号，展示对应的镜子交互按钮
    private void ShowButtonCloseMirrorPlace(int mirrorNumber)
    {
        switch (mirrorNumber)
        {
            case 1:
                transform.Find("ButtonCloseMirrorPlace1").gameObject.SetActive(true);
                break;
            case 2:
                transform.Find("ButtonCloseMirrorPlace2").gameObject.SetActive(true);
                break;
            case 3:
                transform.Find("ButtonCloseMirrorPlace3").gameObject.SetActive(true);
                break;
        }

    }


    private void StartTalk()
    {
        transform.Find("ButtonPlot").gameObject.SetActive(false);
        TaikWithOther taikWithOther = transform.Find("TaikWithOther").GetComponent<TaikWithOther>();
        //填写字符串
        taikWithOther.StartTalk(GameObject.Find("Player").GetComponent<PhysicsCheck>().TalkName.name);
    }

    //离开交互范围
    private void HideButtonCloseMirrorPlace()
    {
        transform.Find("ButtonCloseMirrorPlace1").gameObject.SetActive(false);
        transform.Find("ButtonCloseMirrorPlace2").gameObject.SetActive(false);
        transform.Find("ButtonCloseMirrorPlace3").gameObject.SetActive(false);
        HideMirrorControlButtons();
    }

    private void ShowMirrorControlButtons()
    {
        transform.Find("ButtonMirrorOrientation").gameObject.SetActive(true);
        transform.Find("ButtonMirrorSet").gameObject.SetActive(true);
    }

    private void HideMirrorControlButtons()
    {
        transform.Find("ButtonMirrorOrientation").gameObject.SetActive(false);
        transform.Find("ButtonMirrorSet").gameObject.SetActive(false);
    }

    public override void ShowMe()
    {
        MusicMgr.Instance.PlayBKMusic("Magical Lost Magic Cut 60");
    }

    public override void HideMe()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.E_EnterMirrorInteraction, ShowMirrorControlButtons);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_LeaveMirrorPlace, HideButtonCloseMirrorPlace);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_CloseMirrorPlace, ShowButtonCloseMirrorPlace);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_OutMirrorModeChange, ShowButtonChangeMirrorMode);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_OutPlayerModeChange, ShowButtonChangePlayerMode);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_EnterMirrorRecycle, ShowButtonMirrorRecycle);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_OutMirrorRecycle, HideButtonMirrorRecycle);

        //剧情对话框的的事件
        EventCenter.Instance.RemoveEventListener(E_EventType.E_TalkWithOther, StartTalk);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_CloseTalk, ShowPlot);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_LeaveTalk, HidePlot);

        //玩家受击的事件
        EventCenter.Instance.RemoveEventListener(E_EventType.E_PlayerAttacked, Dead);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_EnterLevel, ResetHP);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_RealFail, ShowFail);

        Reset();
    }
    private void ShowButtonChangeMirrorMode()
    {
        transform.Find("ButtonMirrorTransform").gameObject.SetActive(true);
    }
    private void HideButtonChangeMirrorMode()
    {
        transform.Find("ButtonMirrorTransform").gameObject.SetActive(false);
    }


    private void ShowButtonChangePlayerMode()
    {
        transform.Find("ButtonPlayerTransform").gameObject.SetActive(true);
    }
    private void HideButtonChangeplayerMode()
    {
        transform.Find("ButtonPlayerTransform").gameObject.SetActive(false);
    }

    private void ShowButtonMirrorRecycle(int mirrorNumber)
    {
        switch (mirrorNumber)
        {
            case 1:
                transform.Find("ButtonMirrorRecycle1").gameObject.SetActive(true);
                break;
            case 2:
                transform.Find("ButtonMirrorRecycle2").gameObject.SetActive(true);
                break;
            case 3:
                transform.Find("ButtonMirrorRecycle3").gameObject.SetActive(true);
                break;
        }
    }

    private void HideButtonMirrorRecycle(int mirrorNumber)
    {
        print(mirrorNumber);
        switch (mirrorNumber)
        {
            case 1:
                transform.Find("ButtonMirrorRecycle1").gameObject.SetActive(false);
                break;
            case 2:
                transform.Find("ButtonMirrorRecycle2").gameObject.SetActive(false);
                break;
            case 3:
                transform.Find("ButtonMirrorRecycle3").gameObject.SetActive(false);
                break;
        }
    }

    //进入暂停模式
    private void EnterSuspendMode()
    {
        //时间设为0
        Time.timeScale = 0;
        transform.Find("ImageSuspend").gameObject.SetActive(true);
    }

    //退出暂停模式
    private void OutSuspendMode()
    {
        Time.timeScale = 1;
        transform.Find("ImageSuspend").gameObject.SetActive(false);
    }

    //


    private void ReStart()
    {
        ResetHP();
    }

    //玩家受击时处理HP，做血条减少的方法
    public void Hit()
    {
        
        if (!isDecreasingHP)
        {
            isDecreasingHP = true;
            decreasingDuration = decreasingTime;
        }
        else
        {
            decreasingPool++;
        }
    }

    //逐渐减少血条的方法
    private void DecreasingHP()
    {
        //规定：血条血量贴图的名字：HP1,HP2,HP3
        string HPName = "HP" + NowHPNumber.ToString();

        //找到当前要减少血量的血条的图片
        Image HPImage = transform.Find(HPName).GetComponent<Image>();

        if (decreasingTime>decreasingDuration)
        {
            decreasingDuration += Time.deltaTime;
            //todo：为图片的显示进度做插值（改成：用slider滑块来做，不是图片）
            Mathf.Lerp(1, 0, 1 - decreasingDuration / decreasingTime);
        }
        else
        {
            isDecreasingHP = false;
            //todo:把血条滑块直接setactive(False)

        }
    }

    //重置血条
    private void ResetHP()
    {
        //todo：找到三个血条，然后setActive(True)，且进度设为100
    }

    //展示失败页面
    private void ShowFail()
    {
        
        UIMgr.Instance.HidePanel<PanelPlay>(true);
        UIMgr.Instance.ShowPanel<PanelLose>();
        
    }

    private void Dead()
    {
        ShowFail();
    }
    private void ShowPlot()
    {
        if (transform.Find("TaikWithOther").gameObject.activeSelf) //如果对话框正在显示
        {
            return;
        }
        transform.Find("ButtonPlot").gameObject.SetActive(true);
    }
    private void HidePlot()
    {
        transform.Find("ButtonPlot").gameObject.SetActive(false);
    }

    private void Reset()
    {
        HideButtonChangeplayerMode();
        HideButtonCloseMirrorPlace();
        HideButtonMirrorRecycle(1);
        HideButtonMirrorRecycle(2);
        HideButtonMirrorRecycle(3);
        HideMirrorControlButtons();
        HidePlot();
    }
}
