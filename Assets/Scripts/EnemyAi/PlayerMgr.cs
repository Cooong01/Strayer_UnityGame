using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlayerMgr : MonoBehaviour
{
    public int degree = 45; // 45°
    public float radius = 5f; // 扇形半径
    private int lastDegree; // 记录上一次的degree
    private float lastRadius; // 记录上一次的radius
    private Mesh mesh; // 缓存Mesh对象
    public Material material;
    MeshRenderer meshRenderer;
    void Start()
    {
        // 初始化缓存
        lastDegree = degree;
        lastRadius = radius;
        mesh = new Mesh();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        DrawMesh(); // 初始化扇形网格
    }

    /// <summary>
    /// 绘制扇形检测范围
    /// </summary>
    private void DrawMesh()
    {

        List<Vector3> points = new List<Vector3>();
        List<int> index = new List<int>();

        // 将中心点添加到顶点列表
        points.Add(Vector3.zero);

        // 在2D平面上计算扇形的顶点
        for (int i = -degree; i <= degree; i += 2)
        {
            // 计算在x-y平面的点，忽略z轴
            Vector3 point = Quaternion.Euler(0, 0, i) * Vector3.right * radius; // 2D旋转仅发生在z轴
            points.Add(point);
        }

        // 生成三角形索引
        for (int i = 0; i < points.Count - 1; i++)
        {
            index.Add(0);
            index.Add(i+1);
            index.Add(i);
        }

        // 应用顶点和三角形到网格
        mesh.Clear(); // 清除之前的网格数据
        mesh.vertices = points.ToArray();
        mesh.triangles = index.ToArray();
        mesh.RecalculateNormals(); // 如果需要光照效果，通常在2D项目中不需要

        meshRenderer.material = material;
    }

    // Update is called once per frame
    void Update()
    {
        // 仅在degree或radius发生变化时重新生成网格
        if (degree != lastDegree || Mathf.Abs(radius - lastRadius) > Mathf.Epsilon)
        {
            DrawMesh(); // 更新扇形网格
            lastDegree = degree;
            lastRadius = radius;
        }
    }
}
