using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;  //��ɫ�� Transform
    public float smoothSpeed = 0.125f;  //ƽ��׷���ٶ�
    public Vector3 offset;  //�������ڽ�ɫ��λ��ƫ��

    void LateUpdate()
    {
        // ����Ŀ��λ��
        Vector3 desiredPosition = target.position + offset;
        // ƽ���ƶ����
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(desiredPosition.x,desiredPosition.y,transform.position.z), smoothSpeed);
        transform.position = smoothedPosition;
    }
}
