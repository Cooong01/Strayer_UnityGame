using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用来计算投影的工具类
public static class Projection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="point">需要投影的点坐标</param>
    /// <param name="lineOrigin">投影线的起点</param>
    /// <param name="lineDirection">投影线的终点</param>
    /// <returns></returns>
    public static Vector2 ProjectPointOntoLine(Vector2 point, Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 lineDirection = endPoint - startPoint;
        float lineLengthSquared = lineDirection.sqrMagnitude; // 线段的平方长度

        if (lineLengthSquared == 0)
        {
            return startPoint;
        }

        float distanceToStart = Vector2.Distance(point, startPoint);
        float distanceToEnd = Vector2.Distance(point, endPoint);
        float lineLength = Mathf.Sqrt(lineLengthSquared); // 线段的长度

        //检查距离是否大于两锚点的距离
        if (distanceToStart >= lineLength || distanceToEnd >= lineLength)
        {
            //返回离点击点最近的锚点
            return distanceToStart < distanceToEnd ? startPoint : endPoint;
        }

        Vector2 pointDirection = point - startPoint;
        float projectionLength = Vector2.Dot(pointDirection, lineDirection) / lineLengthSquared;
        projectionLength = Mathf.Clamp01(projectionLength);
        return startPoint + lineDirection * projectionLength;
    }



    /// <summary>
    /// 计算点到线的距离
    /// </summary>
    /// <param name="p">点</param>
    /// <param name="p1">线段端点1</param>
    /// <param name="p2">线段端点2</param>
    /// <returns></returns>
    public static float DistanceFromPoint2Line(Vector3 p, Vector3 p1, Vector3 p2)
    {
        // 计算方向向量
        Vector3 lineDirection = p2 - p1;
        // 计算从p1到p的向量
        Vector3 pointToP1 = p - p1;

        // 投影长度 t
        float t = Vector3.Dot(pointToP1, lineDirection) / lineDirection.sqrMagnitude;

        if (t < 0.0f) // 如果t<0，那么垂足在p1之前，距离为p到p1的距离
        {
            return Vector3.Distance(p, p1);
        }
        else if (t > 1.0f) // 如果t>1，那么垂足在p2之后，距离为p到p2的距离
        {
            return Vector3.Distance(p, p2);
        }

        // 如果0 <= t <= 1，那么垂足在线段p1p2上
        // 计算垂足位置
        Vector3 projection = p1 + t * lineDirection;
        // 返回p到垂足的距离
        return Vector3.Distance(p, projection);
    }
}
