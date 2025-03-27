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
        CircleCollider2D circle = transform.parent.gameObject.GetComponent<CircleCollider2D>();
        if (circle != null) {
            circle.enabled = false;
        }
    
    }
}
