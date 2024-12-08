using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
/// <summary>
/// 用来存储按钮平滑移动操作的委托
/// </summary>
public delegate void TransformAPP();

/// <summary>
/// 手机Home页面面板
/// todo:做手机状态栏效果、手机页面滑动效果、按钮长按拖动效果
/// </summary>
public class PanelPhoneHome : BasePanel, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler,IPointerClickHandler
{
    private float baseWidth = 3200;
    private float baseHeight = 2048;

    private float nowWidth;
    private float nowHeight;


    //注意：由于手机页面是竖屏的，所以手机界面的正视角是从右侧边往左侧边来看。本脚本中，”行“和”列“都是unity画面上的行列，不是模拟的手机画面的意义上的行列。
    //一个页面里的可容纳APP行列数
    public int rowNumber = 3;
    public int columnNumber = 2;

    //这是左右和上下的空缺距离
    private float leftAndRightSafetyValue;
    private float TopAndButtomSafetyValue;

    private float leftAndRightSafetyRadio = 0.25f;
    private float TopAndButtomSafetyRadio = 0.25f;

    //APP自动移动到格点的速度
    public int transformSepeed;


    //切换为拖拽模式的时间
    public float pressingTime = 2;

    //松手后APP自动移动到对应位置的速度
    public float transformSpeed = 0.01f;

    //此页APP的数量
    public int APPNumber = 10;

    //正在拖拽可拖拽物体的标识。
    private bool isDragging = false;

    //玩家按屏幕时的坐标
    private Vector2 pressPosition;
    private bool isPressing = false;
    private float pressDuration = 0;

    //进入拖拽模式的标志
    private bool isDraggingMode = false;

    //在拖拽模式被选中的APP
    private GameObject selected;

    //分别是格点位置和对应的序号
    private List<(Vector2,int)> points;

    //现在正在改变位置的APP
    private event TransformAPP transformAPP = null;

    private Vector3 targetPostion;

    //分别是APP的实例和该实例对应的格点序号
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
        //根据APP行列数以及panel大小，算出一个包含着各种格点中心的position的列表
        Vector3 panelScale = new Vector3(Screen.width, Screen.height, 0);
        nowWidth = Screen.width;
        nowHeight = Screen.height;
        leftAndRightSafetyValue = (int)(leftAndRightSafetyRadio * nowWidth);
        TopAndButtomSafetyValue = (int)(TopAndButtomSafetyRadio * nowHeight);
        print("panel宽度" + panelScale.x);
        print("panel高度" + panelScale.y);
        points = new List<(Vector2,int)>();
        //当前panel的坐标（用来适配处理格点位置偏移）
        Vector3 panelPosition = this.transform.GetComponent<RectTransform>().position;
        print("x坐标"+panelPosition.x);
        print("y坐标" + panelPosition.y);

        //两个坐标格点之间的间距。用来做格点的居中.
        //行间距
        float spaceRow = ((panelScale.x - leftAndRightSafetyValue) / columnNumber) ;
        //列间距
        float spaceCol = ((panelScale.y - TopAndButtomSafetyValue) / rowNumber);
        int pointNumber = 0;

        //此处根据手机页面的逻辑行列来排布，即看手机的时候的行列
        for (int col = 1; col <= columnNumber; col++)
        {
            float x = spaceRow * col;//这一列的x坐标
            print("x" + x);
            for (int row = 1; row <= rowNumber; row++)
            {
                float y = spaceCol * row;//这一行的y坐标
                print("y" + y);
                Vector2 pointPosition = new Vector2(x - panelPosition.x - spaceRow / 2 + leftAndRightSafetyValue / 2, y - panelPosition.y - spaceCol / 2 +TopAndButtomSafetyValue / 2);
                points.Add((pointPosition, ++pointNumber));
                print("目前格点序号： " + pointNumber + "，目前格点位置： " + pointPosition.x + "   " + pointPosition.y);

                ////加载测试格点位置的图片
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
            //todo：这里改成用提前保存好的列表来读取物体
            switch (point.Item2)
            {
                case 1: //第一个位置放教学关
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPTutorials", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("目前APP的格点序号： " + point.Item2 + "，目前APP的格点位置： " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;
                case 2: //第二个位置放论坛关
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPForums", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("目前APP的格点序号： " + point.Item2 + "，目前APP的格点位置： " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;


                case 3: //第三个位置放购物关
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPShooping", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("目前APP的格点序号： " + point.Item2 + "，目前APP的格点位置： " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;

                case 4: //第四个位置放短视频关
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPShortVideo", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("目前APP的格点序号： " + point.Item2 + "，目前APP的格点位置： " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;
                case 5: //第五个位置放制作人员名单
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPStaffList", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("目前APP的格点序号： " + point.Item2 + "，目前APP的格点位置： " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;
                case 6: //第六个位置放退出游戏
                    ABResMgr.Instance.LoadResAsync<GameObject>("apps", "APPExit", callBack: (t) =>
                    {
                        t.transform.GetComponent<RectTransform>().anchoredPosition = point.Item1;
                        GameObject app = GameObject.Instantiate(t, this.transform);
                        app.transform.localScale *= nowWidth / baseWidth;
                        DicApps.Add(app, point.Item2);
                        print("目前APP的格点序号： " + point.Item2 + "，目前APP的格点位置： " + point.Item1.x + "   " + point.Item1.y);
                    });
                    break;
        }
    }

    }

    void Update()
    {
        //如果正在长按，就增加长按时间
        if (isPressing && !isDraggingMode)
        {
            pressDuration += Time.deltaTime;
            print(pressDuration);
        }
        else//如果没长按，长按时间清0
        {
            if (pressDuration!=0)
            {
                pressDuration = 0;
            }
        }
        //如果长按超过预设时间，就进入拖拽模式
        if (pressDuration >= pressingTime)
        {
            isDraggingMode = true;
            print("进入拖拽模式");
        }

        if (transformAPP != null) //如果委托中有函数
        {
            print("正在执行委托函数");
            transformAPP.Invoke();
        }

        if (isDraggingMode)//如果在拖拽模式，各个按钮的图标颤动
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
        //一个重点：按钮会拦截输入事件，所以即使在这个脚本里实现了IPointerDownHandler，它也不能正常通过EventSystem来获得到按下事件。
        //按下事件只会在按钮处被拦截，但是可以在本函数里收到并处理。
        print("按下了按钮");
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
    /// 拖动面板的时候的操作。
    /// todo:拖动面板操作解析：
    /// 0.按钮分类：关卡APP按钮、仅观赏的APP按钮、正在下载中的APP的按钮
    /// 1.页面操作：检测玩家的拖动操作，拖动距离达到一定距离后切换页面
    /// 2.按钮拖动操作：检测玩家的长按操作，长按某个button达到一定时间之后进入可拖动button状态（最后通过按某个按钮来退出可拖动button状态），整个页面的所有button的子物体image会不由自主抖动。按照网格来划分手机屏幕。
    /// 检测到玩家松手的时候看离哪个格子中心点最近，然后将按钮坐标平滑移动到对应格子的中心，同时把原本格子里的物体挤开放到下一个格子里（递归地按顺序把所有物体挤到下一个格子里）。
    /// 3.仅起到观赏作用的按钮的创建和删除操作。
    /// 
    /// todo:开发拖动面板
    /// 1.为面板划分格点，并为格点排序 √
    /// 2.做按钮长按逻辑 √
    /// 3.做进入按钮操作阶段的切换方法 √
    /// 4.做拖动按钮的方法 √
    /// 5.做松开按钮时的按钮进入格点的方法 √
    /// 6.做按钮排序方法 √
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        //规定：可以拖拽的APP图标，必须设定tag为“Draggle”。
        if (eventData.pointerEnter != null)
        {

            print(eventData.pointerEnter.tag);

            if ((isDraggingMode && eventData.pointerEnter.tag == "Draggable") || isSelected)//如果点击到了可拖拽的物体并且点击时间大于预设时间
            {
                isDragging = true;
                print("现在是拖拽模式");
            }
            else
            {
                isDragging = false;
                print("现在是非拖拽模式");
            }

            //如果玩家按到可拖拽图标，则图标跟随玩家触摸位置
            if (isDragging && isDraggingMode && transformAPP == null)
            {
                if (!isSelected)
                {
                    selected = eventData.pointerEnter;
                    isSelected = true;
                }
                selected.transform.position = eventData.position;
                //如果发生拖动，代表玩家不想长按，则：
                isPressing = false;

            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        print("抬起了");
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
                    //todo:跳转到人员名单页面
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
        print("按下了");
        MusicMgr.Instance.PlaySound("Selection");
        print(eventData.pointerEnter.name);
        //如果按下的物体是闲杂物体，就退出拖动模式
        if (eventData.pointerEnter.tag != "Draggable")
        {
            ExitDraggingMode();
        }
        else
        {
            isPressing = true;
        }

    }

    //开始拖拽
    public void OnEndDrag(PointerEventData eventData)
    {

        if (isDraggingMode && isDragging)//如果处于拖拽模式
        {
            //开始找离目前物体坐标最近的格点
            float distanceMin = 100000000;
            Vector2 positionNow = Vector2.zero;
            Vector2 selectedPosition = new Vector2(selected.transform.localPosition.x, selected.transform.localPosition.y);
            int pointNumber = 10000;
            foreach (var point in points)
            {
                float distanceNow = Vector2.Distance(selectedPosition, point.Item1);
                print("正在进行真拖拽，拖拽物体是：" + selected.name);
                //!!!!!!!!!!!!!!!!!!14是一个特殊的位置，这个位置有返回游戏进入页面的按钮，所以不能遮挡
                if (distanceNow <= distanceMin && !DicApps.ContainsValue(point.Item2) && point.Item2!=14) //由于distanceMin足够大，所以一定会至少有一次走入这个if内
                {
                    distanceMin = distanceNow;
                    positionNow = point.Item1;
                    pointNumber = point.Item2;
                }
            }

            print("现在的最近位置x" + positionNow.x + "现在的最近位置y" + positionNow.y);

            Vector3 targetPostion = new Vector3(positionNow.x, positionNow.y, selected.transform.localPosition.z);

            if (transformAPP == null)//只有transformAPP委托为空的时候才传入移动APP的事件
            {
                print("传入委托了");
                //做平滑移动效果.传这个委托，在update方法里调用
                transformAPP += () =>
                {
                    if (isFirstRun) //第一次运行，加上当时的物体的位置
                    {
                        isFirstRun = false;
                        nowAppPostion = selected.transform.localPosition;
                        //转换APP位置
                        DicApps[selected.gameObject] = pointNumber;
                        print("!!!!!!!!!!!" + selected.gameObject.name);
                        print("!!!!!!!!!!!" + pointNumber);

                        if (selected.gameObject.name == "APPExit(Clone)" && pointNumber == 15)
                        {
                            Level.Instance.SaveLevel(3, 2);
                            Level.Instance.LevelBig = 3;
                            Level.Instance.LevelSmall = 2;

                            print("已经解锁全部关卡");
                        }
                        print("当前存入的位置是" + pointNumber);
                    }
                    if (Vector3.Distance(targetPostion, selected.transform.localPosition) >= 0.5)
                    {
                        appTime += Time.deltaTime;
                        selected.transform.localPosition = Vector3.Slerp(nowAppPostion, targetPostion, appTime/ allTime);

                        //todo:在这里写APP顺序的保存逻辑

                    }
                    else
                    {
                        selected.transform.localPosition = targetPostion;
                        transformAPP = null;
                        isFirstRun = true;
                        appTime = 0;
                        print("现在没有拖拽物体了");
                    }
                };
            }

            isSelected = false;
            //todo:做各种按钮依次往后排的方法

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
