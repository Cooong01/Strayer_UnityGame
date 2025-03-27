using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour
{
    //���ڴ洢����Ҫ�õ���UI�ؼ�
    protected Dictionary<string, UIBehaviour> controlDic = new Dictionary<string, UIBehaviour>();

    //Ĭ�Ͽؼ����б�δ�޸����ƵĿؼ����ᱻ��ȡ��
    private static List<string> defaultNameList = new List<string>() { "Image",
                                                                   //"Text (TMP)", //TMP�ؼ�����Ҫ���Ƶģ���ΪTMP���������Resources�����أ���AB�����У�������AB�����������Ԥ����֮����Ҫ�ô����ٴ�Resources�����������Դ�����ӵ�TMP�����font�ֶΡ�
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
        TMPABLoad.Instance.LoadTMP(this.gameObject); //TMP������Դ�����������һ����AB�����أ�����Ҫ������Ŀ������ָ����TMP����·��
                                                     //ͨ��Resources�����ء�����Ŀ·����Resources/Fonts���������Ž���·����
        FindChildrenControl<TextMeshProUGUI>();
        FindChildrenControl<Image>();
        

    }

    /// <summary>
    /// �����ʾʱ����
    /// </summary>
    public abstract void ShowMe();


    /// <summary>
    /// �������ʱ����
    /// </summary>
    public abstract void HideMe();

    /// <summary>
    /// �������ֺ����ͻ�ȡָ���ؼ�
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <param name="name">�������</param>
    public T GetControl<T>(string name) where T:UIBehaviour
    {
        if(controlDic.ContainsKey(name))
        {
            T control = controlDic[name] as T;
            if (control == null)
                Debug.LogError($"�����ڶ�Ӧ����{name}����Ϊ{typeof(T)}�����");
            return control;
        }
        else
        {
            Debug.LogError($"�����ڶ�Ӧ����{name}�����");
            return null;
        }
    }

    //��ť���ʱ����
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
                    //Ϊ����������Ҫ�����Ŀؼ�Ԥ�ȼ��ϼ����ص���������дԤ��Ļص���������ֱ��ʹ�ü�����
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
    /// �������ݹؿ�������ס��ǰѡ�ذ�ť
    /// todo:�д��Ľ�
    /// </summary>
    public void ShowBlock(int BigLevel)
    {
        print("��ǰ�Ľ��ȣ�" + Level.Instance.LevelBig+"," + Level.Instance.LevelSmall);
        if (BigLevel > Level.Instance.LevelBig)
        {
            //ȫ��
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
