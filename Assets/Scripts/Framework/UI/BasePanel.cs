using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour
{
    //用于存储所有要用到的UI控件
    protected Dictionary<string, UIBehaviour> controlDic = new Dictionary<string, UIBehaviour>();

    //默认控件名列表。未修改名称的控件不会被获取。
    private static List<string> defaultNameList = new List<string>() { "Image",
                                                                   //"Text (TMP)", //TMP控件是需要控制的，因为TMP字体必须由Resources来加载，用AB包不行，所以用AB包加载完面板预制体之后需要用代码再从Resources里加载字体资源，附加到TMP组件的font字段。
                                                                   "RawImage",
                                                                   "Background",
                                                                   "Checkmark",
                                                                   "Label",
                                                                   "Text (Legacy)",
                                                                   "Arrow",
                                                                   "Placeholder",
                                                                   "Fill",
                                                                   "Handle",
                                                                   "Viewport",
                                                                   "Scrollbar Horizontal",
                                                                   "Scrollbar Vertical"};


    protected virtual void Awake()
    {
        FindChildrenControl<Button>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<InputField>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<Dropdown>();
        FindChildrenControl<Text>();        
        TMPABLoad.Instance.LoadTMP(this.gameObject); //TMP字体资源不会随着面板一起以AB包加载，所以要按照项目设置里指定的TMP字体路径
                                                     //通过Resources来加载。本项目路径是Resources/Fonts。字体必须放进该路径。
        FindChildrenControl<TextMeshProUGUI>();
        FindChildrenControl<Image>();
        

    }

    /// <summary>
    /// 面板显示时调用
    /// </summary>
    public abstract void ShowMe();


    /// <summary>
    /// 面板隐藏时调用
    /// </summary>
    public abstract void HideMe();

    /// <summary>
    /// 根据名字和类型获取指定控件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="name">组件名字</param>
    public T GetControl<T>(string name) where T:UIBehaviour
    {
        if(controlDic.ContainsKey(name))
        {
            T control = controlDic[name] as T;
            if (control == null)
                Debug.LogError($"不存在对应名字{name}类型为{typeof(T)}的组件");
            return control;
        }
        else
        {
            Debug.LogError($"不存在对应名字{name}的组件");
            return null;
        }
    }

    //按钮点击时触发
    protected virtual void ClickBtn(string btnName)
    {

    }


    protected virtual void SliderValueChange(string sliderName, float value)
    {

    }

    protected virtual void ToggleValueChange(string sliderName, bool value)
    {

    }

    private void FindChildrenControl<T>() where T:UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>(true);
        for (int i = 0; i < controls.Length; i++)
        {
            string controlName = controls[i].gameObject.name;
            if (!controlDic.ContainsKey(controlName))
            {
                if(!defaultNameList.Contains(controlName))
                {
                    controlDic.Add(controlName, controls[i]);
                    //为几个常见需要监听的控件预先加上监听回调函数，重写预设的回调函数即可直接使用监听。
                    if(controls[i] is Button)
                    {
                        (controls[i] as Button).onClick.AddListener(() =>
                        {
                            ClickBtn(controlName);
                        });
                    }
                    else if(controls[i] is Slider)
                    {
                        (controls[i] as Slider).onValueChanged.AddListener((value) =>
                        {
                            SliderValueChange(controlName, value);
                        });
                    }
                    else if(controls[i] is Toggle)
                    {
                        (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                        {
                            ToggleValueChange(controlName, value);
                        });
                    }
                }
                    
            }
        }
    }

    public void EnterLevel()
    {
        UIMgr.Instance.HidePanel<PanelPhoneHome>(true);

        UIMgr.Instance.ShowPanel<PanelPlay>();
    }

    /// <summary>
    /// 用来根据关卡进度锁住当前选关按钮
    /// todo:有待改进
    /// </summary>
    public void ShowBlock(int BigLevel)
    {
        print("当前的进度：" + Level.Instance.LevelBig+"," + Level.Instance.LevelSmall);
        if (BigLevel > Level.Instance.LevelBig)
        {
            //全锁
            this.transform.Find("Image1").gameObject.SetActive(true);
            this.transform.Find("Image2").gameObject.SetActive(true);
            this.transform.Find("Image3").gameObject.SetActive(true);
        }
        else if (BigLevel == Level.Instance.LevelBig)
        {
            if(BigLevel == 0)
            {
                switch (Level.Instance.LevelSmall)
                {
                    case 0:
                        this.transform.Find("Image1").gameObject.SetActive(false);
                        this.transform.Find("Image2").gameObject.SetActive(true);
                        break;
                    case 1:
                        this.transform.Find("Image1").gameObject.SetActive(false);
                        this.transform.Find("Image2").gameObject.SetActive(false);
                        break;

                }
            }
            else
            {
                switch (Level.Instance.LevelSmall)
                {
                    case 0:
                        this.transform.Find("Image1").gameObject.SetActive(false);
                        this.transform.Find("Image2").gameObject.SetActive(true);
                        this.transform.Find("Image3").gameObject.SetActive(true);
                        break;
                    case 1:
                        this.transform.Find("Image1").gameObject.SetActive(false);
                        this.transform.Find("Image2").gameObject.SetActive(false);
                        this.transform.Find("Image3").gameObject.SetActive(true);
                        break;
                    case 2:
                        this.transform.Find("Image1").gameObject.SetActive(false);
                        this.transform.Find("Image2").gameObject.SetActive(false);
                        this.transform.Find("Image3").gameObject.SetActive(false);
                        break;
                }

            }

        }
        else
        {
            this.transform.Find("Image1").gameObject.SetActive(false);
            this.transform.Find("Image2").gameObject.SetActive(false);
            this.transform.Find("Image3").gameObject.SetActive(false);
        }
    }

    public void GetNpc(string name)
    {
        List<string> Data = Level.Instance.NpcNum[name];

        for (int i = 0;i < transform.Find("Text").childCount;i++) {
            transform.Find("Text").GetChild(i).GetComponent<TextMeshProUGUI>().text = Data[i];
        }

    }
}
