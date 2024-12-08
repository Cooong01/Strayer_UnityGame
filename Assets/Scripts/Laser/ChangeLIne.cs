using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLine : Laser
{
    private bool isCalled;  // 标志变量，记录函数是否被调用

    void Start()
    {
        base. laser = transform.Find("Line").GetComponent<LineRenderer>();
        laser.gameObject.SetActive(false);
        isCalled = false;

        // 启动协程，定期检查函数是否被调用
        StartCoroutine(CheckIfCalled());
    }

    Vector3 direction;

    public void ChangeToRainBow(Vector3 hitPosition, Vector3 direction)
    {
        isCalled = true;  // 标记为已调用

        laser.gameObject.SetActive(true);

        laser.transform.position = hitPosition;

        this.direction = direction.normalized;


        //// 判断竖向还是横向并旋转角度，调整一个较小的角度 (5度左右) 来使激光路径产生微小偏移,进行折射

        //float angleAdjustment = transform.rotation.eulerAngles.z / 5;

        //if (Mathf.Abs(direction.y) >  Mathf.Abs(direction.x))
        //{
        //    this.direction = RotateVector(this.direction, angleAdjustment);
        //}
        //else
        //{
        //    this.direction = RotateVector(this.direction, -angleAdjustment);
        //}
    }

    private void Update()
    {
        if (laser.gameObject.activeSelf) {

            base.GetLine(direction * 10);
        }
    }

    //折射
    //private Vector3 RotateVector(Vector3 originalDirection, float angleInDegrees)
    //{
    //    // 将角度转为弧度
    //    float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

    //    // 使用二维旋转矩阵进行旋转
    //    float cos = Mathf.Cos(angleInRadians);
    //    float sin = Mathf.Sin(angleInRadians);

    //    float newX = originalDirection.x * cos - originalDirection.y * sin;
    //    float newY = originalDirection.x * sin + originalDirection.y * cos;

    //    return new Vector3(newX, newY, originalDirection.z).normalized;
    //}



    // 隐藏激光的方法
    public void HideLaser()
    {
        laser.gameObject.SetActive(false);
    }


    // 协程，定期检查函数是否被调用
    private IEnumerator CheckIfCalled()
    {
        while (true)
        {
            // 等待指定的时间
            yield return null;

            // 检查函数在此时间段内是否被调用
            if (!isCalled)
            {
                HideLaser();  // 如果未调用，隐藏激光
            }

            // 重置标志变量
            isCalled = false;
        }
    }

}
