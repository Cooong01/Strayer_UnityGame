using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeToEntity : MonoBehaviour
{
    bool isCalled;

    LineRenderer laser;

    EdgeCollider2D edgeCollider;

    List<Vector3> laserPoint = new List<Vector3>();


    // Start is called before the first frame update
    void Start()
    {
        Transform tran = transform.Find("Line");

        laser = tran.GetComponent<LineRenderer>();
        laser.gameObject.SetActive(false);
        isCalled = false;

        edgeCollider = tran.GetComponent<EdgeCollider2D>();

        // 启动协程，定期检查函数是否被调用
        StartCoroutine(CheckIfCalled());
    }

    Vector3 direction;

    // Update is called once per frame
    void Update()
    {
        if (laser.gameObject.activeSelf)
        {
            GetLine(direction * 10);
        }
    }

    void GetLine(Vector3 h)
    {
        // 计算新的激光路径
        CasetLaser(h);

        // 只渲染当前射线
        laser.positionCount = laserPoint.Count;

        // 将点设置到 LineRenderer
        laser.SetPositions(laserPoint.ToArray());

        Vector2[] colliderPoints = laserPoint.ConvertAll(point => (Vector2)laser.transform.InverseTransformPoint(point)).ToArray();

        edgeCollider.points = colliderPoints;
    }

    public LayerMask EndMask;

    void CasetLaser(Vector3 direction)
    {
        // 清空旧的LaserPoint
        laserPoint.Clear();

        // 从 Laser Gun 的位置出发
        Vector3 startPoint = laser.transform.position;

        // 添加第一个出发点
        laserPoint.Add(startPoint);

        // 获取 LaserGun 自身的碰撞体和层
        Collider2D laserGunCollider = GetComponent<Collider2D>();
        int laserLayer = gameObject.layer;  // 获取 LaserGun 所在的层

        // 使用 LayerMask 忽略自己的层
        LayerMask allLayers = Physics2D.AllLayers;  // 包含所有层

        // 排除自己的层
        LayerMask layerMaskWithoutSelf = allLayers & ~(1 << laserLayer);

        int maxReflections = 6; // 最大反射次数
        int i = 0;

        while (i < maxReflections)
        {

            // 检测射线路径上的所有碰撞，并忽略自己的层
            RaycastHit2D hitInfo = Physics2D.Raycast(startPoint, direction, Mathf.Infinity, layerMaskWithoutSelf);

            if (hitInfo.collider == edgeCollider || hitInfo.collider.transform.name == "BG") {

                layerMaskWithoutSelf &= ~(1 << hitInfo.collider.gameObject.layer);
                startPoint = hitInfo.point + (Vector2)direction.normalized * 0.01f; // 让射线起点稍微偏移，避免重复检测
                //laserPoint.Add(startPoint);
                continue;
            }
            else if ((EndMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                laserPoint.Add(hitInfo.point);

                break;
            }
            else {
                break;

            }


        }
    }


    public void ChangeTo(Vector3 hitPosition, Vector3 direction)
    {
        isCalled = true;  // 标记为已调用

        laser.gameObject.SetActive(true);

        laser.transform.position = hitPosition + direction .normalized * 1.2f;

        this.direction = direction.normalized;
    }

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

    public void HideLaser()
    {
        laser.gameObject.SetActive(false);
    }


}
