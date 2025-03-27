using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI面板分层
/// </summary>
public enum E_UILayer
{
    Bottom,
    Middle,
    Top,
    System,
}

/// <summary>
/// UI管理器
/// </summary>
public class UIMgr : BaseManager<UIMgr>
{
    private abstract class BasePanelInfo { } //面板基类
    
    //面板信息类
    //注意：面板脚本和面板预制体必须是一对一的关系。这样便于使用反射获取脚本名进而获取资源名，再结合泛型之后非常方便使用面板脚本类对面板进行管理
    private class PanelInfo<T> : BasePanelInfo where T:BasePanel
    {
        public T panel;
        public UnityAction<T> callBack;
        public bool isHide;

        public PanelInfo(UnityAction<T> callBack)
        {
            this.callBack += callBack;
        }
    }


    private Camera uiCamera; //UI专用相机
    private Canvas uiCanvas; //非3D空间UI的专用Canvas
    private EventSystem uiEventSystem; //unity事件系统
    private string panelPath = "ui"; //默认的面板预制体位置是ui文件夹下

    private Transform bottomLayer;
    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>(); //面板池

    private UIMgr()
    {
        uiCamera = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
        GameObject.DontDestroyOnLoad(uiCamera.gameObject);
        uiCanvas = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
        uiCanvas.worldCamera = uiCamera;
        GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

        bottomLayer = uiCanvas.transform.Find("Bottom");
        middleLayer = uiCanvas.transform.Find("Middle");
        topLayer = uiCanvas.transform.Find("Top");
        systemLayer = uiCanvas.transform.Find("System");

        uiEventSystem = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
        GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
    }

    //获得指定层级的载体物体
    public Transform GetLayerFather(E_UILayer layer)
    {
        switch (layer)
        {
            case E_UILayer.Bottom:
                return bottomLayer;
            case E_UILayer.Middle:
                return middleLayer;
            case E_UILayer.Top:
                return topLayer;
            case E_UILayer.System:
                return systemLayer;
            default:
                return null;
        }
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="layer">显示的层级，默认为Middle</param>
    /// <param name="callBack">显示以后的回调</param>
    /// <param name="isSync">是否同步</param>
    /// <typeparam name="T">面板脚本类</typeparam>
    public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle, UnityAction<T> callBack = null, bool isSync = false) where T:BasePanel
    {
        string panelName = typeof(T).Name;
        if(panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            if(panelInfo.panel == null)
            {
                panelInfo.isHide = false;
                if (callBack != null)
                    panelInfo.callBack += callBack;
            }
            else
            {
                if (!panelInfo.panel.gameObject.activeSelf)
                    panelInfo.panel.gameObject.SetActive(true);

                panelInfo.panel.ShowMe();
                callBack?.Invoke(panelInfo.panel);
            }
            return;
        }
        panelDic.Add(panelName, new PanelInfo<T>(callBack));
        ABResMgr.Instance.LoadResAsync<GameObject>(panelPath, panelName, (res) =>
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            if(panelInfo.isHide)
            {
                panelDic.Remove(panelName);
                return;
            }

            Transform father = GetLayerFather(layer);
            if (father == null)
                father = middleLayer;
            GameObject panelObj = GameObject.Instantiate(res, father, false);

            T panel = panelObj.GetComponent<T>();
            panel.ShowMe();
            panelInfo.callBack?.Invoke(panel);
            panelInfo.callBack = null;
            panelInfo.panel = panel;

        }, isSync);
    }
    //隐藏面板
    public void HidePanel<T>(bool isDestory = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            if(panelInfo.panel == null)
            {
                panelInfo.isHide = true;
                panelInfo.callBack = null;
            }
            else
            {
                panelInfo.panel.HideMe();
                if (isDestory)
                {
                    GameObject.Destroy(panelInfo.panel.gameObject);
                    panelDic.Remove(panelName);
                }
                else
                    panelInfo.panel.gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// 获取指定面板
    /// </summary>
    /// <typeparam name="T">面板脚本</typeparam>
    public void GetPanel<T>( UnityAction<T> callBack ) where T:BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            if(panelInfo.panel == null)
            {
                panelInfo.callBack += callBack;
            }
            else if(!panelInfo.isHide)
            {
                callBack?.Invoke(panelInfo.panel);
            }
        }
    }

    /// <summary>
    /// 为指定UI控件添加指定类型的监听
    /// </summary>
    /// <param name="control">UI控件</param>
    /// <param name="type">事件触发类型</param>
    /// <param name="callBack">回调</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }
}
