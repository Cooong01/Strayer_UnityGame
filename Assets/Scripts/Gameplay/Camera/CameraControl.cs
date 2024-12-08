using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //获得当前玩家正处在的关卡内
        //todo：在物理检测里做一个检测关卡BG的功能，返回检测到的关卡BG，相机每帧获取这个BG，作为自己的摄像边缘。
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
