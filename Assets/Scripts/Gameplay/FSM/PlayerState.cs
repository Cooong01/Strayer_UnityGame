using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ʵ��״̬�ӿڣ���Ϊ��ҵ�״̬�Ļ��ࡣ
/// ���ҵ���⣬��������ã��������Ƿ���״̬��֮������ݹ�ͨ���Լ���һЩ��ÿ��״̬�����е��߼�д��ȥ�����綯�������߼���
/// �ӿ�̫������Ҫһ������������Щ���顣
/// �����ϣ����������ʵ�ֶ�����صĿ���
/// </summary>
[Serializable]
public class PlayerState : IState
{
    protected PlayerFSM manager;
    protected PlayerParameter parameter;
    protected string animationName;
    protected AnimatorStateInfo animatorStateInfo;   // ����״̬��Ϣ

    //�����ڴ���ĳЩ״̬��ʱ���п��ܶ�����Ҫ���ݸ������ȼ��Ķ��������ţ�ͬʱ���˳���״̬�������Ҫ��¼�������ȼ��Ķ�����
    private string realAnimationName; //�����Ķ�������
    private bool isMove;
    private bool alreadyMove;
    private bool alreadyReal;

    //��ʱ���������������״̬�л����߼��ж�
    float stateStartTime;
    protected float StateDuration => Time.time - stateStartTime;

    public PlayerState(PlayerFSM manager, string animationName)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
        this.animationName = animationName;
    }

    public virtual void OnEnter()
    {
        alreadyMove = false;
        Debug.Log("����ת��״̬");
        stateStartTime = Time.time;
        realAnimationName = this.animationName;
        //����ǰ����ƽ�����ɵ���ΪanimationName�Ķ���״̬�����ҹ��ɳ���ʱ��Ϊ 0.1 ��
        parameter.animator.CrossFade(animationName, 0.1f);
    }

    public virtual void OnUpdate()
    {
        if(parameter.rb.velocity.x == 0)
        {
            isMove = false;
        }
        if (isMove && !alreadyMove)
        {
            alreadyMove = true;
            alreadyReal = false;
            parameter.animator.CrossFade("Run", 0.1f);
        }
        else if (isMove && alreadyMove)
        {
            
        }
        else if (!isMove && alreadyReal)
        {
        }
        else if (!isMove && !alreadyReal)
        {
            alreadyMove = false;
            alreadyReal = true;
            Debug.Log(realAnimationName);
            parameter.animator.CrossFade(realAnimationName, 0.1f);
        }
        //�������
        if (parameter.isDead)
        {
            //ʧ��
            EventCenter.Instance.EventTrigger(E_EventType.E_FailLevel);
            Level.Instance.DestroyLevel();
            parameter.isDead = false;
        }

        animatorStateInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);// ��ȡ��ǰ����״̬��Ϣ 

        if (parameter.rb.velocity.y < 0.1f && parameter.check.isTouchGround)
        {
            parameter.currentJumpNum = 0;//�Ӵ�����������Ծ����
        }
        //���ӵ�һЩ�Զ���Ϊ��������ֱ��д�ϡ�

        foreach (var mirror in parameter.mirrors)
        {
            //������Ӳ����ٿ��Ҳ��ڷ���״̬���͸�������
            if (mirror.Key != parameter.nowControlMirrorNumber && !parameter.nowSetMirror.Contains(mirror.Key))
            {
                MoveMirror(mirror.Key);
            }
        }


        if (parameter.nowSetMirror.Contains(1))
        {
            parameter.mirrors[1].GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            parameter.mirrors[1].GetComponent<BoxCollider2D>().enabled = false;
        }

        if (parameter.nowSetMirror.Contains(2))
        {
            parameter.mirrors[2].GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            parameter.mirrors[2].GetComponent<BoxCollider2D>().enabled = false;
        }

        if (parameter.nowSetMirror.Contains(3))
        {
            parameter.mirrors[3].GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            parameter.mirrors[3].GetComponent<BoxCollider2D>().enabled = false;
        }


    }


    public virtual void OnFixedUpdate() 
    {

    }

    public virtual void OnExit() { }

    //�����Ƿ񲥷����
    public bool PlayComplete()
    {
        // ��ǰ�����Ƿ񲥷���95%
        if (animatorStateInfo.normalizedTime >= .95f && animatorStateInfo.IsName(animationName)) return true;
        return false;
    }

    //�ƶ�.
    public void Move()
    {
        isMove = true;
        // �������뷽���ƶ���ɫ
        SetVelocity(parameter.inputDirection.x * parameter.currentSpeed, parameter.rb.velocity.y);
        //ֻ���ƶ����ܷ�ת��ɫ��
        //��¼��ɫ��תǰ��
        manager.FlipTo();
    }

    //��������ٶ�
    public void SetVelocity(float setVelocityX, float setVelocityY)
    {
        parameter.rb.velocity = new Vector2(setVelocityX, setVelocityY);
    }
    
    //���þ���λ�ڽ�ɫ��Χ�̶�λ��.�����Ƿ񵽴�Ŀ��λ��
    public bool MoveMirror(int mirrorNumber)
    {
        Vector3 mirrorPositionNow = parameter.mirrors[mirrorNumber].transform.position;
        parameter.mirrors[mirrorNumber].transform.position = Vector3.Lerp(mirrorPositionNow, manager.transform.TransformPoint(parameter.IdleMirrorPosition[mirrorNumber]),parameter.MirrorFollowSpeed);

        return Vector3.Distance(mirrorPositionNow, manager.transform.TransformPoint(parameter.IdleMirrorPosition[mirrorNumber])) <= 0.1;
    }

    public bool isOnTargetPostion(int mirrorNumber)
    {
        Vector3 mirrorPositionNow = parameter.mirrors[mirrorNumber].transform.position;
        return Vector3.Distance(mirrorPositionNow, manager.transform.TransformPoint(parameter.IdleMirrorPosition[mirrorNumber])) <= 0.1;
    }

}

