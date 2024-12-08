using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerControlMirrorPostionState : PlayerState
{
    public PlayerControlMirrorPostionState(PlayerFSM manager, string animationName) : base(manager, animationName) { }


    public Transform anchor1; //Ŀ������ĵ�һ��ê��
    public Transform anchor2; //Ŀ������ĵڶ���ê��
    public Vector3 anchor1Position;//Ŀ������ĵ�һ��ê��λ��
    public Vector3 anchor2Position;//Ŀ������ĵڶ���ê��λ��
    public Vector3 nowMirrorPosition; //Ŀǰ���Ƶľ��ӵ�λ��
    public bool isOnFacility = false; //���ڿ��Ƶľ���������ʩ��
    public bool isSet = false;
    public float startDistance = 0; //�������㾵�ӷ��й����е���ת�ǶȲ�ֵ
    public bool added = false;
    public bool orientated = true;
    private Vector3 lastPosition; 
    private Vector3 targetPosition; //��������ʩʱ��


    public override void OnEnter()
    {
        base.OnEnter();
        EnterMirrorPlaceMode(parameter.nowControlMirrorNumber);
        isOnFacility = false;
        Debug.Log("���ڲٿؾ���״̬");
        startDistance = Vector3.Distance(anchor1Position, parameter.mirrors[parameter.nowControlMirrorNumber].transform.position);
        added = false;
        isSet = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //����뿪�˿ɽ������룬�Զ�ȡ���ٿأ���������״̬���������ȡ�����ӽ�����Ҳȡ���ٿأ���������״̬��
        if (!parameter.isInteractionPlace)
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_LeaveMirrorPlace); //�����뿪����״̬���¼�
            //
            manager.TransitionState(E_PlayerStateType.Idle);
        }
        if (parameter.isDash)
        {
            manager.TransitionState(E_PlayerStateType.Dash);
        }
        if (parameter.isJump)
        {
            manager.TransitionState(E_PlayerStateType.Jump);
        }
        if (parameter.isChangeMirrorMode)
        {
            manager.TransitionState(E_PlayerStateType.Mirror_ChangeMirrorMode);
        }
        //�����ڵ��ٶ��𽥱��Ĭ�����٣�ģ�¼��ٵĹ���
        parameter.currentSpeed = Mathf.MoveTowards(parameter.currentSpeed, parameter.normalSpeed, parameter.acceration * Time.deltaTime);

        if (parameter.isControlling) //�����ڿ���ģʽ�ϣ��ͽ���������ӵ�ģʽ
        {
            ControlMirrorPosition();
        }

    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (!isOnFacility && Vector3.Distance(targetPosition, parameter.mirrors[parameter.nowControlMirrorNumber].transform.position) <= 0.1)
        {
            parameter.mirrors[parameter.nowControlMirrorNumber].transform.position = targetPosition;
            parameter.isControlling = true;
            isOnFacility = true;
            //ע������������ӽǶȺͷ��þ��ӵ��¼�
            orientated = false;
            EventCenter.Instance.AddEventListener(E_EventType.E_PressMirrorOrientationButton, ChangeMirrorOrientation);
            EventCenter.Instance.AddEventListener(E_EventType.E_PressMirrorSetButton, SetMirror);
            Vector3 eulerAngle = parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles;
            eulerAngle.z = parameter.nowOrientation;
            parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles = eulerAngle;
            
        }
        if (!isOnFacility) //���������ʩ�ϣ��ͷ�����ʩ
        {
            //ÿ֡�ƶ����ӵ���ʩ�ĵ�һ��ê����
            parameter.mirrors[parameter.nowControlMirrorNumber].transform.position = Vector3.Lerp(parameter.mirrors[parameter.nowControlMirrorNumber].transform.position, targetPosition, parameter.MirrorFindTargetSpeed);

            Vector3 eulerAngle = parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles;
            eulerAngle.z = Mathf.LerpAngle(eulerAngle.z,parameter.nowOrientation,Vector3.Distance(targetPosition, parameter.mirrors[parameter.nowControlMirrorNumber].transform.position) /startDistance);
            parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles = eulerAngle;
        }

        //��״̬�����ƶ�
        Move();
    }

    public override void OnExit()
    {
        base.OnExit();
        //todo:�˳���ʱ����ն�Ӧ�¼��ļ���,��û���µľ����ջ�
        if (!isSet)
        {
            Vector3 euler = parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles;
            euler.z = 0;
            parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles = euler;
        }


        parameter.isControlling = false;
        parameter.nowControlMirrorNumber = 0;

        EventCenter.Instance.EventTrigger(E_EventType.E_LeaveMirrorPlace);
        EventCenter.Instance.Claer(E_EventType.E_PressMirrorSetButton);
        EventCenter.Instance.Claer(E_EventType.E_PressMirrorOrientationButton);

    }

    public void ControlMirrorPosition()
    {

        float distance1 = Vector3.Distance(parameter.pointPosition, anchor1Position);
        float distance2 = Vector3.Distance(parameter.pointPosition, anchor2Position);
        float distanceAnchors = Vector3.Distance((Vector2)anchor1Position, (Vector2)anchor2Position);
        //ÿһ֡������ҵ����λ�����þ��ӵ�λ�õ���ҵ����ͶӰλ��
        //��������������ʩһ����Χ�ڣ����ƶ����ӣ������ƶ�

        if (Projection.DistanceFromPoint2Line(parameter.pointPosition, anchor1Position, anchor2Position) <= 4)
        {

            Vector2 projectionPosition = Projection.ProjectPointOntoLine(parameter.pointPosition, anchor1Position, anchor2Position);
            Debug.Log(parameter.pointPosition.y);
            //����ı�֮���λ�õ���ײ������ǽ�ˣ��Ͳ�����ı䣬������һ��λ�á�

            parameter.mirrors[parameter.nowControlMirrorNumber].transform.position = new Vector3(projectionPosition.x, projectionPosition.y, parameter.mirrors[parameter.nowControlMirrorNumber].transform.position.z);
            //todo�Ż�:����1����һ��isControlling��ǣ��������Ŀǰ����ǽ��ڸñ��Ϊfalse��ʱ�򣬾��ӷ�������ķ�ǽλ�á�
            //����2��:��һ����canSet����ǣ������Ӵ��ڷ�ǽλ�õ�ʱ�򣬲�����set��
        }
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Wall", "Ground"));
        Collider2D[] result = new Collider2D[10];
        Debug.Log(parameter.mirrors[parameter.nowControlMirrorNumber].transform.Find("detection").GetComponent<CircleCollider2D>().OverlapCollider(contactFilter, result));
        if (parameter.nowControlMirrorNumber == 1 || parameter.nowControlMirrorNumber ==2)
        {
            if (parameter.mirrors[parameter.nowControlMirrorNumber].transform.Find("detection").GetComponent<CircleCollider2D>().OverlapCollider(contactFilter, result) > 0)
            {
                EventCenter.Instance.EventTrigger(E_EventType.E_LeaveMirrorPlace);
            }
            else
            {
                EventCenter.Instance.EventTrigger(E_EventType.E_EnterMirrorInteraction);
            }
        }
        else
        {

            EventCenter.Instance.EventTrigger(E_EventType.E_EnterMirrorInteraction);
        }

    }

    //���뾵��ģʽ
    public void EnterMirrorPlaceMode(int mirrorNumber)
    {
        //Ҫ�þ����Զ�������ʩ��Ĭ��λ��
        //�����ҵ�Ŀ����ʩ��Ȼ��

        anchor1 = parameter.check.touchedMirrorPlaceObj.transform.GetComponent<ItemRectangle>().anchor1.transform;
        anchor2 = parameter.check.touchedMirrorPlaceObj.transform.GetComponent<ItemRectangle>().anchor2.transform;

        //�������ê�����������
        anchor1Position = anchor1.position;
        anchor2Position = anchor2.position;

        targetPosition = (anchor1Position + anchor2Position) / 2;

        nowMirrorPosition = parameter.mirrors[mirrorNumber].transform.position;
        parameter.mirrors[mirrorNumber].transform.position = Vector3.Lerp(nowMirrorPosition, targetPosition, parameter.MirrorFindTargetSpeed);
    }

    //�ı侵�ӽǶ�
    public void ChangeMirrorOrientation() 
    {
        MusicMgr.Instance.PlaySound("Heavy Object Move");
        orientated = true;
        int orientation = parameter.orientationList[ ++parameter.currentOrientationIndex % parameter.orientationList.Count];
        Debug.Log(orientation);
        Vector3 eulerAngle = parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles;
        parameter.nowOrientation = orientation;
        eulerAngle.z = parameter.nowOrientation;
        parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles = eulerAngle;


        foreach (var mirror in parameter.nowSetMirror)
        {
            eulerAngle = parameter.mirrors[mirror].transform.eulerAngles;
            eulerAngle.z = parameter.nowOrientation;
            parameter.mirrors[mirror].transform.eulerAngles = eulerAngle;
        }
    }

    //���¾���,�˳���״̬
    public void SetMirror()
    {
        MusicMgr.Instance.PlaySound("Whoosh Blade 003");
        if (!added)
        {
            MusicMgr.Instance.PlaySound("setMirror");
            isSet = true;
            added = true;
            parameter.nowSetMirror.Add(parameter.nowControlMirrorNumber);
            //ֻҪ�����˾��ӣ���֪ͨ���հ�ť����
            EventCenter.Instance.EventTrigger(E_EventType.E_EnterMirrorRecycle, parameter.nowControlMirrorNumber);
            manager.TransitionState(E_PlayerStateType.Idle);
        }
    }
}
