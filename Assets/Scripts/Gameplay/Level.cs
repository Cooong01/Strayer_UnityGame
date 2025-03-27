using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// �ؿ�������
/// </summary>
public class Level : SingletonMono<Level>
{
    public List<string> bigLevels = new List<string> { "Tutoral", "Forum", "Store","ShortVideo"};

    //��Ϸ�����õĹؿ���Ϣ�����Ǵ������ֵ��С��Ԥ������
    public Dictionary<string, List<string>> levelDetail = new Dictionary<string, List<string>> { 
    {"Tutoral",new List<string> { "level0-1", "level0-2", "level0-3"} },
    {"Forum",new List<string> { "level1-1", "level1-2", "level1-3" } },
    {"Store",new List<string> { "level2-1", "level2-2", "level2-3" } },
    {"ShortVideo",new List<string> { "level3-1", "level3-2", "level3-3" } }};
    
    //�Ѿ�����Ľ���:���ڽ��е�����һ��
    public int LevelBig;
    public int LevelSmall;

    //�������ڽ��еĴ�غ�С��
    public int nowLevelBig;
    public int nowLevelSmall;

    private string levelDic = "level"; //level���ļ�������

    public GameObject nowLevelObj;

    public GameObject nextLevelObj;

    public GameObject player; //��Ҷ���


    public GameObject start; //ÿһ����һ����ʼ��ǣ����ھ�����ҽ���˹صı�ǣ��Լ����ⲿ����˹ؿ��ĳ�ʼ��λ��

    public GameObject end; //ÿһ����һ����ɱ�ǡ�������ɱ��û�гɹ���壬���Ǽ�����һ��

    public GameObject nowAirWall; //ÿһ�ؼ�һ������ǽ������start�Ժ����ǽactive��true������endʱ����ǽactive��false��

    public GameObject nextAirWall;

    public float offSet = 96; //ÿһ�ص�ƫ��ֵ




    //����ע�ͣ���һλ������д
    //NPC������Ϣ�����Ǵ������ֵ��С��Ԥ������
    public Dictionary<string, List<string>> NpcNum = new Dictionary<string, List<string>> {
    {"PanelTip(Clone)",new List<string> { " ", " ", " "} },
    {"PanelForums(Clone)",new List<string> { "0/1", "0/1", "0/1" } },
    {"PanelStore(Clone)",new List<string> { "0/14", "0/1", "0/2" } },
    {"PanelShortVideo(Clone)",new List<string> { "0/2", "0/2", "0/1" } }};



    //��Ϸ�տ�ʼ��ʱ�򼴼��أ�����ұ��ش洢�ŵĹؿ���Ϣ
    private void Start()
    {
        //��ȡ��ǰ�ؿ�����
        LevelBig = PlayerPrefs.GetInt("LevelBig", 0);
        LevelSmall = PlayerPrefs.GetInt("LevelSmall", 0);
        print("��ǰ����Ľ��ȣ�" + LevelBig + "," + LevelSmall);
        player.transform.position = player.GetComponent<PlayerController>().parameter.defaultPosition;
    }

    //�������������һ�ص����̵����������
    public void EnterNextLevel()
    {
        SetNpcNum();
        player.GetComponent<PlayerController>().parameter.isEnteringNextLevel = true;
        //Ȼ�������һ�ء�����ƶ��������ڡ�
        InitNextLevel(LevelNext(nowLevelBig, nowLevelSmall).Item1, LevelNext(nowLevelBig, nowLevelSmall).Item2);
    }

    //�ٳ�ʼ��һ�ε�ǰ�Ĺؿ�
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
            nowAirWall.SetActive(true);//������һ�صĿ���ǽ

            nowAirWall = nowLevelObj.transform.Find("AirWall").gameObject;
            nowAirWall.SetActive(true); //������һ�صĿ���ǽ

            player.GetComponent<PlayerController>().StopMove();
            EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 1);
            EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 2);
            EventCenter.Instance.EventTrigger(E_EventType.E_PressMirrorRecycleButton, 3);
        }

    }

    /// <summary>
    /// ��ʼ���ؿ��ķ������첽���عؿ�����ùؿ���ʼ������ı�ǣ����ùؿ������ʵ�λ��
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
            player.transform.position = start.transform.position; //���λ�ó�ʼ��
            nowLevelBig = big;
            nowLevelSmall = small;
            ; },isSync:true);
    }

    /// <summary>
    /// ��ʼ����һ�صķ�����
    /// ʵ�֣��첽���عؿ�����ùؿ���ʼ������ı�ǣ����ؿ����뵽���ʵ�λ��
    /// </summary>
    private void InitNextLevel(int big, int small)
    {
        print("��ǰ����Ĵ�ؽ���" + LevelBig);
        print("��ǰ�����С�ؽ���" + LevelSmall);
        print("��ǰ����Ĵ��" + big);
        print("��ǰ�����С��" + small);
        if (LevelBig < big)
        {
            LevelBig = big;
            LevelSmall = 0;
            print("������+" + big + small);
            SaveLevel(big, small);
        }
        else
        {
            if (LevelBig == big)
            {
                if (LevelSmall < small)
                {
                    LevelSmall = small;
                    print("������+" + big + small);
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
    /// ���¿�ʼ���ء�չʾ�ؿ���壬ж�ر��أ����¼��ر��ء�
    /// </summary>
    public void restart()
    {
        string prefabName = levelDetail[bigLevels[nowLevelBig]][nowLevelSmall];
        UIMgr.Instance.ShowPanel<PanelLoad>();
        Destroy(nowLevelObj); //ж�ر���

        //���ر���
        ABResMgr.Instance.LoadResAsync<GameObject>(levelDic, prefabName, (t) => {
            start = t.transform.Find("Start").gameObject;
            end = t.transform.Find("End").gameObject;
            nowAirWall = t.transform.Find("AirWall").gameObject;
            nowAirWall.SetActive(true);
            player.transform.position = start.transform.position; //���λ�ó�ʼ��
            UIMgr.Instance.HidePanel<PanelLoad>();
        });

    }

    /// <summary>
    /// �ɹ�ͨ�صķ������ɹ�ͨ�أ������⵽ͨ�ر���Ժ�ʱ�����ȼ�����һ���ؿ�Ԥ���壬��Ԥ����ŵ��Եĵط���Ȼ��
    /// ������Զ��ƶ���������Զ��ƶ�����һ���ؿ���start��Ǻ�ж����һ�أ�����Զ��ƶ�����һ���ؿ��ķ�Χ�ڣ��Ҳ�������ҷ��ء�
    /// </summary>
    public void SuccessLevel()
    {
        SaveLevel(nowLevelBig,nowLevelSmall);
        (nowLevelBig,nowLevelSmall) = LevelNext(nowLevelBig, nowLevelSmall);
        
    }

    /// <summary>
    /// �ҵ���һ���ؿ�
    /// </summary>
    /// <param name="big">�����</param>
    /// <param name="small">С��Ԥ������</param>
    public (int,int) LevelNext(int big,int small)
    {
        string nowBigOrder = bigLevels[big];

        if(small == levelDetail[nowBigOrder].Count - 1) //�����ǰС����Ŵﵽ��ǰС�ص����ֵ��������һ����صĵ�һ��С��
        {
            if(big == bigLevels.Count) //��������һ���
            {
                return (100,100); //����ɹ�
            }
            else //����һ��ص�һС��
            {
                return (big + 1,0);
            }
        }
        else
        {
            return (big, small + 1);
        }
    }

    //����ؿ�
    public void SaveLevel(int big, int small)
    {
        PlayerPrefs.SetInt("LevelBig", big);
        PlayerPrefs.SetInt("LevelSmall", small);

    }

    //ɾ����ǰ�ؿ�
    public void DestroyLevel()
    {
        Destroy(nowLevelObj);
        //��ɫ�ٶ���գ��ص�Ĭ��λ�ã������ع�Ĭ��״̬
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

    // -----дAI�ĳ�������������δ֪-----
    void SetNpcNum() {
        CheckResult end = GameObject.FindFirstObjectByType<CheckResult>();
        string LevelNum = "";
        string input = end.transform.parent.parent.name;
        // �ҵ� '-' ��λ��
        int dashIndex = input.IndexOf('-');

        if (dashIndex != -1)
        {
            // ��ȡ�� '-' ֮ǰ�Ĳ���
            string beforeDash = input.Substring(0, dashIndex);

            // ʹ��������ʽ��ȡ���һ�������ַ�
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
                // ��ȡ '-' ����Ĳ���
                string afterDash = input.Substring(index + 1);

                // ����Ը�����Ҫ��һ������ afterDash
                // ����������У����Ǽ��� afterDash ����������
                int x = int.Parse(afterDash.Replace("(Clone)", "")) - 1;

                int y = int.Parse(Data[x].Substring(0, Data[x].IndexOf('/')));

                if (end.NowNum>=y) {
                    Data[x] = $"{end.NowNum }/{end.EnemyNum}";
                }
               
            }
        }
    }
}
