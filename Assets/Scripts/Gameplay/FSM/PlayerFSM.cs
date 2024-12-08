using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 需要有一个控制类来继承这个状态机，并将控制类挂载在玩家角色身上
/// </summary>
public class PlayerFSM : FiniteStateMachine
{
    public PlayerParameter parameter;  // 状态机参数
    protected override void Awake()
    {
        Debug.Log(transform.gameObject.name);

        parameter = GetComponent<PlayerParameter>();
        parameter.rb = GetComponent<Rigidbody2D>();
        parameter.animator = GetComponent<Animator>();
        parameter.sr = GetComponent<SpriteRenderer>();
        parameter.check = GetComponent<PhysicsCheck>();
        

        //为参数绑定镜子
        GameObject[] mirrors = GameObject.FindGameObjectsWithTag("mirror");
        foreach(var mirror in mirrors)
        {
            //默认进入反射状态，所以把镜子1和2禁用
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
        

        //从玩家角色身上获取重力
        parameter.Gravity = parameter.rb.gravityScale;

        parameter.HP = parameter.HPMax;

        // 初始化各个状态，并添加到状态字典中
        //玩家对应的状态（目前15个）：
        //空闲、移动、跳跃、下落、落地、冲刺、受击、死亡、接近镜子交互区域、
        //镜子交互状态（控制镜子位置和镜子方向、放置镜子的操作都在这个状态）、举起镜子状态、切换玩家模式的状态、
        //切换镜子模式的状态、在镜子之间转送、收回镜子、进入下一关。
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



    // 翻转角色
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

