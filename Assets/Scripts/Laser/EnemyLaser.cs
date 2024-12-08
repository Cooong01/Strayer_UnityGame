using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : Laser

{
    // Start is called before the first frame update
    void Start()
    {
        base.laser = transform.Find("Line").GetComponent<LineRenderer>();
        GetComponent<EnemyAI>().RainBow = base.laser;
        laser.gameObject.SetActive(false);
    }

    Vector3 direction;

    // Update is called once per frame
    private void Update()
    {
        if (laser.gameObject.activeSelf)
        {

            base.GetLine(direction * 10);
        }
    }

    public void ChangeToRainBow(Vector3 hitPosition, Vector3 direction)
    {

        laser.gameObject.SetActive(true);

        laser.transform.position = hitPosition;

        this. direction = direction.normalized;
    }
}
