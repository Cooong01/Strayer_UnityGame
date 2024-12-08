using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteSpoit : Laser
{
    // Start is called before the first frame update
    void Start()
    {
        base.laser = transform.Find("Line").GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        base.GetLine(laser.transform.right * 10);
    }
}
