using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ����
/// </summary>
public class PlayerTransferState : PlayerState
{
    //���ͷ����������Ƚ�ɫλ���ƶ�������ľ��ӣ�Ȼ���ɫλ��˲������Ϊ��һ������,Ȼ���˳�״̬
    public PlayerTransferState(PlayerFSM manager, string animationName) : base(manager, animationName){}

    private bool isClosing; //���ڽӽ�����ľ���

    private bool isEnd;

    private float nowTime; //������ʱ�ĵ�ǰʱ��

    

    public override void OnEnter()
    {
        //���봫��״̬
        Debug.Log("���ڴ���״̬");
        SetVelocity(0, 0); //�ٶȹ���
        //״ֵ̬����
        isClosing = true;
        isEnd = false;
        nowTime = 0;
        MusicMgr.Instance.PlaySound("transfer");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (isEnd)
        {
            manager.TransitionState(E_PlayerStateType.Idle);
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (isClosing)
        {
            Transform();
        }
        else
        {
            isEnd = true;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        parameter.check.shouldDetectAway = true;
        parameter.check.shouldDetectionMirrorObj = parameter.check.touchedMirrorObj;
    }

    //�ƶ�������
    private void Transform()
    {
        if (Vector3.Distance(manager.transform.position, parameter.check.touchedMirrorObj.transform.position) <= 0.1)
        {
            //ֱ�Ӵ��͵���һ�����ӵ�λ��
            manager.transform.position = FindTheOtherMirror(parameter.check.touchedMirrorObj).transform.position;
            isClosing = false; //�������˾��˳���״̬;
        }
        else
        {
            ////todo:��Ҫ�Ż���
            //manager.transform.position = Vector3.Lerp(manager.transform.position,parameter.check.touchedMirrorObj.transform.position,parameter.flyingSpeed); 
            manager.transform.position = parameter.check.touchedMirrorObj.transform.position;

        }
    }

    private GameObject FindTheOtherMirror(GameObject mirror)
    {
        if (mirror.name == "Mirror1")
        {
            Debug.Log(parameter.mirrors[2].name);
            return parameter.mirrors[2];
        }
        return parameter.mirrors[1];
    }

}

