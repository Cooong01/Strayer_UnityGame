using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ����Ҫ��AB��������TMP��Դ����ʱ�����øù����ࡣ
/// ��Ҫ�ó������ɷ�������͵���Դ����Դ�ϰ���TMP���壬��ص��ô˹��ߡ�
/// </summary>
public class TMPABLoad : SingletonAutoMono<TMPABLoad>
{

    public void load(GameObject obj)
    {
        TextMeshProUGUI[] TMP = obj.transform.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(var i in TMP)
        {

            i.font = Resources.Load<TMP_FontAsset>("Fonts/" + i.name);
        }
    }
}
