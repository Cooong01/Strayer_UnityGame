using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI���ֲ�
/// </summary>
public enum E_UILayer
{
    Bottom,
    Middle,
    Top,
    System,
}

/// <summary>
/// UI������
/// </summary>
public class UIMgr : BaseManager<UIMgr>
{
    private abstract class BasePanelInfo { } //������
    
    //�����Ϣ��
    //ע�⣺���ű������Ԥ���������һ��һ�Ĺ�ϵ����������ʹ�÷����ȡ�ű���������ȡ��Դ�����ٽ�Ϸ���֮��ǳ�����ʹ�����ű���������й���
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


    private Camera uiCamera; //UIר�����
    private Canvas uiCanvas; //��3D�ռ�UI��ר��Canvas
    private EventSystem uiEventSystem; //unity�¼�ϵͳ
    private string panelPath = "ui"; //Ĭ�ϵ����Ԥ����λ����ui�ļ�����

    private Transform bottomLayer;
    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>(); //����

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

    //���ָ���㼶����������
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
    /// ��ʾ���
    /// </summary>
    /// <param name="layer">��ʾ�Ĳ㼶��Ĭ��ΪMiddle</param>
    /// <param name="callBack">��ʾ�Ժ�Ļص�</param>
    /// <param name="isSync">�Ƿ�ͬ��</param>
    /// <typeparam name="T">���ű���</typeparam>
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
    //�������
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
    /// ��ȡָ�����
    /// </summary>
    /// <typeparam name="T">���ű�</typeparam>
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
    /// Ϊָ��UI�ؼ����ָ�����͵ļ���
    /// </summary>
    /// <param name="control">UI�ؼ�</param>
    /// <param name="type">�¼���������</param>
    /// <param name="callBack">�ص�</param>
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
