using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// -----дAI�ĳ�������������δ֪-----
//����ע�ͣ�
public class CheckResult : MonoBehaviour
{
    public int EnemyNum;
    public int NowNum = 0;
    void Start()
    {
        EnemyNum = GameObject.FindObjectsOfType<EnemyAI>().Length;
        StartCoroutine(GetNum());
    }
    
    //����ע�ͣ���ȡ�ִ�AI����
    IEnumerator GetNum() {
        while (true) { 
            int x = GameObject.FindObjectsOfType<EnemyAI>().Length;
            yield return null;
            if (x>0) {
                continue;
                yield return null;
            }
            else {
                EventCenter.Instance.EventTrigger(E_EventType.E_RealFail);
                break;
            }
        }
    }

    //����ע�ͣ�����δ֪
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyAI enemy;
        if (collision.gameObject.TryGetComponent<EnemyAI>(out enemy))
        {
            enemy.StopMove();
            NowNum++;
            if (NowNum >0) {
                EventCenter.Instance.EventTrigger(E_EventType.E_isValid2SaveNPC);
            }
        }
    }
}
