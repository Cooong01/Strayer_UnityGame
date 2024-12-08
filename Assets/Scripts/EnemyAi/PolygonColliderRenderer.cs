using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(PolygonCollider2D))]
public class PolygonColliderRenderer : MonoBehaviour
{
    LineRenderer lineRenderer;
    PolygonCollider2D polygonCollider;
    MeshFilter meshFilter; // 添加 MeshFilter 组件

    public int angle; // 角度
    public float radius; // 半径
    private int lastAngle; // 记录上一次的角度
    private float lastRadius; // 记录上一次的半径

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        meshFilter = gameObject.AddComponent<MeshFilter>(); // 添加 MeshFilter
        DrawSector(); // 初始化扇形
    }

    void Update()
    {
        // 检查角度和半径是否发生变化
        if (angle != lastAngle || Mathf.Abs(radius - lastRadius) > Mathf.Epsilon)
        {
            DrawSector(); // 重新绘制扇形
            lastAngle = angle; // 更新上一次的角度
            lastRadius = radius; // 更新上一次的半径
        }

        DrawColliderOutline(); // 每帧更新渲染
    }

    // 绘制扇形
    public void DrawSector()
    {
        // 创建点数组用于碰撞体
        List<Vector2> colliderPoints = new List<Vector2>();
        colliderPoints.Add(Vector2.zero); // 中心点作为第一个顶点

        // 绘制扇形的边缘并生成碰撞体的顶点
        for (int i = -angle / 2; i <= angle / 2; i++)
        {
            // 计算每个点的位置，使用物体的旋转
            Vector3 point = Quaternion.Euler(0, 0, i) * Vector3.right * radius;
            // 将 3D 点转换为 2D 点
            colliderPoints.Add(point);
        }

        // 更新 PolygonCollider2D 的路径（2D 顶点）
        polygonCollider.pathCount = 1; // 设置为单个路径
        polygonCollider.SetPath(0, colliderPoints.ToArray());
    }

    // 使用LineRenderer渲染PolygonCollider2D的路径
    void DrawColliderOutline()
    {
        lineRenderer.positionCount = polygonCollider.points.Length;

        Vector2[] points = polygonCollider.points; // 获取碰撞体顶点

        // 将每个顶点转换为世界坐标并传递给LineRenderer
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 worldPoint = transform.TransformPoint(points[i]);
            lineRenderer.SetPosition(i, worldPoint);
        }
    }

    // 创建网格（如果需要）
    void CreateMesh()
    {
        int pointCount = polygonCollider.points.Length;
        Vector3[] vertices = new Vector3[pointCount + 1]; // +1 for center point
        int[] triangles = new int[(pointCount - 1) * 3]; // 每两个点形成一个三角形

        // 获取 PolygonCollider2D 的点
        Vector2[] colliderPoints = polygonCollider.points;

        // 设置顶点
        vertices[0] = transform.position; // 中心点
        for (int i = 0; i < pointCount; i++)
        {
            vertices[i + 1] = transform.TransformPoint(colliderPoints[i]); // 将 2D 点转换为 3D 点
        }

        // 创建三角形
        for (int i = 0; i < pointCount - 1; i++)
        {
            triangles[i * 3] = 0; // 中心点
            triangles[i * 3 + 1] = i + 1; // 当前点
            triangles[i * 3 + 2] = (i + 2) % pointCount + 1; // 下一个点
        }

        // 创建网格
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // 计算法线
        mesh.RecalculateNormals();

        // 应用网格
        meshFilter.mesh = mesh;

        // 添加 MeshRenderer（如果需要）
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default")); // 使用默认的精灵着色器
    }
}
