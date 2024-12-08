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

        // ����Э�̣����ڼ�麯���Ƿ񱻵���
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
        // �����µļ���·��
        CasetLaser(h);

        // ֻ��Ⱦ��ǰ����
        laser.positionCount = laserPoint.Count;

        // �������õ� LineRenderer
        laser.SetPositions(laserPoint.ToArray());

        Vector2[] colliderPoints = laserPoint.ConvertAll(point => (Vector2)laser.transform.InverseTransformPoint(point)).ToArray();

        edgeCollider.points = colliderPoints;
    }

    public LayerMask EndMask;

    void CasetLaser(Vector3 direction)
    {
        // ��վɵ�LaserPoint
        laserPoint.Clear();

        // �� Laser Gun ��λ�ó���
        Vector3 startPoint = laser.transform.position;

        // ��ӵ�һ��������
        laserPoint.Add(startPoint);

        // ��ȡ LaserGun �������ײ��Ͳ�
        Collider2D laserGunCollider = GetComponent<Collider2D>();
        int laserLayer = gameObject.layer;  // ��ȡ LaserGun ���ڵĲ�

        // ʹ�� LayerMask �����Լ��Ĳ�
        LayerMask allLayers = Physics2D.AllLayers;  // �������в�

        // �ų��Լ��Ĳ�
        LayerMask layerMaskWithoutSelf = allLayers & ~(1 << laserLayer);

        int maxReflections = 6; // ��������
        int i = 0;

        while (i < maxReflections)
        {

            // �������·���ϵ�������ײ���������Լ��Ĳ�
            RaycastHit2D hitInfo = Physics2D.Raycast(startPoint, direction, Mathf.Infinity, layerMaskWithoutSelf);

            if (hitInfo.collider == edgeCollider || hitInfo.collider.transform.name == "BG") {

                layerMaskWithoutSelf &= ~(1 << hitInfo.collider.gameObject.layer);
                startPoint = hitInfo.point + (Vector2)direction.normalized * 0.01f; // �����������΢ƫ�ƣ������ظ����
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
        isCalled = true;  // ���Ϊ�ѵ���

        laser.gameObject.SetActive(true);

        laser.transform.position = hitPosition + direction .normalized * 1.2f;

        this.direction = direction.normalized;
    }

    private IEnumerator CheckIfCalled()
    {
        while (true)
        {
            // �ȴ�ָ����ʱ��
            yield return null;

            // ��麯���ڴ�ʱ������Ƿ񱻵���
            if (!isCalled)
            {
                HideLaser();  // ���δ���ã����ؼ���
            }

            // ���ñ�־����
            isCalled = false;
        }
    }

    public void HideLaser()
    {
        laser.gameObject.SetActive(false);
    }


}
