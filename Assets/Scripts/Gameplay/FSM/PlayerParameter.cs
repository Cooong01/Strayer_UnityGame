using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerParameter : Parameter
{
    [Header("��������ֵ")]
    public int HP; //��ǰ��HPֵ
    public int HPMax = 3; //HP�����ֵ��Ŀǰ��3

    [Header("Ĭ����Ϣ")]
    public Vector3 defaultPosition = new Vector3(10000,10000,-1);//�����û���ؿ�֮ǰ��Ĭ��λ��


    [Header("�ƶ�")]
    public float normalSpeed = 12f; // Ĭ���ƶ��ٶ�
    public float jumpSpeed = 6f; // ��Ծʱ���ƶ��ٶ�
    public float acceration = 40f;//���ٹ���ֵ
    public float deceleration = 40f;//���ٹ���ֵ
    [HideInInspector] public float Gravity;//����
    //������˸ı��ɫ���򣬻��������ã�1�������ķ���2.����ȷ��δ����ǽ�ڵķ���
    [HideInInspector] public int facingDirection = 1; // ��ɫ����ķ���1�� -1�󣬲���bool��Ϊ�˷���ֱ�Ӳ�������
    [HideInInspector] public Vector2 inputDirection; // ������ƶ�����
    [HideInInspector] public float currentSpeed; // ��ǰ�ƶ��ٶ�
    //��������������� ���� ��ǽ�������ƶ��ſ����ƶ�
    public bool isMove => (check.isTouchWall && inputDirection.x == -facingDirection) || (!check.isTouchWall && inputDirection.x != 0);

    [Header("��Ծ")]
    public float jumpForce = 25;//��Ծ��
    public int jumpNum = 1;//����Ծ����
    [HideInInspector] public int currentJumpNum;//��ǰ��Ծ����
    public bool isClickJump;//�Ƿ�����Ծ
    public bool isJump => isClickJump && currentJumpNum < jumpNum;
    public bool isClickStopJump;
    public bool isFall => (rb.velocity.y < 0.1f && !check.isTouchGround) || (isClickStopJump && !check.isTouchGround);

    [Header("�ܻ�")]
    public bool isHit; //�Ƿ������ܻ�״̬
    public float yingZhi; //Ӳֱʱ��

    [Header("����")]
    public bool isDead;

    [Header("��ս����")]
    public Vector2 attackMovePostion;//�ƶ�����
    [HideInInspector] public bool isClickMeleeAttack;//�Ƿ�����ս����

    [Header("���")]
    public Vector2 dashPostion = new Vector2(35,0);//����ٶ�
    public float dashTime = 0.3f; //���ʱ��
    public float dashCD = 1f; //���CD
    public GameObject shadowPrefab; //����Ӱ
    [HideInInspector] public bool isClickDash;
    [HideInInspector] public bool isDashing;//�Ƿ����ڳ��
    [HideInInspector] public bool isWaitDash;//�Ƿ�CD��ȴ
    public bool isDash => isClickDash && !isDashing && !isWaitDash;

    [Header("����ʱ��")]
    public float coyoteTime = 0.1f;

    [Header("Ԥ����")]
    public float waitJumpInputBufferTime = 0f;//��Ծ���뻺��ʱ��
    public bool HasJumpInputBuffer;//�Ƿ������Ծ����

    [Header("��̬")]
    public bool isChangePlayerMode =false;
    public bool form = true; //�涨��true����ͨ��̬��ʵ����̬����false�ǹ⻯��̬
    public float changePlayerModeDuration = 0.5f;

    [Header("����")]
    //������ز�����״̬�Ƚϸ��ӡ�ͳһ������ٿصĲ��������á�
    public Dictionary<int, GameObject> mirrors = new Dictionary<int, GameObject>(); //�洢�ž���ʵ�����ֵ䡣1��2Ϊ����ģʽ���������ӣ�3Ϊ����ģʽ��һ�����ӡ�
    public int nowControlMirrorNumber; //Ŀǰ�ٿصľ��ӱ�š������0����û���ƾ��ӡ�
    public int nowControlMirrorOrientation; //Ŀǰ�ٿصľ��ӵĽǶȡ�
    public List<int> orientationList = new List<int> {0 ,45, 90, 135, 180, 225, 270, 315 };
    public int nowOrientation = 0; //��ǰ���о��ӽǶ�
    public int currentOrientationIndex = 0;
    public List<int> nowSetMirror = new List<int>(); //Ŀǰ���ڳ��ϵľ��ӱ���б�set�Ժ������б����򲻼��롣
    //�Ȳ��ٿأ�Ҳ�����ڳ��ϵľ��ӣ����ǿ��о��ӣ���һֱ�������ǡ�
    public Dictionary<int, Vector3> IdleMirrorPosition = new Dictionary<int, Vector3> {
        {1,new Vector3(-1,2,0) },
        {2,new Vector3(1,2,0) },
        {3,new Vector3(1,2,0) }}; //ÿ�����ӿ���ʱ��Ӧ��Ĭ�ϸ���λ��(�����������ϵ��
    public float MirrorFollowSpeed = 0.05f; //���о��ӵ�Ĭ�ϸ����ٶ�
    public float MirrorFindTargetSpeed = 0.1f; //���ӷ���Ŀ����ʩ���ٶ�
    public float MirrorRecycleSpeed = 0.05f; //���ӻ���ʱ������ҵ��ٶ�
    public Dictionary<int, float> MirrorSpeedNow = new Dictionary<int, float>();
    public bool MirrorControlMode = true;//���ӵĲٿ�ģʽ���涨��true�Ƿ���ģʽ��false�Ǵ���ģʽ,Ĭ��Ϊ����ģʽ��
    public bool isControlling = false; //�Ƿ��ڿ��ƾ��ӵ�״̬
    public Vector2 pointPosition;//�����Ļ��ʱ�������λ��
    public bool isInteractionPlace => check.isTouchMirrorPlace;
    public bool canEnterMirrorCloseState => (MirrorControlMode && !nowSetMirror.Contains(3)) || (!MirrorControlMode && (!nowSetMirror.Contains(1) || !nowSetMirror.Contains(2)));

    //״̬����ʼ��ʱ����������ӵı�׼���š�
    public Vector3 mirror1OriginalScale;
    public Vector3 mirror2OriginalScale;
    public Vector3 mirror3OriginalScale;
    public float reduceDuration = 0.5f; //�л�״̬ʱ������С/�Ŵ��ʱ��
    public float minScale = 0.1f; //���Ӹճ����Լ���Сʱ��ʱ�����С����
    public float reduceSpeed => (1 - minScale) / reduceDuration; //���ӱ仯���ٶ�
    public bool isChangeMirrorMode = false;

    public SpriteShapeController spriteShapeController; //����֮��Ĵ��ͱ����
    //�Ƿ��ڿɴ���״̬
    //public bool canTransfer => nowSetMirror.Count == 2 && check.isTouchMirror && (!check.shouldDetectAway || check.touchedMirrorObj != check.shouldDetectionMirrorObj); //Ҫô����������״̬��Ҫô����������һ�����ӡ������п��ܵ��·�������
    public bool canTransfer => nowSetMirror.Count == 2 && check.isTouchMirror && (!check.shouldDetectAway); //����������״̬��������������غ��������һ�����ӵ�һ����Χ�ﴫ�ͳ�ȥ�����ǿ��Է�ֹ��������
    public float flyingSpeed = 0.5f; //����ǰ����������ľ��Ӷ���ʱ���ٶȣ���0��1��


    [Header("�ؿ��л�״̬")]
    public bool isEnteringNextLevel = false;
}

