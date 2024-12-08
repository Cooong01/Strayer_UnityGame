using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������ͶӰ�Ĺ�����
public static class Projection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="point">��ҪͶӰ�ĵ�����</param>
    /// <param name="lineOrigin">ͶӰ�ߵ����</param>
    /// <param name="lineDirection">ͶӰ�ߵ��յ�</param>
    /// <returns></returns>
    public static Vector2 ProjectPointOntoLine(Vector2 point, Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 lineDirection = endPoint - startPoint;
        float lineLengthSquared = lineDirection.sqrMagnitude; // �߶ε�ƽ������

        if (lineLengthSquared == 0)
        {
            return startPoint;
        }

        float distanceToStart = Vector2.Distance(point, startPoint);
        float distanceToEnd = Vector2.Distance(point, endPoint);
        float lineLength = Mathf.Sqrt(lineLengthSquared); // �߶εĳ���

        //�������Ƿ������ê��ľ���
        if (distanceToStart >= lineLength || distanceToEnd >= lineLength)
        {
            //���������������ê��
            return distanceToStart < distanceToEnd ? startPoint : endPoint;
        }

        Vector2 pointDirection = point - startPoint;
        float projectionLength = Vector2.Dot(pointDirection, lineDirection) / lineLengthSquared;
        projectionLength = Mathf.Clamp01(projectionLength);
        return startPoint + lineDirection * projectionLength;
    }



    /// <summary>
    /// ����㵽�ߵľ���
    /// </summary>
    /// <param name="p">��</param>
    /// <param name="p1">�߶ζ˵�1</param>
    /// <param name="p2">�߶ζ˵�2</param>
    /// <returns></returns>
    public static float DistanceFromPoint2Line(Vector3 p, Vector3 p1, Vector3 p2)
    {
        // ���㷽������
        Vector3 lineDirection = p2 - p1;
        // �����p1��p������
        Vector3 pointToP1 = p - p1;

        // ͶӰ���� t
        float t = Vector3.Dot(pointToP1, lineDirection) / lineDirection.sqrMagnitude;

        if (t < 0.0f) // ���t<0����ô������p1֮ǰ������Ϊp��p1�ľ���
        {
            return Vector3.Distance(p, p1);
        }
        else if (t > 1.0f) // ���t>1����ô������p2֮�󣬾���Ϊp��p2�ľ���
        {
            return Vector3.Distance(p, p2);
        }

        // ���0 <= t <= 1����ô�������߶�p1p2��
        // ���㴹��λ��
        Vector3 projection = p1 + t * lineDirection;
        // ����p������ľ���
        return Vector3.Distance(p, projection);
    }
}
