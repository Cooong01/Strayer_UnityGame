using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 一个小问题：TMP字体资源不会随着面板一起以AB包加载，所以要按照指定的TMP字体的加载路径，通过Resources来加载。这个路径是Resources/Fonts下面。可以在其他地方引用字体，但是必须在这里存放一份。
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 用于存储所有要用到的UI控件
    /// </summary>
    protected Dictionary<string, UIBehaviour> controlDic = new Dictionary<string, UIBehaviour>();


    /// <summary>
    /// 控件默认名字 如果得到的控件名字存在于这个容器 意味着我们不会通过代码去使用它 它只会是起到显示作用的控件。
    /// 这个容器对于程序的实现不是必要的，但是没有这个列表就可能会造成内存占用过高。如果需要无差别获取某一类控件，可以把其中的某类控件默认名称给注释掉。
    /// </summary>
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
        //为了避免 某一个对象上存在两种控件的情况
        //我们应该优先查找重要的组件
        FindChildrenControl<Button>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<InputField>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<Dropdown>();
        //即使对象上挂在了多个组件 只要优先找到了重要组件
        //之后也可以通过重要组件得到身上其他挂载的内容
        FindChildrenControl<Text>();
        FindChildrenControl<TextMeshProUGUI>();
        TMPABLoad.Instance.load(this.gameObject);
        FindChildrenControl<Image>();
    }

    /// <summary>
    /// 面板显示时会调用的逻辑。
    /// </summary>
    public abstract void ShowMe();


    /// <summary>
    /// 面板隐藏时会调用的逻辑
    /// </summary>
    public abstract void HideMe();

    /// <summary>
    /// 获取指定名字以及指定类型的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="name">组件名字</param>
    /// <returns></returns>
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
            //获取当前控件的名字
            string controlName = controls[i].gameObject.name;
            //通过这种方式 将对应组件记录到字典中
            if (!controlDic.ContainsKey(controlName))
            {
                if(!defaultNameList.Contains(controlName))
                {
                    controlDic.Add(controlName, controls[i]);
                    //判断控件的类型 决定是否加事件监听
                    if(controls[i] is Button)
                    {
                        (controls[i] as Button).onClick.AddListener(() =>
                        {
                            ClickBtn(controlName);
                        });
                    }
                    //todo：为按钮加上“按钮点击”的事件
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
    /// 用来根据关卡进度锁住当前按钮的方案
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
