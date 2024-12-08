using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public enum Talking
{
    StartTalking,
    Talking,
    NotTalking

}

[System.Serializable]
public class ListData
{
    public string name;

    public List<string> Data;

    public void  SetData(List<string> items)
    {
        this.Data = items;
    }
}

[System.Serializable]
public class StringList
{
    public List<ListData> items;

}

public class TaikWithOther : BasePanel
{

    Talking newState;

    List<ListData> ObjectWord = new List<ListData>();
    List<string> TalkData = new List<string>(); 
    public TextMeshProUGUI Talk;
    public static TaikWithOther instance;
    public float WorlSpeed = 0.1f; // 默认的逐字显示速度

    bool WorkLodng = false; // 用于跟踪是否正在逐字显示

    int WordIndex = -1;


    new void Awake()
    {

        StartCoroutine(ReadJsonFile());

        if (instance != null) {
            Destroy(gameObject);

        }
        else {
            instance = this;
        
        }
        newState = Talking.NotTalking;
    }

    void Start()
    {
        transform.Find("Button").GetComponent<Button>().onClick.AddListener(()=> {
            if (newState == Talking.Talking)
            {
                if (WorkLodng)
                {
                    StopAllCoroutines();

                    WorkLodng = false;

                    Talk.text = TalkData[WordIndex];

                }
                else
                {
                    WordIndex++;

                    StartCoroutine(WordByWord(WordIndex));

                }
            }; });
    }



    public void Click() {

    }

    
    

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartTalk(jjjj);
        //}
    }

    public void StartTalk(string Name) {

        WordIndex = -1;

        Debug.LogWarning(Name);

        TalkData = GetDataByName(Name);

        if (TalkData.Count > 0)
        {

            gameObject.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(true);
            WordIndex++;
            StartCoroutine(WordByWord(WordIndex)); // 从第一个单词开始逐字显示
        }
    }

    private IEnumerator ReadJsonFile()
    {
        string uri = Path.Combine(Application.streamingAssetsPath, "Words.json");

        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            StringList dataList = JsonUtility.FromJson<StringList>(json);

            foreach (var table in dataList.items)
            {

                ObjectWord.Add(table);
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON: " + request.error);
        }

        gameObject.SetActive(false);
    }

    IEnumerator WordByWord(int order)
    {

        newState = Talking.Talking;

        // 检查 order 是否在有效范围内
        if (order >= TalkData.Count)
        {
            newState = Talking.NotTalking;

            transform.GetChild(0).gameObject.SetActive(false);
            this.gameObject.SetActive(false);
            yield break; // 退出协程

        }

        WorkLodng = true; // 开始逐字显示

        string old = TalkData[order]; // 获取要显示的单词

        if (old == null)
        {
            newState = Talking.NotTalking;
            gameObject.SetActive(false);
            yield break; // 退出协程
        }

        Talk.text = ""; // 清空文本框

        for (int i = 0; i < old.Length; i++)
        {
            Talk.text += old[i]; // 逐字添加字符
            yield return new WaitForSeconds(WorlSpeed); // 等待指定的时间
        }

        WorkLodng = false; // 结束逐字显示
    }

    public override void ShowMe()
    {
        
    }

    public override void HideMe()
    {
        
    }
    public List<string> GetDataByName(string name)
    {
        foreach (var listData in ObjectWord)
        {
            if (listData.name == name)
            {
                return listData.Data;
            }
        }

        return null;
    }

}