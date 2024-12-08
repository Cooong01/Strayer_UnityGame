using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Laser 本体
    [HideInInspector]
    public LineRenderer laser;
    // 存储 Laser 经过的路径的列表
    [HideInInspector]
    public List<Vector3> laserPoint = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> RiftPoint = new List<Vector3>();

    public void GetLine(Vector3 h)
    {
        // 计算新的激光路径
        CasetLaser(h);

        // 只渲染当前射线
        laser.positionCount = laserPoint.Count;

        // 将点设置到 LineRenderer
        laser.SetPositions(laserPoint.ToArray());
    }

    public LayerMask ReflectLayerMask;  // 反射的Layer
    public LayerMask ChangToEnLayerMask; // 实体化Layer
    public LayerMask ChangeLayerMask; // 转换成彩虹
    public LayerMask MissorMask;  // 仅连接不反射和不穿透的Layer
    public LayerMask PenetrateMask;  // 穿透的Layer
    public LayerMask PlayerMask; // 玩家层
    public LayerMask EnemyMask; // 敌人层

    void CasetLaser(Vector3 direction)
    {
        // 清空旧的LaserPoint
        laserPoint.Clear();

        // 清空旧的RiftPoint
        RiftPoint.Clear();

        // 从 Laser Gun 的位置出发
        Vector3 startPoint = laser.transform.position;

        // 添加第一个出发点
        laserPoint.Add(startPoint);

        int maxReflections = 6; // 最大反射次数
        int i = 0;

        // 获取 LaserGun 自身的碰撞体和层
        Collider2D laserGunCollider = GetComponent<Collider2D>();
        int laserLayer = gameObject.layer;  // 获取 LaserGun 所在的层

        // 使用 LayerMask 忽略自己的层
        LayerMask allLayers = Physics2D.AllLayers;  // 包含所有层

        // 排除自己的层
        LayerMask layerMaskWithoutSelf = allLayers & ~(1 << laserLayer);

        while (i < maxReflections)
        {
            // 检测射线路径上的所有碰撞，并忽略自己的层
            RaycastHit2D hitInfo = Physics2D.Raycast(startPoint, direction, Mathf.Infinity, layerMaskWithoutSelf);

            // 如果射线未击中任何物体
            if (hitInfo.collider == null)
            {
                Vector3 extensionPoint;

                if (RiftPoint.Count < 1)
                {
                    // 沿着当前方向延伸指定距离
                    extensionPoint = (startPoint + direction);
                }
                else
                {
                    extensionPoint = (RiftPoint[RiftPoint.Count - 1] + direction);
                }

                RiftPoint.Add(extensionPoint);
                laserPoint.Add(extensionPoint);
                i++;
                continue;
            }

            // 如果击中的是 LaserGun 自己，忽略
            if (hitInfo.collider == laserGunCollider)
            {
                break;
            }

            // 如果击中了镜子层
            if ((MissorMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {

                MirrorLight mirrorLight = hitInfo.collider.GetComponent<MirrorLight>();

                if (mirrorLight != null)
                {
                    laserPoint.Add(hitInfo.point);
                    RemoveRiftPointsFromLaserPoint();
                    mirrorLight.ChangeToRainBow(hitInfo.collider.transform.InverseTransformPoint(hitInfo.point),
                        Vector2.Reflect(hitInfo.point - (Vector2)startPoint, hitInfo.normal));
                    break;
                }
            }
            // 如果击中了实体化层
            else if ((ChangToEnLayerMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                ChangeToEntity changeToEntity = hitInfo.collider.GetComponent<ChangeToEntity>();
                if (changeToEntity != null)
                {
                    RemoveRiftPointsFromLaserPoint();
                    laserPoint.Add(hitInfo.point);

                    // 实体化
                    changeToEntity.ChangeTo(hitInfo.point, direction);

                    break; // 添加这一行以停止后续的else if判断
                }
            }
            // 如果击中了彩虹层
            else if ((ChangeLayerMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                ChangeLine rainBowLine = hitInfo.collider.GetComponent<ChangeLine>();
                if (rainBowLine != null)
                {
                    laserPoint.Add(hitInfo.point);
                    RemoveRiftPointsFromLaserPoint();
                    rainBowLine.ChangeToRainBow(hitInfo.point, Vector2.Reflect(hitInfo.point - (Vector2)startPoint, hitInfo.normal));
                    i++;
                    break;
                }
            }
            // 如果击中了穿透层
            else if ((PenetrateMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                // 忽略该层并继续射线检测
                layerMaskWithoutSelf &= ~(1 << hitInfo.collider.gameObject.layer);
                startPoint = hitInfo.point + (Vector2)direction.normalized * 0.01f; // 让射线起点稍微偏移，避免重复检测
                continue;



                //RemoveRiftPointsFromLaserPoint();

                //float extensionDistance = 0.5f;
                //Vector2 newEndPoint = hitInfo.point + (Vector2)direction.normalized * extensionDistance;

                //startPoint = newEndPoint;

                //RiftPoint.Add(newEndPoint);
                //laserPoint.Add(newEndPoint);

                ////原设定穿透层
                ///laserPoint.Add(startPoint); 
                //RemoveRiftPointsFromLaserPoint();
                //startPoint = laserPoint[laserPoint.Count - 1] + direction;
                //RiftPoint.Add(startPoint);
                //laserPoint.AddRange(RiftPoint);
            }
            // 如果击中了反射层
            else if ((ReflectLayerMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                laserPoint.Add(hitInfo.point);

                // 反射处理
                direction = Vector2.Reflect(hitInfo.point - (Vector2)startPoint, hitInfo.normal);
                startPoint = (Vector3)hitInfo.point + direction * 0.01f; // 让射线起点稍微偏移，避免重复检测
                i++;
            }
            // 如果击中了玩家层
            else if ((PlayerMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                // 玩家被彩虹激光击中时
                if (laser.material.name.Contains("RainBow"))
                {
                    laserPoint.Add(hitInfo.collider.transform.position);
                    RemoveRiftPointsFromLaserPoint();

                    // 触发玩家受攻击的事件
                    EventCenter.Instance.EventTrigger(E_EventType.E_PlayerAttacked);
                }

                if (laser.material.name.Contains("white"))
                {
                    // 忽略该层并继续射线检测
                    startPoint = hitInfo.point + (Vector2)direction.normalized * 0.01f; // 让射线起点稍微偏移，避免重复检测
                    continue;

                }

                break;
            }
            // 如果击中了敌人层
            else if ((EnemyMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                // 玩家被彩虹激光击中时
                if (laser.material.name.Contains("RainBow"))
                {
                    laserPoint.Add(hitInfo.point);
                    RemoveRiftPointsFromLaserPoint();

                    // 触发敌人受攻击的事件
                    EnemyAI enemyAI = hitInfo.collider.GetComponent<EnemyAI>();

                    enemyAI.Attacked();
                }
                if (laser.material.name.Contains("white")) {
                    // 忽略该层并继续射线检测
                    startPoint = hitInfo.point + (Vector2)direction.normalized * 0.01f; // 让射线起点稍微偏移，避免重复检测
                    continue;

                }

                break;
            }
            // 如果击中了未处理的层
            else
            {
                RemoveRiftPointsFromLaserPoint();
                laserPoint.Add(hitInfo.point);
                break;
            }
        }
    }

    // 删除 laserPoint 中的 RiftPoint 元素
    void RemoveRiftPointsFromLaserPoint()
    {
        foreach (Vector3 riftPoint in RiftPoint)
        {
            laserPoint.RemoveAll(p => p == riftPoint); // 确保移除所有匹配的点
        }

        // 清空 RiftPoint 列表
        RiftPoint.Clear();
    }
}
