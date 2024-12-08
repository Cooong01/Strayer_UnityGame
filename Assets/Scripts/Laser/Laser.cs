using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Laser ����
    [HideInInspector]
    public LineRenderer laser;
    // �洢 Laser ������·�����б�
    [HideInInspector]
    public List<Vector3> laserPoint = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> RiftPoint = new List<Vector3>();

    public void GetLine(Vector3 h)
    {
        // �����µļ���·��
        CasetLaser(h);

        // ֻ��Ⱦ��ǰ����
        laser.positionCount = laserPoint.Count;

        // �������õ� LineRenderer
        laser.SetPositions(laserPoint.ToArray());
    }

    public LayerMask ReflectLayerMask;  // �����Layer
    public LayerMask ChangToEnLayerMask; // ʵ�廯Layer
    public LayerMask ChangeLayerMask; // ת���ɲʺ�
    public LayerMask MissorMask;  // �����Ӳ�����Ͳ���͸��Layer
    public LayerMask PenetrateMask;  // ��͸��Layer
    public LayerMask PlayerMask; // ��Ҳ�
    public LayerMask EnemyMask; // ���˲�

    void CasetLaser(Vector3 direction)
    {
        // ��վɵ�LaserPoint
        laserPoint.Clear();

        // ��վɵ�RiftPoint
        RiftPoint.Clear();

        // �� Laser Gun ��λ�ó���
        Vector3 startPoint = laser.transform.position;

        // ��ӵ�һ��������
        laserPoint.Add(startPoint);

        int maxReflections = 6; // ��������
        int i = 0;

        // ��ȡ LaserGun �������ײ��Ͳ�
        Collider2D laserGunCollider = GetComponent<Collider2D>();
        int laserLayer = gameObject.layer;  // ��ȡ LaserGun ���ڵĲ�

        // ʹ�� LayerMask �����Լ��Ĳ�
        LayerMask allLayers = Physics2D.AllLayers;  // �������в�

        // �ų��Լ��Ĳ�
        LayerMask layerMaskWithoutSelf = allLayers & ~(1 << laserLayer);

        while (i < maxReflections)
        {
            // �������·���ϵ�������ײ���������Լ��Ĳ�
            RaycastHit2D hitInfo = Physics2D.Raycast(startPoint, direction, Mathf.Infinity, layerMaskWithoutSelf);

            // �������δ�����κ�����
            if (hitInfo.collider == null)
            {
                Vector3 extensionPoint;

                if (RiftPoint.Count < 1)
                {
                    // ���ŵ�ǰ��������ָ������
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

            // ������е��� LaserGun �Լ�������
            if (hitInfo.collider == laserGunCollider)
            {
                break;
            }

            // ��������˾��Ӳ�
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
            // ���������ʵ�廯��
            else if ((ChangToEnLayerMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                ChangeToEntity changeToEntity = hitInfo.collider.GetComponent<ChangeToEntity>();
                if (changeToEntity != null)
                {
                    RemoveRiftPointsFromLaserPoint();
                    laserPoint.Add(hitInfo.point);

                    // ʵ�廯
                    changeToEntity.ChangeTo(hitInfo.point, direction);

                    break; // �����һ����ֹͣ������else if�ж�
                }
            }
            // ��������˲ʺ��
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
            // ��������˴�͸��
            else if ((PenetrateMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                // ���Ըò㲢�������߼��
                layerMaskWithoutSelf &= ~(1 << hitInfo.collider.gameObject.layer);
                startPoint = hitInfo.point + (Vector2)direction.normalized * 0.01f; // �����������΢ƫ�ƣ������ظ����
                continue;



                //RemoveRiftPointsFromLaserPoint();

                //float extensionDistance = 0.5f;
                //Vector2 newEndPoint = hitInfo.point + (Vector2)direction.normalized * extensionDistance;

                //startPoint = newEndPoint;

                //RiftPoint.Add(newEndPoint);
                //laserPoint.Add(newEndPoint);

                ////ԭ�趨��͸��
                ///laserPoint.Add(startPoint); 
                //RemoveRiftPointsFromLaserPoint();
                //startPoint = laserPoint[laserPoint.Count - 1] + direction;
                //RiftPoint.Add(startPoint);
                //laserPoint.AddRange(RiftPoint);
            }
            // ��������˷����
            else if ((ReflectLayerMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                laserPoint.Add(hitInfo.point);

                // ���䴦��
                direction = Vector2.Reflect(hitInfo.point - (Vector2)startPoint, hitInfo.normal);
                startPoint = (Vector3)hitInfo.point + direction * 0.01f; // �����������΢ƫ�ƣ������ظ����
                i++;
            }
            // �����������Ҳ�
            else if ((PlayerMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                // ��ұ��ʺ缤�����ʱ
                if (laser.material.name.Contains("RainBow"))
                {
                    laserPoint.Add(hitInfo.collider.transform.position);
                    RemoveRiftPointsFromLaserPoint();

                    // ��������ܹ������¼�
                    EventCenter.Instance.EventTrigger(E_EventType.E_PlayerAttacked);
                }

                if (laser.material.name.Contains("white"))
                {
                    // ���Ըò㲢�������߼��
                    startPoint = hitInfo.point + (Vector2)direction.normalized * 0.01f; // �����������΢ƫ�ƣ������ظ����
                    continue;

                }

                break;
            }
            // ��������˵��˲�
            else if ((EnemyMask & (1 << hitInfo.collider.gameObject.layer)) != 0)
            {
                // ��ұ��ʺ缤�����ʱ
                if (laser.material.name.Contains("RainBow"))
                {
                    laserPoint.Add(hitInfo.point);
                    RemoveRiftPointsFromLaserPoint();

                    // ���������ܹ������¼�
                    EnemyAI enemyAI = hitInfo.collider.GetComponent<EnemyAI>();

                    enemyAI.Attacked();
                }
                if (laser.material.name.Contains("white")) {
                    // ���Ըò㲢�������߼��
                    startPoint = hitInfo.point + (Vector2)direction.normalized * 0.01f; // �����������΢ƫ�ƣ������ظ����
                    continue;

                }

                break;
            }
            // ���������δ����Ĳ�
            else
            {
                RemoveRiftPointsFromLaserPoint();
                laserPoint.Add(hitInfo.point);
                break;
            }
        }
    }

    // ɾ�� laserPoint �е� RiftPoint Ԫ��
    void RemoveRiftPointsFromLaserPoint()
    {
        foreach (Vector3 riftPoint in RiftPoint)
        {
            laserPoint.RemoveAll(p => p == riftPoint); // ȷ���Ƴ�����ƥ��ĵ�
        }

        // ��� RiftPoint �б�
        RiftPoint.Clear();
    }
}
