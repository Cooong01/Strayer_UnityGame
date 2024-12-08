using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;  //角色的 Transform
    public float smoothSpeed = 0.125f;  //平滑追踪速度
    public Vector3 offset;  //相机相对于角色的位置偏移

    void LateUpdate()
    {
        // 计算目标位置
        Vector3 desiredPosition = target.position + offset;
        // 平滑移动相机
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(desiredPosition.x,desiredPosition.y,transform.position.z), smoothSpeed);
        transform.position = smoothedPosition;
    }
}
