using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIoutline : MaskableGraphic
{
    public Color lineColor = Color.black; // Ĭ�������ɫ
    public float lineWidth = 1f; // Ĭ���߿�

    [Header("�ߵ����ɿ���")]
    public bool drawTop = true;    // �Ƿ���ƶ�����
    public bool drawBottom = true; // �Ƿ���Ƶײ���
    public bool drawLeft = true;   // �Ƿ��������
    public bool drawRight = true;  // �Ƿ�����Ҳ��

    [Header("͸���Ƚ������")]
    [Range(0, 1)] public float topStartAlpha = 1f;    // �����߿�ʼ��͸����
    [Range(0, 1)] public float topEndAlpha = 0.5f;    // �����߽�����͸����
    [Range(0, 1)] public float bottomStartAlpha = 1f; // �ײ��߿�ʼ��͸����
    [Range(0, 1)] public float bottomEndAlpha = 0.5f; // �ײ��߽�����͸����
    [Range(0, 1)] public float leftStartAlpha = 1f;   // ���߿�ʼ��͸����
    [Range(0, 1)] public float leftEndAlpha = 0.5f;   // ���߽�����͸����
    [Range(0, 1)] public float rightStartAlpha = 1f;  // �Ҳ�߿�ʼ��͸����
    [Range(0, 1)] public float rightEndAlpha = 0.5f;  // �Ҳ�߽�����͸����


    protected override void Start()
    {
        base.Start();
        this.raycastTarget = false;
       
    }

    public void Blink() {


    }


    private void UpdateOutlineAlpha(float alpha)
    {
        // ����ÿ���ߵ�͸����ֵ
        topStartAlpha = alpha;
        topEndAlpha = alpha;
        bottomStartAlpha = alpha;
        bottomEndAlpha = alpha;
        leftStartAlpha = alpha;
        leftEndAlpha = alpha;
        rightStartAlpha = alpha;
        rightEndAlpha = alpha;

        SetVerticesDirty(); // ֪ͨ UI �����Ҫ���»���
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        vh.Clear();

        // ���Ʊ�Ե
        if (drawTop)
        {
            DrawEdge(vh, drawTop, topStartAlpha, topEndAlpha, new Vector3(-rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f, 0), new Vector3(rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f, 0), new Vector3(rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f - lineWidth, 0), new Vector3(-rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f - lineWidth, 0));
        }

        if (drawBottom)
        {
            DrawEdge(vh, drawBottom, bottomStartAlpha, bottomEndAlpha, new Vector3(-rectTransform.rect.width * 0.5f, -rectTransform.rect.height * 0.5f + lineWidth, 0), new Vector3(rectTransform.rect.width * 0.5f, -rectTransform.rect.height * 0.5f + lineWidth, 0), new Vector3(rectTransform.rect.width * 0.5f, -rectTransform.rect.height * 0.5f, 0), new Vector3(-rectTransform.rect.width * 0.5f, -rectTransform.rect.height * 0.5f, 0));
        }

        if (drawLeft)
        {
            DrawEdge(vh, drawLeft, leftStartAlpha, leftEndAlpha, new Vector3(-rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f, 0), new Vector3(-rectTransform.rect.width * 0.5f + lineWidth, rectTransform.rect.height * 0.5f, 0), new Vector3(-rectTransform.rect.width * 0.5f + lineWidth, -rectTransform.rect.height * 0.5f, 0), new Vector3(-rectTransform.rect.width * 0.5f, -rectTransform.rect.height * 0.5f, 0));
        }

        if (drawRight)
        {
            DrawEdge(vh, drawRight, rightStartAlpha, rightEndAlpha, new Vector3(rectTransform.rect.width * 0.5f - lineWidth, rectTransform.rect.height * 0.5f, 0), new Vector3(rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f, 0), new Vector3(rectTransform.rect.width * 0.5f, -rectTransform.rect.height * 0.5f, 0), new Vector3(rectTransform.rect.width * 0.5f - lineWidth, -rectTransform.rect.height * 0.5f, 0));
        }
    }

    private void DrawEdge(VertexHelper vh, bool draw, float startAlpha, float endAlpha, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4)
    {
        if (draw)
        {
            UIVertex[] quad = new UIVertex[4];
            quad[0] = new UIVertex() { color = new Color(lineColor.r, lineColor.g, lineColor.b, startAlpha), position = pos1, uv0 = Vector2.zero };
            quad[1] = new UIVertex() { color = new Color(lineColor.r, lineColor.g, lineColor.b, endAlpha), position = pos2, uv0 = Vector2.zero };
            quad[2] = new UIVertex() { color = new Color(lineColor.r, lineColor.g, lineColor.b, endAlpha), position = pos3, uv0 = Vector2.zero };
            quad[3] = new UIVertex() { color = new Color(lineColor.r, lineColor.g, lineColor.b, startAlpha), position = pos4, uv0 = Vector2.zero };

            vh.AddUIVertexQuad(quad);
        }
    }
}
