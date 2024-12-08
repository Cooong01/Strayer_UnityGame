using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChangeMirrorModeState : PlayerState
{

    public PlayerChangeMirrorModeState(PlayerFSM manager, string animationName) : base(manager, animationName){}

    public int step; //��һ�������վ�������ת���ڶ�����������ת����һ����С�����������µľ���һ�߷Ŵ�һ����ת���������������˳���״̬��ÿһ������ǰһ����ϣ�����д�������һ��
    public float nowTime;//��������������ʱ
    public bool step3;
    public override void OnEnter()
    {
        base.OnEnter();
        parameter.MirrorControlMode = !parameter.MirrorControlMode;
        step = 1;
        parameter.nowSetMirror.Clear(); //������þ����б��ڳ��ϵľ��ӻ��Զ��ص���ɫ���;
        parameter.nowOrientation = 0;
        EventCenter.Instance.EventTrigger(E_EventType.E_OutMirrorRecycle, 1);
        EventCenter.Instance.EventTrigger(E_EventType.E_OutMirrorRecycle, 2);
        EventCenter.Instance.EventTrigger(E_EventType.E_OutMirrorRecycle, 3);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        switch (step)
        {
            case 1:
                foreach (var mirror in parameter.mirrors)
                {
                    //���վ�������ת
                    if (!parameter.nowSetMirror.Contains(mirror.Key))
                    {
                        if (!MoveMirror(mirror.Key)) //һ��ִ��һ���ж�
                        {
                            //��ת
                            mirror.Value.transform.Rotate(0, 30, 0);
                        }
                        else
                        {
                            step = 2;
                            manager.StartCoroutine(DurationStep2());
                        }
                    }
                }
                break;
            case 2:
                foreach (var mirror in parameter.mirrors)
                {
                    //���վ�������ת
                    if (!parameter.nowSetMirror.Contains(mirror.Key))
                    {
                        if (!MoveMirror(mirror.Key)) //һ��ִ��һ���ж�
                        {
                            //��ת
                            mirror.Value.transform.Rotate(0, 3, 0);
                        }
                        else
                        {
                            step = 2;
                            manager.StartCoroutine(DurationStep2());
                        }
                    }

                    //һ����תһ����С
                    if (!parameter.nowSetMirror.Contains(mirror.Key))
                    {
                        //��ת
                        mirror.Value.transform.Rotate(0, 3, 0);
                        //��С
                        mirror.Value.transform.localScale = mirror.Value.transform.localScale * (1 - parameter.reduceSpeed * Time.deltaTime);
                    }
                }
                if (step3)
                {
                    changeMirrorActive();
                    manager.StartCoroutine(DurationStep3());
                    step = 3;
                    nowTime = 0;
                }
                break;
            case 3:

                foreach (var mirror in parameter.mirrors)
                {
                    //��ת
                    mirror.Value.transform.Rotate(0, 3, 0);
                    //�Ŵ�
                    nowTime += Time.deltaTime;
                    float scaleFactor = Mathf.Lerp(parameter.minScale, 1f, nowTime / parameter.reduceDuration);
                    switch (mirror.Value.name)
                    {
                        case "Mirror1":
                            mirror.Value.transform.localScale = parameter.mirror1OriginalScale * scaleFactor;
                            break;
                        case "Mirror2":
                            mirror.Value.transform.localScale = parameter.mirror2OriginalScale * scaleFactor;
                            break;
                        case "Mirror3":
                            mirror.Value.transform.localScale = parameter.mirror3OriginalScale * scaleFactor;
                            break;
                    }

                }
                break;
            case 4:
                //ʱ������Ժ��þ��ӱ�Ϊ������̬
                foreach (var mirror in parameter.mirrors)
                {
                    //��ת
                    mirror.Value.transform.rotation = new Quaternion(0, 0, 0,0);
                    switch (mirror.Value.name)
                    {
                        case "Mirror1":
                            mirror.Value.transform.localScale = parameter.mirror1OriginalScale;
                            break;
                        case "Mirror2":
                            mirror.Value.transform.localScale = parameter.mirror2OriginalScale;
                            break;
                        case "Mirror3":
                            mirror.Value.transform.localScale = parameter.mirror3OriginalScale;
                            break;
                    }
                }
                manager.TransitionState(E_PlayerStateType.Idle);
                break;
        }
    }

    public override void OnFixedUpdate()
    {
        //��״̬��ִ��base�ľ��Ӳ�������ΪҪ���¶��ơ�

        //��״̬���ƶ�
        Move();
    }

    public override void OnExit()
    {
        base.OnExit();
        parameter.isChangeMirrorMode = false;
        EventCenter.Instance.EventTrigger(E_EventType.E_OutMirrorModeChange);
    }

    //�л����ӵ�ʧ�����
    public void changeMirrorActive()
    {
        if (parameter.MirrorControlMode)
        {
            foreach (var mirror in parameter.mirrors)
            {
                if (mirror.Value.name == "Mirror1")
                {
                    mirror.Value.gameObject.SetActive(false);
                }
                if (mirror.Value.name == "Mirror2")
                {
                    mirror.Value.gameObject.SetActive(false);
                }
                if (mirror.Value.name == "Mirror3")
                {
                    mirror.Value.gameObject.SetActive(true);
                    mirror.Value.transform.localScale = parameter.mirror3OriginalScale * parameter.minScale;
                }
            }
        }
        else
        {
            foreach (var mirror in parameter.mirrors)
            {
                if (mirror.Value.name == "Mirror1")
                {
                    mirror.Value.gameObject.SetActive(true);
                    mirror.Value.transform.localScale = parameter.mirror1OriginalScale * parameter.minScale;
                }
                if (mirror.Value.name == "Mirror2")
                {
                    mirror.Value.gameObject.SetActive(true);
                    mirror.Value.transform.localScale = parameter.mirror2OriginalScale * parameter.minScale;
                }
                if (mirror.Value.name == "Mirror3")
                {
                    mirror.Value.gameObject.SetActive(false);
                }
            }
        }
    }

    //�ڶ��׶γ�����ʱ��
    IEnumerator DurationStep2()
    {
        yield return new WaitForSeconds(parameter.reduceDuration); // �ȴ�
        step3 = true; //�����Ժ��л��������׶�
    }

    IEnumerator DurationStep3()
    {
        yield return new WaitForSeconds(parameter.reduceDuration); // �ȴ�
        step = 4; //�����Ժ��л������Ľ׶�
    }
}
