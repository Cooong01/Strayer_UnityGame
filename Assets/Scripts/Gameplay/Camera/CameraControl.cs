using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //��õ�ǰ��������ڵĹؿ���
        //todo��������������һ�����ؿ�BG�Ĺ��ܣ����ؼ�⵽�Ĺؿ�BG�����ÿ֡��ȡ���BG����Ϊ�Լ��������Ե��
        if (GetComponent<CinemachineVirtualCamera>().LookAt.GetComponent<PlayerController>().parameter.check.BGObj != null)
        {

            GetComponent<CinemachineConfiner>().m_BoundingShape2D = GetComponent<CinemachineVirtualCamera>().LookAt.GetComponent<PlayerController>().parameter.check.BGObj.GetComponent<PolygonCollider2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<CinemachineVirtualCamera>().LookAt.GetComponent<PlayerController>().parameter.check.BGObj!=null)
        {

            GetComponent<CinemachineConfiner>().m_BoundingShape2D = GetComponent<CinemachineVirtualCamera>().LookAt.GetComponent<PlayerController>().parameter.check.BGObj.GetComponent<PolygonCollider2D>();
        }
    }
}
