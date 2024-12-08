using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorLight : Laser
{

    MirrorLight OtherMirror;
    BoxCollider2D boxCollider;

    private bool isCalled;  // 标志变量，记录函数是否被调用


    private void OnEnable()
    {
        base.laser = transform.Find("Line").GetComponent<LineRenderer>();
        laser.gameObject.SetActive(false);
        isCalled = false;
        boxCollider = GetComponent<BoxCollider2D>();

        // 启动协程，定期检查函数是否被调用
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
            this.isCalled = true;  // 标记为已调用

            this.laser.gameObject.SetActive(true);

            this.laser.transform.position = transform.TransformPoint(hitPosition);

            this.direction = direction.normalized;
        }
        else {
            if (OtherMirror.boxCollider.enabled)
            {

                OtherMirror.isCalled = true;  // 标记为已调用

                OtherMirror.laser.gameObject.SetActive(true);

                OtherMirror.laser.transform.position = OtherMirror.transform.TransformPoint(hitPosition);

                OtherMirror.direction = direction.normalized;
            }
            else {
                OtherMirror.isCalled = false;  // 标记为已调用

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

    // 隐藏激光的方法
    public void HideLaser()
    {
        laser.gameObject.SetActive(false);
    }


    // 协程，定期检查函数是否被调用
    private IEnumerator CheckIfCalled()
    {
        while (true)
        {
            // 等待指定的时间
            yield return null;

            // 检查函数在此时间段内是否被调用
            if (!isCalled)
            {
                HideLaser();  // 如果未调用，隐藏激光
            }

            // 重置标志变量
            isCalled = false;
        }
    }



}
