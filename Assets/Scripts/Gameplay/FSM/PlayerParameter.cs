using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerParameter : Parameter
{
    [Header("基本属性值")]
    public int HP; //当前的HP值
    public int HPMax = 3; //HP的最大值，目前是3

    [Header("默认信息")]
    public Vector3 defaultPosition = new Vector3(10000,10000,-1);//玩家在没进关卡之前的默认位置


    [Header("移动")]
    public float normalSpeed = 12f; // 默认移动速度
    public float jumpSpeed = 6f; // 跳跃时的移动速度
    public float acceration = 40f;//加速过渡值
    public float deceleration = 40f;//减速过渡值
    [HideInInspector] public float Gravity;//重力
    //朝向除了改变角色方向，还有两作用：1决定冲锋的方向2.用来确定未面向墙壁的方向
    [HideInInspector] public int facingDirection = 1; // 角色面向的方向，1右 -1左，不用bool是为了方便直接参与运算
    [HideInInspector] public Vector2 inputDirection; // 输入的移动方向
    [HideInInspector] public float currentSpeed; // 当前移动速度
    //如果不触壁有输入 或者 触墙反方向移动才可以移动
    public bool isMove => (check.isTouchWall && inputDirection.x == -facingDirection) || (!check.isTouchWall && inputDirection.x != 0);

    [Header("跳跃")]
    public float jumpForce = 25;//跳跃力
    public int jumpNum = 1;//可跳跃次数
    [HideInInspector] public int currentJumpNum;//当前跳跃次数
    public bool isClickJump;//是否按下跳跃
    public bool isJump => isClickJump && currentJumpNum < jumpNum;
    public bool isClickStopJump;
    public bool isFall => (rb.velocity.y < 0.1f && !check.isTouchGround) || (isClickStopJump && !check.isTouchGround);

    [Header("受击")]
    public bool isHit; //是否正在受击状态
    public float yingZhi; //硬直时间

    [Header("死亡")]
    public bool isDead;

    [Header("近战攻击")]
    public Vector2 attackMovePostion;//移动补偿
    [HideInInspector] public bool isClickMeleeAttack;//是否点击近战攻击

    [Header("冲锋")]
    public Vector2 dashPostion = new Vector2(35,0);//冲锋速度
    public float dashTime = 0.3f; //冲锋时间
    public float dashCD = 1f; //冲锋CD
    public GameObject shadowPrefab; //冲锋残影
    [HideInInspector] public bool isClickDash;
    [HideInInspector] public bool isDashing;//是否正在冲锋
    [HideInInspector] public bool isWaitDash;//是否CD冷却
    public bool isDash => isClickDash && !isDashing && !isWaitDash;

    [Header("土狼时间")]
    public float coyoteTime = 0.1f;

    [Header("预输入")]
    public float waitJumpInputBufferTime = 0f;//跳跃输入缓冲时间
    public bool HasJumpInputBuffer;//是否存在跳跃缓冲

    [Header("形态")]
    public bool isChangePlayerMode =false;
    public bool form = true; //规定：true是普通形态（实体形态），false是光化形态
    public float changePlayerModeDuration = 0.5f;

    [Header("镜子")]
    //镜子相关操作，状态比较复杂。统一于人物操控的参数里配置。
    public Dictionary<int, GameObject> mirrors = new Dictionary<int, GameObject>(); //存储着镜子实例的字典。1、2为传送模式的两个镜子，3为反射模式的一个镜子。
    public int nowControlMirrorNumber; //目前操控的镜子编号。如果是0代表没控制镜子。
    public int nowControlMirrorOrientation; //目前操控的镜子的角度。
    public List<int> orientationList = new List<int> {0 ,45, 90, 135, 180, 225, 270, 315 };
    public int nowOrientation = 0; //当前所有镜子角度
    public int currentOrientationIndex = 0;
    public List<int> nowSetMirror = new List<int>(); //目前放在场上的镜子编号列表。set以后加入此列表，否则不加入。
    //既不操控，也不放在场上的镜子，就是空闲镜子，会一直跟随主角。
    public Dictionary<int, Vector3> IdleMirrorPosition = new Dictionary<int, Vector3> {
        {1,new Vector3(-1,2,0) },
        {2,new Vector3(1,2,0) },
        {3,new Vector3(1,2,0) }}; //每个镜子空闲时对应的默认跟随位置(相对主角坐标系）
    public float MirrorFollowSpeed = 0.05f; //空闲镜子的默认跟随速度
    public float MirrorFindTargetSpeed = 0.1f; //镜子飞向目标设施的速度
    public float MirrorRecycleSpeed = 0.05f; //镜子回收时飞向玩家的速度
    public Dictionary<int, float> MirrorSpeedNow = new Dictionary<int, float>();
    public bool MirrorControlMode = true;//镜子的操控模式，规定：true是反射模式，false是传送模式,默认为反射模式。
    public bool isControlling = false; //是否在控制镜子的状态
    public Vector2 pointPosition;//点击屏幕的时候的世界位置
    public bool isInteractionPlace => check.isTouchMirrorPlace;
    public bool canEnterMirrorCloseState => (MirrorControlMode && !nowSetMirror.Contains(3)) || (!MirrorControlMode && (!nowSetMirror.Contains(1) || !nowSetMirror.Contains(2)));

    //状态机初始化时获得三个镜子的标准缩放。
    public Vector3 mirror1OriginalScale;
    public Vector3 mirror2OriginalScale;
    public Vector3 mirror3OriginalScale;
    public float reduceDuration = 0.5f; //切换状态时镜子缩小/放大的时长
    public float minScale = 0.1f; //镜子刚出现以及缩小时的时候的最小比例
    public float reduceSpeed => (1 - minScale) / reduceDuration; //镜子变化的速度
    public bool isChangeMirrorMode = false;

    public SpriteShapeController spriteShapeController; //镜子之间的传送标记线
    //是否处于可传送状态
    //public bool canTransfer => nowSetMirror.Count == 2 && check.isTouchMirror && (!check.shouldDetectAway || check.touchedMirrorObj != check.shouldDetectionMirrorObj); //要么处于无需检测状态，要么碰到的是另一个镜子。这样有可能导致反复传送
    public bool canTransfer => nowSetMirror.Count == 2 && check.isTouchMirror && (!check.shouldDetectAway); //处于无需检测状态。这样不能在落地后立马从另一个镜子的一定范围里传送出去，但是可以防止反复传送
    public float flyingSpeed = 0.5f; //传送前，飞向最近的镜子对象时的速度（从0到1）


    [Header("关卡切换状态")]
    public bool isEnteringNextLevel = false;
}

