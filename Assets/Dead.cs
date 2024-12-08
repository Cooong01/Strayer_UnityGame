using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RealDead() {
        GameObject circle = transform.parent.gameObject;
        if (circle != null) {
            circle.gameObject.SetActive(false);
        }
    
    }
}
