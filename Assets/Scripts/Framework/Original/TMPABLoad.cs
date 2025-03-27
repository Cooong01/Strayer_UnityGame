using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ����Ҫ��AB��������TMP��Դ����ʱ�����øù��ߡ�
/// ��Ҫ�ó���̬���ص���Դ�ϰ���TMP���壬��ص��ô˹��ߡ�
/// </summary>
public class TMPABLoad : BaseManager<TMPABLoad>
{
    private TMPABLoad() { }
    public void LoadTMP(GameObject obj)
    {
        TextMeshProUGUI[] TMP = obj.transform.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(var i in TMP)
        {
            i.font = Resources.Load<TMP_FontAsset>("Fonts/" + i.name); //�����ļ��涨����Resources��
        }
    }
}
