using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorLight : Laser
{

    MirrorLight OtherMirror;
    BoxCollider2D boxCollider;

    private bool isCalled;  // ��־��������¼�����Ƿ񱻵���


    private void OnEnable()
    {
        base.laser = transform.Find("Line").GetComponent<LineRenderer>();
        laser.gameObject.SetActive(false);
        isCalled = false;
        boxCollider = GetComponent<BoxCollider2D>();

        // ����Э�̣����ڼ�麯���Ƿ񱻵���
        StartCoroutine(CheckIfCalled());
    }


    void Start()
    {
        if (transform.name == "Mirror2")
        {
            MirrorLight h = GameObject.Find("Mirror1").GetComponent<MirrorLight>();
            if (h != null)
            {
                OtherMirror = h;
            }

        }

        if (transform.name == "Mirror1")
        {
            MirrorLight h = GameObject.Find("Mirror2").GetComponent<MirrorLight>();
            if (h != null)
            {
                OtherMirror = h;
            }

        }

    }

    Vector3 direction;

    public void ChangeToRainBow(Vector3 hitPosition, Vector3 direction)
    {

        if (transform.name == "Mirror3")
        {
            this.isCalled = true;  // ���Ϊ�ѵ���

            this.laser.gameObject.SetActive(true);

            this.laser.transform.position = transform.TransformPoint(hitPosition);

            this.direction = direction.normalized;
        }
        else {
            if (OtherMirror.boxCollider.enabled)
            {

                OtherMirror.isCalled = true;  // ���Ϊ�ѵ���

                OtherMirror.laser.gameObject.SetActive(true);

                OtherMirror.laser.transform.position = OtherMirror.transform.TransformPoint(hitPosition);

                OtherMirror.direction = direction.normalized;
            }
            else {
                OtherMirror.isCalled = false;  // ���Ϊ�ѵ���

                OtherMirror.laser.gameObject.SetActive(false);

            }
        }


    }

    private void Update()
    {

        if (!boxCollider.enabled) {
            HideLaser();
        }

        if (laser.gameObject.activeSelf)
        {
            base.GetLine(direction * 10);
        }
    }

    // ���ؼ���ķ���
    public void HideLaser()
    {
        laser.gameObject.SetActive(false);
    }


    // Э�̣����ڼ�麯���Ƿ񱻵���
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



}
