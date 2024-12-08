using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
/// <summary>
/// �����洢��ťƽ���ƶ�������ί��
/// </summary>
public delegate void TransformAPP();

/// <summary>
/// �ֻ�Homeҳ�����
/// todo:���ֻ�״̬��Ч�����ֻ�ҳ�滬��Ч������ť�����϶�Ч��
/// </summary>
public class PanelPhoneHome : BasePanel, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler,IPointerClickHandler
{
    private float baseWidth = 3200;
    private float baseHeight = 2048;

    private float nowWidth;
    private float nowHeight;


    //ע�⣺�����ֻ�ҳ���������ģ������ֻ���������ӽ��Ǵ��Ҳ�����������������ű��У����С��͡��С�����unity�����ϵ����У�����ģ����ֻ�����������ϵ����С�
    //һ��ҳ����Ŀ�����APP������
    public int rowNumber = 3;
    public int columnNumber = 2;

    //�������Һ����µĿ�ȱ����
    private float leftAndRightSafetyValue;
    private float TopAndButtomSafetyValue;

    private float leftAndRightSafetyRadio = 0.25f;
    private float TopAndButtomSafetyRadio = 0.25f;

    //APP�Զ��ƶ��������ٶ�
    public int transformSepeed;


    //�л�Ϊ��קģʽ��ʱ��
    public float pressingTime = 2;

    //���ֺ�APP�Զ��ƶ�����Ӧλ�õ��ٶ�
    public float transformSpeed = 0.01f;

    //��ҳAPP������
    public int APPNumber = 10;

    //������ק����ק����ı�ʶ��
    private bool isDragging = false;

    //��Ұ���Ļʱ������
    private Vector2 pressPosition;
    private bool isPressing = false;
    private float pressDuration = 0;

    //������קģʽ�ı�־
    private bool isDraggingMode = false;

    //����קģʽ��ѡ�е�APP
    private GameObject selected;

    //�ֱ��Ǹ��λ�úͶ�Ӧ�����
    private List<(Vector2,int)> points;

    //�������ڸı�λ�õ�APP
    private event TransformAPP transformAPP = null;

    private Vector3 targetPostion;

    //�ֱ���APP��ʵ���͸�ʵ����Ӧ�ĸ�����
    private Dictionary<GameObject, int> DicApps;

    private bool isSelected = false;

    private bool isFirstRun = true;

    private Vector3 nowAppPostion;

    private float appTime = 0;
    private float allTime = 0.2f;



    public override void HideMe()
    {

    }

    public override void ShowMe()
    {
        isDragging = false;
        isPressing = false;
        pressDuration = 0;
        isDraggingMode = false;
        transformAPP = null;
        isFirstRun = true;
        appTime = 0;
        allTime = 0.2f;

    }

    void Start()
    {
        //����APP�������Լ�panel��С�����һ�������Ÿ��ָ�����ĵ�position���б�
        Vector3 panelScale = new Vector3(Screen.width, Screen.height, 0);
        nowWidth = Screen.width;
        nowHeight = Screen.height;
        leftAndRightSafetyValue = (int)(leftAndRightSafetyRadio * nowWidth);
        TopAndButtomSafetyValue = (int)(TopAndButtomSafetyRadio * nowHeight);
        print("panel���" + panelScale.x);
        print("panel�߶�" + panelScale.y);
        points = new List<(Vector2,int)>();
        //��ǰpanel�����꣨�������䴦����λ��ƫ�ƣ�
        Vector3 panelPosition = this.transform.GetComponent<RectTransform>().position;
        print("x����"+panelPosition.x);
        print("y����" + panelPosition.y);

        //����������֮��ļ�ࡣ���������ľ���.
        //�м��
        float spaceRow = ((panelScale.x - leftAndRightSafetyValue) / columnNumber) ;
        //�м��
        float spaceCol = ((panelScale.y - TopAndButtomSafetyValue) / rowNumber);
        int pointNumber = 0;

        //�˴������ֻ�ҳ����߼��������Ų��������ֻ���ʱ�������
        for (int col = 1; col <= columnNumber; col++)
        {
            float x = spaceRow * col;//��һ�е�x����
            print("x" + x);
            for (int row = 1; row <= rowNumber; row++)
            {
                float y = spaceCol * row;//��һ�е�y����
                print("y" + y);
                Vector2 pointPosition = new Vector2(x - panelPosition.x - spaceRow / 2 + leftAndRightSafetyValue / 2, y - panelPosition.y - spaceCol / 2 +TopAndButtomSafetyValue / 2);
                points.Add((pointPosition, ++pointNumber));
                print("Ŀǰ�����ţ� " + pointNumber + "��Ŀǰ���λ�ã� " + pointPosition.x + "   " + pointPosition.y);

                ////���ز��Ը��λ�õ�ͼƬ
                //ABResMgr.Instance.LoadResAsync<GameObject>("prefabs","imageTest" ,callBack: (t) => {
                //    GameObject AB = GameObject.Instantiate(t,this.transform,false);
                //    AB.transform.GetComponent<RectTransform>().anchoredPosition = pointPosition;
                //});
            }
        }

        DicApps = new Dictionary<GameObject, int>();
        int number = 0;
        foreach (var point in points)
        {
            //todo������ĳ�����ǰ����õ��б�����ȡ����
            switch (point.Item2)
            {
                case 1: //��һ��λ�÷Ž�ѧ��
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPTutorials", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("ĿǰAPP�ĸ����ţ� " + point.Item2 + "��ĿǰAPP�ĸ��λ�ã� " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;
                case 2: //�ڶ���λ�÷���̳��
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPForums", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("ĿǰAPP�ĸ����ţ� " + point.Item2 + "��ĿǰAPP�ĸ��λ�ã� " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;


                case 3: //������λ�÷Ź����
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPShooping", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("ĿǰAPP�ĸ����ţ� " + point.Item2 + "��ĿǰAPP�ĸ��λ�ã� " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;

                case 4: //���ĸ�λ�÷Ŷ���Ƶ��
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPShortVideo", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("ĿǰAPP�ĸ����ţ� " + point.Item2 + "��ĿǰAPP�ĸ��λ�ã� " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;
                case 5: //�����λ�÷�������Ա����
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPStaffList", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("ĿǰAPP�ĸ����ţ� " + point.Item2 + "��ĿǰAPP�ĸ��λ�ã� " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;
                case 6: //������λ�÷��˳���Ϸ
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPExit", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("ĿǰAPP�ĸ����ţ� " + point.Item2 + "��ĿǰAPP�ĸ��λ�ã� " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;
        }
    }

    }

    void Update()
    {
        //������ڳ����������ӳ���ʱ��
        if (isPressing && !isDraggingMode)
        {
            pressDuration += Time.deltaTime;
            print(pressDuration);
        }
        else//���û����������ʱ����0
        {
            if (pressDuration!=0)
            {
                pressDuration = 0;
            }
        }
        //�����������Ԥ��ʱ�䣬�ͽ�����קģʽ
        if (pressDuration >= pressingTime)
        {
            isDraggingMode = true;
            print("������קģʽ");
        }

        if (transformAPP != null) //���ί�����к���
        {
            print("����ִ��ί�к���");
            transformAPP.Invoke();
        }

        if (isDraggingMode)//�������קģʽ��������ť��ͼ�����
        {
            foreach(var app in DicApps)
            {
                float r = Random.Range(-10, 10);
                RectTransform appRect =  app.Key.GetComponent<RectTransform>();
                appRect.eulerAngles = new Vector3(0,0, 90 + r);
            }
        }


    }

    protected override void ClickBtn(string btnName)
    {
        base.ClickBtn(btnName);
        //һ���ص㣺��ť�����������¼������Լ�ʹ������ű���ʵ����IPointerDownHandler����Ҳ��������ͨ��EventSystem����õ������¼���
        //�����¼�ֻ���ڰ�ť�������أ����ǿ����ڱ��������յ�������
        print("�����˰�ť");
        switch (btnName)
        {
            case "ButtonBack":
                MusicMgr.Instance.PlaySound("Yes Button");
                UIMgr.Instance.HidePanel<PanelPhoneHome>();
                UIMgr.Instance.ShowPanel<PanelMain>();
                break;
        }
    }



    private void ExitDraggingMode()
    {
        isDraggingMode = false;
        foreach (var app in DicApps)
        {
            RectTransform appRect = app.Key.GetComponent<RectTransform>();
            appRect.eulerAngles = new Vector3(0, 0, 90);
        }
    }


    /// <summary>
    /// �϶�����ʱ��Ĳ�����
    /// todo:�϶�������������
    /// 0.��ť���ࣺ�ؿ�APP��ť�������͵�APP��ť�����������е�APP�İ�ť
    /// 1.ҳ������������ҵ��϶��������϶�����ﵽһ��������л�ҳ��
    /// 2.��ť�϶������������ҵĳ�������������ĳ��button�ﵽһ��ʱ��֮�������϶�button״̬�����ͨ����ĳ����ť���˳����϶�button״̬��������ҳ�������button��������image�᲻���������������������������ֻ���Ļ��
    /// ��⵽������ֵ�ʱ�����ĸ��������ĵ������Ȼ�󽫰�ť����ƽ���ƶ�����Ӧ���ӵ����ģ�ͬʱ��ԭ������������強���ŵ���һ��������ݹ�ذ�˳����������強����һ���������
    /// 3.���𵽹������õİ�ť�Ĵ�����ɾ��������
    /// 
    /// todo:�����϶����
    /// 1.Ϊ��廮�ָ�㣬��Ϊ������� ��
    /// 2.����ť�����߼� ��
    /// 3.�����밴ť�����׶ε��л����� ��
    /// 4.���϶���ť�ķ��� ��
    /// 5.���ɿ���ťʱ�İ�ť������ķ��� ��
    /// 6.����ť���򷽷� ��
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        //�涨��������ק��APPͼ�꣬�����趨tagΪ��Draggle����
        if (eventData.pointerEnter != null)
        {

            print(eventData.pointerEnter.tag);

            if ((isDraggingMode && eventData.pointerEnter.tag == "Draggable") || isSelected)//���������˿���ק�����岢�ҵ��ʱ�����Ԥ��ʱ��
            {
                isDragging = true;
                print("��������קģʽ");
            }
            else
            {
                isDragging = false;
                print("�����Ƿ���קģʽ");
            }

            //�����Ұ�������קͼ�꣬��ͼ�������Ҵ���λ��
            if (isDragging && isDraggingMode && transformAPP == null)
            {
                if (!isSelected)
                {
                    selected = eventData.pointerEnter;
                    isSelected = true;
                }
                selected.transform.position = eventData.position;
                //��������϶���������Ҳ��볤������
                isPressing = false;

            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        print("̧����");
        isPressing = false;
        if (!isDraggingMode)
        {
            string realName = eventData.pointerEnter.name.Replace("(Clone)", "").Trim();
            print(realName);
            switch (realName)
            {
                case "APPTutorials":
                    UIMgr.Instance.ShowPanel<PanelTip>(E_UILayer.Top);
                    break;
                case "APPShooping":
                    UIMgr.Instance.ShowPanel<PanelStore>(E_UILayer.Top);
                    break;
                case "APPForums":
                    UIMgr.Instance.ShowPanel<PanelForums>(E_UILayer.Top);
                    break;
                case "APPShortVideo":
                    UIMgr.Instance.ShowPanel<PanelShortVideo>(E_UILayer.Top);
                    break;
                case "APPStaffList":
                    //todo:��ת����Ա����ҳ��
                    UIMgr.Instance.HidePanel<PanelPhoneHome>();
                    UIMgr.Instance.ShowPanel<PanelStaff>();
                    break;
                case "APPExit":
                    Application.Quit();
                    break;
            }
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("������");
        MusicMgr.Instance.PlaySound("Selection");
        print(eventData.pointerEnter.name);
        //������µ��������������壬���˳��϶�ģʽ
        if (eventData.pointerEnter.tag != "Draggable")
        {
            ExitDraggingMode();
        }
        else
        {
            isPressing = true;
        }

    }

    //��ʼ��ק
    public void OnEndDrag(PointerEventData eventData)
    {

        if (isDraggingMode && isDragging)//���������קģʽ
        {
            //��ʼ����Ŀǰ������������ĸ��
            float distanceMin = 100000000;
            Vector2 positionNow = Vector2.zero;
            Vector2 selectedPosition = new Vector2(selected.transform.localPosition.x, selected.transform.localPosition.y);
            int pointNumber = 10000;
            foreach (var point in points)
            {
                float distanceNow = Vector2.Distance(selectedPosition, point.Item1);
                print("���ڽ�������ק����ק�����ǣ�" + selected.name);
                //!!!!!!!!!!!!!!!!!!14��һ�������λ�ã����λ���з�����Ϸ����ҳ��İ�ť�����Բ����ڵ�
                if (distanceNow <= distanceMin && !DicApps.ContainsValue(point.Item2) && point.Item2!=14) //����distanceMin�㹻������һ����������һ���������if��
                {
                    distanceMin = distanceNow;
                    positionNow = point.Item1;
                    pointNumber = point.Item2;
                }
            }

            print("���ڵ����λ��x" + positionNow.x + "���ڵ����λ��y" + positionNow.y);

            Vector3 targetPostion = new Vector3(positionNow.x, positionNow.y, selected.transform.localPosition.z);

            if (transformAPP == null)//ֻ��transformAPPί��Ϊ�յ�ʱ��Ŵ����ƶ�APP���¼�
            {
                print("����ί����");
                //��ƽ���ƶ�Ч��.�����ί�У���update���������
                transformAPP += () =>
                {
                    if (isFirstRun) //��һ�����У����ϵ�ʱ�������λ��
                    {
                        isFirstRun = false;
                        nowAppPostion = selected.transform.localPosition;
                        //ת��APPλ��
                        DicApps[selected.gameObject] = pointNumber;
                        print("!!!!!!!!!!!" + selected.gameObject.name);
                        print("!!!!!!!!!!!" + pointNumber);

                        if (selected.gameObject.name == "APPExit(Clone)" && pointNumber == 15)
                        {
                            Level.Instance.SaveLevel(3, 2);
                            Level.Instance.LevelBig = 3;
                            Level.Instance.LevelSmall = 2;

                            print("�Ѿ�����ȫ���ؿ�");
                        }
                        print("��ǰ�����λ����" + pointNumber);
                    }
                    if (Vector3.Distance(targetPostion, selected.transform.localPosition) >= 0.5)
                    {
                        appTime += Time.deltaTime;
                        selected.transform.localPosition = Vector3.Slerp(nowAppPostion, targetPostion, appTime/ allTime);

                        //todo:������дAPP˳��ı����߼�

                    }
                    else
                    {
                        selected.transform.localPosition = targetPostion;
                        transformAPP = null;
                        isFirstRun = true;
                        appTime = 0;
                        print("����û����ק������");
                    }
                };
            }

            isSelected = false;
            //todo:�����ְ�ť���������ŵķ���

            //selected.transform
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
