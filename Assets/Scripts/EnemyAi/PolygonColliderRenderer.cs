using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(PolygonCollider2D))]
public class PolygonColliderRenderer : MonoBehaviour
{
    LineRenderer lineRenderer;
    PolygonCollider2D polygonCollider;
    MeshFilter meshFilter; // ��� MeshFilter ���

    public int angle; // �Ƕ�
    public float radius; // �뾶
    private int lastAngle; // ��¼��һ�εĽǶ�
    private float lastRadius; // ��¼��һ�εİ뾶

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        meshFilter = gameObject.AddComponent<MeshFilter>(); // ��� MeshFilter
        DrawSector(); // ��ʼ������
    }

    void Update()
    {
        // ���ǶȺͰ뾶�Ƿ����仯
        if (angle != lastAngle || Mathf.Abs(radius - lastRadius) > Mathf.Epsilon)
        {
            DrawSector(); // ���»�������
            lastAngle = angle; // ������һ�εĽǶ�
            lastRadius = radius; // ������һ�εİ뾶
        }

        DrawColliderOutline(); // ÿ֡������Ⱦ
    }

    // ��������
    public void DrawSector()
    {
        // ����������������ײ��
        List<Vector2> colliderPoints = new List<Vector2>();
        colliderPoints.Add(Vector2.zero); // ���ĵ���Ϊ��һ������

        // �������εı�Ե��������ײ��Ķ���
        for (int i = -angle / 2; i <= angle / 2; i++)
        {
            // ����ÿ�����λ�ã�ʹ���������ת
            Vector3 point = Quaternion.Euler(0, 0, i) * Vector3.right * radius;
            // �� 3D ��ת��Ϊ 2D ��
            colliderPoints.Add(point);
        }

        // ���� PolygonCollider2D ��·����2D ���㣩
        polygonCollider.pathCount = 1; // ����Ϊ����·��
        polygonCollider.SetPath(0, colliderPoints.ToArray());
    }

    // ʹ��LineRenderer��ȾPolygonCollider2D��·��
    void DrawColliderOutline()
    {
        lineRenderer.positionCount = polygonCollider.points.Length;

        Vector2[] points = polygonCollider.points; // ��ȡ��ײ�嶥��

        // ��ÿ������ת��Ϊ�������겢���ݸ�LineRenderer
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 worldPoint = transform.TransformPoint(points[i]);
            lineRenderer.SetPosition(i, worldPoint);
        }
    }

    // �������������Ҫ��
    void CreateMesh()
    {
        int pointCount = polygonCollider.points.Length;
        Vector3[] vertices = new Vector3[pointCount + 1]; // +1 for center point
        int[] triangles = new int[(pointCount - 1) * 3]; // ÿ�������γ�һ��������

        // ��ȡ PolygonCollider2D �ĵ�
        Vector2[] colliderPoints = polygonCollider.points;

        // ���ö���
        vertices[0] = transform.position; // ���ĵ�
        for (int i = 0; i < pointCount; i++)
        {
            vertices[i + 1] = transform.TransformPoint(colliderPoints[i]); // �� 2D ��ת��Ϊ 3D ��
        }

        // ����������
        for (int i = 0; i < pointCount - 1; i++)
        {
            triangles[i * 3] = 0; // ���ĵ�
            triangles[i * 3 + 1] = i + 1; // ��ǰ��
            triangles[i * 3 + 2] = (i + 2) % pointCount + 1; // ��һ����
        }

        // ��������
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // ���㷨��
        mesh.RecalculateNormals();

        // Ӧ������
        meshFilter.mesh = mesh;

        // ��� MeshRenderer�������Ҫ��
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default")); // ʹ��Ĭ�ϵľ�����ɫ��
    }
}
