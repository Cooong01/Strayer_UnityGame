using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// 关卡管理器
/// </summary>
public class Level : SingletonMono<Level>
{
    public List<string> bigLevels = new List<string> { "Tutoral", "Forum", "Store","ShortVideo"};

    //游戏内配置的关卡信息，键是大关名，值是小关预制体名
    public Dictionary<string, List<string>> levelDetail = new Dictionary<string, List<string>> { 
    {"Tutoral",new List<string> { "level0-1", "level0-2", "level0-3"} },
    {"Forum",new List<string> { "level1-1", "level1-2", "level1-3" } },
    {"Store",new List<string> { "level2-1", "level2-2", "level2-3" } },
    {"ShortVideo",new List<string> { "level3-1", "level3-2", "level3-3" } }};
    
    //已经保存的进度:正在进行的最新一关
    public int LevelBig;
    public int LevelSmall;

    //现在正在进行的大关和小关
    public int nowLevelBig;
    public int nowLevelSmall;

    private string levelDic = "level"; //level的文件夹名字

    public GameObject nowLevelObj;

    public GameObject nextLevelObj;

    public GameObject player; //玩家对象


    public GameObject start; //每一关有一个开始标记，用于决定玩家进入此关的标记，以及从外部进入此关卡的初始化位置

    public GameObject end; //每一关有一个完成标记。触发完成标记没有成功面板，而是加载下一关

    public GameObject nowAirWall; //每一关加一个空气墙，进入start以后空气墙active是true，进入end时空气墙active是false。

    public GameObject nextAirWall;

    public float offSet = 96; //每一关的偏移值




    //补充注释：另一位程序所写
    //NPC配置信息，键是大关名，值是小关预制体名
    public Dictionary<string, List<string>> NpcNum = new Dictionary<string, List<string>> {
    {"PanelTip(Clone)",new List<string> { " ", " ", " "} },
    {"PanelForums(Clone)",new List<string> { "0/1", "0/1", "0/1" } },
    {"PanelStore(Clone)",new List<string> { "0/14", "0/1", "0/2" } },
    {"PanelShortVideo(Clone)",new List<string> { "0/2", "0/2", "0/1" } }};



    //游戏刚开始的时候即加载，找玩家本地存储着的关卡信息
    private void Start()
    {
        //读取当前关卡进度
        LevelBig = PlayerPrefs.GetInt("LevelBig", 0);
        LevelSmall = PlayerPrefs.GetInt("LevelSmall", 0);
        print("当前保存的进度：" + LevelBig + "," + LevelSmall);
        player.transform.position = player.GetComponent<PlayerController>().parameter.defaultPosition;
    }

    //用来管理进入下一关的流程的主函数入口
    public void EnterNextLevel()
    {
        SetNpcNum();
        player.GetComponent<PlayerController>().parameter.isEnteringNextLevel = true;
        //然后加载下一关。玩家移动包含在内。
        InitNextLevel(LevelNext(nowLevelBig, nowLevelSmall).Item1, LevelNext(nowLevelBig, nowLevelSmall).Item2);
    }

    //再初始化一次当前的关卡
    public void EnterThisLevel()
    {
        InitLevel(nowLevelBig,nowLevelSmall);
    }

    public void EnterLevel(int big, int small)
    {
        InitLevel(big, small);
    }

    public void EnteredNextLevel()
    {
        MusicMgr.Instance.PlaySound("Victory");
        if (nextLevelObj != null)
        {
            Destroy(nowLevelObj);
            nowLevelObj = nextLevelObj;
            nowAirWall.SetActive(true);//开启上一关的空气墙

            nowAirWall = nowLevelObj.transform.Find("AirWall").gameObject;
            nowAirWall.SetActive(true); //开启这一关的空气墙

            player.GetComponent<PlayerController>().StopMove();
            EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 1);
            EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 2);
            EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 3);
        }

    }

    /// <summary>
    /// 初始化关卡的方法。异步加载关卡，获得关卡开始或结束的标记，放置关卡到合适的位置
    /// </summary>
    private void InitLevel(int big, int small)
    {
        string prefabName = levelDetail[bigLevels[big]][small];
        print(prefabName);

        ABResMgr.Instance.LoadResAsync<GameObject>(levelDic, prefabName, (t) => {
            print(levelDic);
            print(prefabName);
            ShaderABLoad.Instance.Test(t);
            nowLevelObj = Instantiate(t);
            player.SetActive(true);
            print(nowLevelObj.name);
            start = nowLevelObj.transform.Find("Start").gameObject;
            end = nowLevelObj.transform.Find("End").gameObject;
            nowAirWall = nowLevelObj.transform.Find("AirWall").gameObject;
            nowAirWall.SetActive(true);
            player.transform.position = start.transform.position; //玩家位置初始化
            nowLevelBig = big;
            nowLevelSmall = small;
            ; },isSync:true);
    }

    /// <summary>
    /// 初始化下一关的方法。
    /// 实现：异步加载关卡，获得关卡开始或结束的标记，将关卡对齐到合适的位置
    /// </summary>
    private void InitNextLevel(int big, int small)
    {
        print("当前保存的大关进度" + LevelBig);
        print("当前保存的小关进度" + LevelSmall);
        print("当前传入的大关" + big);
        print("当前传入的小关" + small);
        if (LevelBig < big)
        {
            LevelBig = big;
            LevelSmall = 0;
            print("保存了+" + big + small);
            SaveLevel(big, small);
        }
        else
        {
            if (LevelBig == big)
            {
                if (LevelSmall < small)
                {
                    LevelSmall = small;
                    print("保存了+" + big + small);
                    SaveLevel(big, small);
                }
            }
        }
        string prefabName = levelDetail[bigLevels[big]][small];
        print(prefabName);
        nowAirWall.SetActive(false);
        ABResMgr.Instance.LoadResAsync<GameObject>(levelDic, prefabName, (t) => {
            float x = nowLevelObj.transform.position.x;
            nextLevelObj = Instantiate(t);
            nextLevelObj.transform.position = new Vector3( x + offSet, nowLevelObj.transform.position.y, nowLevelObj.transform.position.z);
            player.SetActive(true);
            start = nextLevelObj.transform.Find("Start").gameObject;
            end = nextLevelObj.transform.Find("End").gameObject;
            nextAirWall = nextLevelObj.transform.Find("AirWall").gameObject;
            nextAirWall.SetActive(false);
            nowLevelBig = big;
            nowLevelSmall = small;
            
        });
    }



    /// <summary>
    /// 重新开始本关。展示关卡面板，卸载本关，重新加载本关。
    /// </summary>
    public void restart()
    {
        string prefabName = levelDetail[bigLevels[nowLevelBig]][nowLevelSmall];
        UIMgr.Instance.ShowPanel<PanelLoad>();
        Destroy(nowLevelObj); //卸载本关

        //加载本关
        ABResMgr.Instance.LoadResAsync<GameObject>(levelDic, prefabName, (t) => {
            start = t.transform.Find("Start").gameObject;
            end = t.transform.Find("End").gameObject;
            nowAirWall = t.transform.Find("AirWall").gameObject;
            nowAirWall.SetActive(true);
            player.transform.position = start.transform.position; //玩家位置初始化
            UIMgr.Instance.HidePanel<PanelLoad>();
        });

    }

    /// <summary>
    /// 成功通关的方法。成功通关（物体检测到通关标记以后）时，首先加载下一个关卡预制体，把预制体放到对的地方，然后
    /// 让玩家自动移动，在玩家自动移动到下一个关卡的start标记后，卸载上一关，相机自动移动到下一个关卡的范围内，且不允许玩家返回。
    /// </summary>
    public void SuccessLevel()
    {
        SaveLevel(nowLevelBig,nowLevelSmall);
        (nowLevelBig,nowLevelSmall) = LevelNext(nowLevelBig, nowLevelSmall);
        
    }

    /// <summary>
    /// 找到下一个关卡
    /// </summary>
    /// <param name="big">大关名</param>
    /// <param name="small">小关预制体名</param>
    public (int,int) LevelNext(int big,int small)
    {
        string nowBigOrder = bigLevels[big];

        if(small == levelDetail[nowBigOrder].Count - 1) //如果当前小关序号达到当前小关的最大值，就找下一个大关的第一个小关
        {
            if(big == bigLevels.Count) //如果是最后一大关
            {
                return (100,100); //代表成功
            }
            else //找下一大关第一小关
            {
                return (big + 1,0);
            }
        }
        else
        {
            return (big, small + 1);
        }
    }

    //保存关卡
    public void SaveLevel(int big, int small)
    {
        PlayerPrefs.SetInt("LevelBig", big);
        PlayerPrefs.SetInt("LevelSmall", small);

    }

    //删除当前关卡
    public void DestroyLevel()
    {
        Destroy(nowLevelObj);
        //角色速度清空，回到默认位置，参数回归默认状态
        player.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.transform.position = player.GetComponent<PlayerController>().parameter.defaultPosition;
        player.GetComponent<PlayerController>().StopMove();
        EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 1);
        EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 2);
        EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 3);
    }


    public void ShowAirWall()
    {
        
    }

    public void HideAirWall()
    {

    }

    // -----写AI的程序所负责，作用未知-----
    void SetNpcNum() {
        CheckResult end = GameObject.FindFirstObjectByType<CheckResult>();
        string LevelNum = "";
        string input = end.transform.parent.parent.name;
        // 找到 '-' 的位置
        int dashIndex = input.IndexOf('-');

        if (dashIndex != -1)
        {
            // 截取到 '-' 之前的部分
            string beforeDash = input.Substring(0, dashIndex);

            // 使用正则表达式提取最后一个数字字符
            Match match = Regex.Match(beforeDash, @"\d+$");
            if (match.Success)
            {
                string numberBeforeDash = match.Value;

                switch (numberBeforeDash)
                {
                    case "1":
                        LevelNum = "PanelForums(Clone)";
                        break;
                    case "2":
                        LevelNum = "PanelStore(Clone)";
                        break;
                    case "3":
                        LevelNum = "PanelShortVideo(Clone)";
                        break;
                }
            }

            if (!NpcNum .ContainsKey(LevelNum)) {
                return;
            
            }
            List<string> Data = NpcNum[LevelNum];

            int index = input.IndexOf('-');

            if (index != -1 && index + 1 < input.Length)
            {
                // 截取 '-' 后面的部分
                string afterDash = input.Substring(index + 1);

                // 你可以根据需要进一步处理 afterDash
                // 在这个例子中，我们假设 afterDash 仅包含数字
                int x = int.Parse(afterDash.Replace("(Clone)", "")) - 1;

                int y = int.Parse(Data[x].Substring(0, Data[x].IndexOf('/')));

                if (end.NowNum>=y) {
                    Data[x] = $"{end.NowNum }/{end.EnemyNum}";
                }
               
            }
        }
    }
}
