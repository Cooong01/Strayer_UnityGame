using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// ��Ҫ��һ�����������̳����״̬���������������������ҽ�ɫ����
/// </summary>
public class PlayerFSM : FiniteStateMachine
{
    public PlayerParameter parameter;  // ״̬������
    protected override void Awake()
    {
        Debug.Log(transform.gameObject.name);

        parameter = GetComponent<PlayerParameter>();
        parameter.rb = GetComponent<Rigidbody2D>();
        parameter.animator = GetComponent<Animator>();
        parameter.sr = GetComponent<SpriteRenderer>();
        parameter.check = GetComponent<PhysicsCheck>();
        

        //Ϊ�����󶨾���
        GameObject[] mirrors = GameObject.FindGameObjectsWithTag("mirror");
        foreach(var mirror in mirrors)
        {
            //Ĭ�Ͻ��뷴��״̬�����԰Ѿ���1��2����
            if (mirror.name == "Mirror1")
            {
                parameter.mirrors.Add(1,mirror);
                parameter.mirror1OriginalScale = mirror.transform.localScale;
                mirror.gameObject.SetActive(false);
            }
            if (mirror.name == "Mirror2")
            {
                parameter.mirrors.Add(2, mirror);
                parameter.mirror2OriginalScale = mirror.transform.localScale;
                mirror.gameObject.SetActive(false);
            }
            if (mirror.name == "Mirror3")
            {
                parameter.mirror3OriginalScale = mirror.transform.localScale;
                parameter.mirrors.Add(3, mirror);
                mirror.gameObject.SetActive(true);
            }
        }
        

        //����ҽ�ɫ���ϻ�ȡ����
        parameter.Gravity = parameter.rb.gravityScale;

        parameter.HP = parameter.HPMax;

        // ��ʼ������״̬������ӵ�״̬�ֵ���
        //��Ҷ�Ӧ��״̬��Ŀǰ15������
        //���С��ƶ�����Ծ�����䡢��ء���̡��ܻ����������ӽ����ӽ�������
        //���ӽ���״̬�����ƾ���λ�ú;��ӷ��򡢷��þ��ӵĲ����������״̬����������״̬���л����ģʽ��״̬��
        //�л�����ģʽ��״̬���ھ���֮��ת�͡��ջؾ��ӡ�������һ�ء�
        states.Add(E_PlayerStateType.Idle, new PlayerIdleState(this, "Idle"));
        states.Add(E_PlayerStateType.Move, new PlayerMoveState(this, "Run"));
        states.Add(E_PlayerStateType.Jump, new PlayerJumpState(this, "Jump"));
        states.Add(E_PlayerStateType.Fall, new PlayerFallState(this, "Fall"));
        states.Add(E_PlayerStateType.Land, new PlayerLandState(this, "Land"));
        states.Add(E_PlayerStateType.Dash, new PlayerDashState(this, "Dash"));
        states.Add(E_PlayerStateType.Hit, new PlayerHitState(this, "Hit"));
        states.Add(E_PlayerStateType.Death, new PlayerDeathState(this, "Death"));

        states.Add(E_PlayerStateType.Mirror_CloseMirrorPlace, new PlayerCloseMirrorPlaceState(this, "Close"));
        states.Add(E_PlayerStateType.Mirror_ControlMirror, new PlayerControlMirrorPostionState(this, "Control"));
        states.Add(E_PlayerStateType.Mirror_HoldMirror, new PlayerHoldMirrorState(this, "Hold"));
        states.Add(E_PlayerStateType.ChangePlayerMode, new PlayerChangePlayerModeState(this, "ChangePlayer"));
        states.Add(E_PlayerStateType.Mirror_ChangeMirrorMode, new PlayerChangeMirrorModeState(this, "ChangeMirror"));
        states.Add(E_PlayerStateType.Mirror_transfer, new PlayerTransferState(this, "Transfer"));
        states.Add(E_PlayerStateType.Mirror_RecycleMirror, new PlayerRecycleMirrorState(this, "Recycle"));
        states.Add(E_PlayerStateType.enterNextLevel, new PlayerEnterNextLevelState(this, "EnterNextLevel"));


        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }



    // ��ת��ɫ
    public override void FlipTo()
    {
        if (parameter.inputDirection.x < 0)
        {
            parameter.facingDirection = -1;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        if (parameter.inputDirection.x > 0)
        {
            parameter.facingDirection = 1;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

    }

    public void OnStartCoroutine(IEnumerator name)
    {
        StartCoroutine(name);
    }
}

