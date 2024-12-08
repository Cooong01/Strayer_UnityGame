using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckResult : MonoBehaviour
{
    public int EnemyNum;
    public int NowNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        EnemyNum = GameObject.FindObjectsOfType<EnemyAI>().Length;
        StartCoroutine(GetNum());
    }
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

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyAI enemy;

        if (collision.gameObject.TryGetComponent<EnemyAI>(out enemy))
        {
            enemy.StopMove();
            NowNum++;

            if (NowNum >0) {
                EventCenter.Instance.EventTrigger(E_EventType.E_canSuccess);

            }

           
        }
    }
}
