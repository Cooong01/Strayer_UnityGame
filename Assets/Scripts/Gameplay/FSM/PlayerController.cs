using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.U2D;

/// <summary>
/// �̳������״̬���Ŀ����ࡣ
/// ������д���Ǹ��ֲٿصĻص�������
/// ͨ������������룬���������ı��Ӧ������ֵ��
/// </summary>
public class PlayerController : PlayerFSM
{

    protected override void Awake()
    {
        base.Awake();
        // ע����������¼���Ӧ�ļ����ص�����
        EventCenter.Instance.AddEventListener<Vector3>(E_EventType.E_PressMoveButton, Move);
        EventCenter.Instance.AddEventListener(E_EventType.E_UpMoveButton, StopMove);
        EventCenter.Instance.AddEventListener(E_EventType.E_PressJumpButton, Jump);
        EventCenter.Instance.AddEventListener(E_EventType.E_PressDashButton, Dash);
        EventCenter.Instance.AddEventListener(E_EventType.E_PressChangeMirrorModeButton, TransformMirrorMode);
        EventCenter.Instance.AddEventListener(E_EventType.E_PressChangePlayerModeButton, TransformPlayerMode);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_PressMirrorRecycleButton, RecycleMirror);
        EventCenter.Instance.AddEventListener(E_EventType.E_PlayerAttacked, Dead);

        InitSpriteShape();
        //todo�������¼�
        //EventCenter.Instance.AddEventListener();
        //EventCenter.Instance.AddEventListener<int>(E_EventType.E_DragJoyStick, ControlMirror);
        //EventCenter.Instance.AddEventListener(E_EventType.E_PressChangeMirrorModeButton, TransformMirrorMode);
        //EventCenter.Instance.AddEventListener(E_EventType.E_PressChangePlayerModeButton, TransformPlayerMode);

    }

    protected override void Update()
    {
        base.Update();

        // ����豸����
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            // �����ƶ��豸�Ĵ�������
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == UnityEngine.TouchPhase.Began ||
                    touch.phase == UnityEngine.TouchPhase.Moved ||
                    touch.phase == UnityEngine.TouchPhase.Stationary)
                {
                    // ��ȡ��������Ļ����
                    Vector3 screenPosition = touch.position;
                    // ����Ļ����ת��Ϊ��������
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -1));
                    parameter.pointPosition = new Vector2(worldPosition.x, worldPosition.y);
                }
            }
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            // ���������豸���������
            if (Input.GetMouseButton(0))
            {
                // ��ȡ���������Ļ����
                Vector3 screenPosition = Input.mousePosition;
                // ����Ļ����ת��Ϊ��������
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -1));
                parameter.pointPosition = new Vector2(worldPosition.x, worldPosition.y);
            }
        }


        if (parameter.canTransfer) //������ڿɴ���ģʽ
        {
            UpdateSpriteShape();
        }
        else
        {
            parameter.check.spriteShapeController.gameObject.SetActive(false);
        }
        

    }


    //�ƶ�
    public void Move(Vector3 context)
    {
        if (context.x >0)
        {
            parameter.inputDirection = Vector2.right;
        }
        else if (context.x < 0)
        {
            parameter.inputDirection = Vector2.left;
        }
    }

    //ֹͣ�ƶ�
    public void StopMove()
    {
        parameter.inputDirection = Vector2.zero;
    }

    // ��Ծ
    public void Jump()
    {
        parameter.isClickJump = true;
        parameter.isClickStopJump = false;
        ////Ԥ�����ʱ
        // ���ݲ߻���ƣ�Ԥ�����ѷ���
        //StopCoroutine(PreInputExitTime());
        //StartCoroutine(PreInputExitTime());
    }

    IEnumerator PreInputExitTime()
    {
        parameter.HasJumpInputBuffer = true;
        yield return new WaitForSeconds(parameter.waitJumpInputBufferTime); // �ȴ�
        parameter.HasJumpInputBuffer = false;
    }

    private void Dash()
    {
        parameter.isClickDash = true;
    }

    //�л����ģʽ
    private void TransformPlayerMode()
    {
        parameter.isChangePlayerMode = true;
    }

    //�л�����ģʽ
    private void TransformMirrorMode()
    {
        parameter.isChangeMirrorMode = true;
    }

    //���վ���
    private void RecycleMirror(int mirrorNumber)
    {
        MusicMgr.Instance.PlaySound("recycle");
        parameter.nowSetMirror.Remove(mirrorNumber);
        Vector3 euler = parameter.mirrors[mirrorNumber].transform.eulerAngles;
        euler.z = 0;
        parameter.mirrors[mirrorNumber].transform.eulerAngles = euler;
        //�������鵱ǰ����ʣ�ྵ��������Ϊ0�����þ��ӽǶ�
        if(parameter.nowSetMirror.Count == 0)
        {
            parameter.nowOrientation = parameter.orientationList[0];
        }
    }

    //�ھ���֮�䴫��
    private void Transfer()
    {

    }

    private void InitSpriteShape()
    {
        // ��ȡ Spline ����
        Spline spline = parameter.check.spriteShapeController.spline;
        // ������еĿ��Ƶ�
        spline.Clear();

        // ������㣨��һ�������λ�ã�
        spline.InsertPointAt(0,Vector3.zero);

        // �����յ㣨�ڶ��������λ�ã�
        spline.InsertPointAt(1, Vector3.zero);

        // ȷ������������ֱ������
        spline.SetTangentMode(0, ShapeTangentMode.Linear);
        spline.SetTangentMode(1, ShapeTangentMode.Linear);
    }

    private void UpdateSpriteShape()
    {

        Spline spline = parameter.check.spriteShapeController.spline;
        parameter.check.spriteShapeController.gameObject.SetActive(true);
        print( "��һ��λ��"+parameter.check.spriteShapeController.transform.InverseTransformPoint(parameter.mirrors[1].transform.position));
        Vector3 position1 = parameter.check.spriteShapeController.transform.InverseTransformPoint(parameter.mirrors[1].transform.position);
        position1.z = -3;
        spline.SetPosition(0, position1);
        spline.SetHeight(0, Random.Range(3f, 4f));

        int lastIndex = spline.GetPointCount() - 1;
        print("�ڶ���λ��" + parameter.check.spriteShapeController.transform.InverseTransformPoint(parameter.mirrors[2].transform.position));
        Vector3 position2 = parameter.check.spriteShapeController.transform.InverseTransformPoint(parameter.mirrors[2].transform.position);
        position2.z = -3;
        spline.SetPosition(lastIndex, position2);
        spline.SetHeight(lastIndex, Random.Range(3f, 4f));
        
        parameter.check.spriteShapeController.BakeMesh();
    }
    
    //����ܵ�����
    private void Hit()
    {
        parameter.isHit = true;
    }

    private void Dead()
    {
        print(Level.Instance.nowLevelObj.name);
        if (Level.Instance.nowLevelObj.name == "level0-1") //todo��һ���ܲ����ŵ������ǣ���Ϊ�߻��������ǵ�һ��Ҫ����һ������������������ʡ�
        {
            return;
        }
        EventCenter.Instance.EventTrigger(E_EventType.E_RealFail);
        parameter.isDead = true;
    }

}
