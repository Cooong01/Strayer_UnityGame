using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIoutline : MaskableGraphic
{
    public Color lineColor = Color.black; // 默认描边颜色
    public float lineWidth = 1f; // 默认线宽

    [Header("边的生成控制")]
    public bool drawTop = true;    // 是否绘制顶部边
    public bool drawBottom = true; // 是否绘制底部边
    public bool drawLeft = true;   // 是否绘制左侧边
    public bool drawRight = true;  // 是否绘制右侧边

    [Header("透明度渐变控制")]
    [Range(0, 1)] public float topStartAlpha = 1f;    // 顶部边开始的透明度
    [Range(0, 1)] public float topEndAlpha = 0.5f;    // 顶部边结束的透明度
    [Range(0, 1)] public float bottomStartAlpha = 1f; // 底部边开始的透明度
    [Range(0, 1)] public float bottomEndAlpha = 0.5f; // 底部边结束的透明度
    [Range(0, 1)] public float leftStartAlpha = 1f;   // 左侧边开始的透明度
    [Range(0, 1)] public float leftEndAlpha = 0.5f;   // 左侧边结束的透明度
    [Range(0, 1)] public float rightStartAlpha = 1f;  // 右侧边开始的透明度
    [Range(0, 1)] public float rightEndAlpha = 0.5f;  // 右侧边结束的透明度


    protected override void Start()
    {
        base.Start();
        this.raycastTarget = false;
       
    }

    public void Blink() {


    }


    private void UpdateOutlineAlpha(float alpha)
    {
        // 更新每条边的透明度值
        topStartAlpha = alpha;
        topEndAlpha = alpha;
        bottomStartAlpha = alpha;
        bottomEndAlpha = alpha;
        leftStartAlpha = alpha;
        leftEndAlpha = alpha;
        rightStartAlpha = alpha;
        rightEndAlpha = alpha;

        SetVerticesDirty(); // 通知 UI 组件需要重新绘制
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        vh.Clear();

        // 绘制边缘
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
