using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 当需要用AB包来进行TMP资源加载时，调用该工具。
/// 当要用程序动态加载的资源上包含TMP字体，务必调用此工具。
/// </summary>
public class TMPABLoad : BaseManager<TMPABLoad>
{
    private TMPABLoad() { }
    public void LoadTMP(GameObject obj)
    {
        TextMeshProUGUI[] TMP = obj.transform.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(var i in TMP)
        {
            i.font = Resources.Load<TMP_FontAsset>("Fonts/" + i.name); //字体文件规定放在Resources里
        }
    }
}
