using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    void Start()
    {
        //获得当前玩家正处在的关卡内
        if (GetComponent<CinemachineVirtualCamera>().LookAt.GetComponent<PlayerController>().parameter.check.BGObj != null)
        {

            GetComponent<CinemachineConfiner>().m_BoundingShape2D = GetComponent<CinemachineVirtualCamera>().LookAt.GetComponent<PlayerController>().parameter.check.BGObj.GetComponent<PolygonCollider2D>();
        }
    }

    void Update()
    {
        if (GetComponent<CinemachineVirtualCamera>().LookAt.GetComponent<PlayerController>().parameter.check.BGObj!=null)
        {

            GetComponent<CinemachineConfiner>().m_BoundingShape2D = GetComponent<CinemachineVirtualCamera>().LookAt.GetComponent<PlayerController>().parameter.check.BGObj.GetComponent<PolygonCollider2D>();
        }
    }
}
