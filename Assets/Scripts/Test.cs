using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        ABResMgr.Instance.LoadResAsync<GameObject>("guanqia", "Level 0", (t) => {
            print(t.name);
            Instantiate(t);
        });
    }

    void Update()
    {
        
    }
}
