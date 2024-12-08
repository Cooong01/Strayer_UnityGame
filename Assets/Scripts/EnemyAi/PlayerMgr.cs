using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlayerMgr : MonoBehaviour
{
    public int degree = 45; // 45��
    public float radius = 5f; // ���ΰ뾶
    private int lastDegree; // ��¼��һ�ε�degree
    private float lastRadius; // ��¼��һ�ε�radius
    private Mesh mesh; // ����Mesh����
    public Material material;
    MeshRenderer meshRenderer;
    void Start()
    {
        // ��ʼ������
        lastDegree = degree;
        lastRadius = radius;
        mesh = new Mesh();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        DrawMesh(); // ��ʼ����������
    }

    /// <summary>
    /// �������μ�ⷶΧ
    /// </summary>
    private void DrawMesh()
    {

        List<Vector3> points = new List<Vector3>();
        List<int> index = new List<int>();

        // �����ĵ���ӵ������б�
        points.Add(Vector3.zero);

        // ��2Dƽ���ϼ������εĶ���
        for (int i = -degree; i <= degree; i += 2)
        {
            // ������x-yƽ��ĵ㣬����z��
            Vector3 point = Quaternion.Euler(0, 0, i) * Vector3.right * radius; // 2D��ת��������z��
            points.Add(point);
        }

        // ��������������
        for (int i = 0; i < points.Count - 1; i++)
        {
            index.Add(0);
            index.Add(i+1);
            index.Add(i);
        }

        // Ӧ�ö���������ε�����
        mesh.Clear(); // ���֮ǰ����������
        mesh.vertices = points.ToArray();
        mesh.triangles = index.ToArray();
        mesh.RecalculateNormals(); // �����Ҫ����Ч����ͨ����2D��Ŀ�в���Ҫ

        meshRenderer.material = material;
    }

    // Update is called once per frame
    void Update()
    {
        // ����degree��radius�����仯ʱ������������
        if (degree != lastDegree || Mathf.Abs(radius - lastRadius) > Mathf.Epsilon)
        {
            DrawMesh(); // ������������
            lastDegree = degree;
            lastRadius = radius;
        }
    }
}
