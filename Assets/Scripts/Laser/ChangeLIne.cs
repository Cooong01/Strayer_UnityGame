using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLine : Laser
{
    private bool isCalled;  // ��־��������¼�����Ƿ񱻵���

    void Start()
    {
        base. laser = transform.Find("Line").GetComponent<LineRenderer>();
        laser.gameObject.SetActive(false);
        isCalled = false;

        // ����Э�̣����ڼ�麯���Ƿ񱻵���
        StartCoroutine(CheckIfCalled());
    }

    Vector3 direction;

    public void ChangeToRainBow(Vector3 hitPosition, Vector3 direction)
    {
        isCalled = true;  // ���Ϊ�ѵ���

        laser.gameObject.SetActive(true);

        laser.transform.position = hitPosition;

        this.direction = direction.normalized;


        //// �ж������Ǻ�����ת�Ƕȣ�����һ����С�ĽǶ� (5������) ��ʹ����·������΢Сƫ��,��������

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

    //����
    //private Vector3 RotateVector(Vector3 originalDirection, float angleInDegrees)
    //{
    //    // ���Ƕ�תΪ����
    //    float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

    //    // ʹ�ö�ά��ת���������ת
    //    float cos = Mathf.Cos(angleInRadians);
    //    float sin = Mathf.Sin(angleInRadians);

    //    float newX = originalDirection.x * cos - originalDirection.y * sin;
    //    float newY = originalDirection.x * sin + originalDirection.y * cos;

    //    return new Vector3(newX, newY, originalDirection.z).normalized;
    //}



    // ���ؼ���ķ���
    public void HideLaser()
    {
        laser.gameObject.SetActive(false);
    }


    // Э�̣����ڼ�麯���Ƿ񱻵���
    private IEnumerator CheckIfCalled()
    {
        while (true)
        {
            // �ȴ�ָ����ʱ��
            yield return null;

            // ��麯���ڴ�ʱ������Ƿ񱻵���
            if (!isCalled)
            {
                HideLaser();  // ���δ���ã����ؼ���
            }

            // ���ñ�־����
            isCalled = false;
        }
    }

}
