using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �¼����� ö��
/// ������Ҫע����¼���Ҫ������д�ϣ�����ע�ͱ�ע�������ͣ����ڱ�д������ͣ��ʱ��鿴��
/// </summary>
public enum E_EventType 
{

    /// <summary>
    /// �����л�ʱ�Ľ��ȱ仯
    /// ������float�����ؽ���
    /// </summary>
    E_SceneLoadChange,

    /// <summary>
    /// ˮƽ�ȼ� -1~1���¼�����
    /// </summary>
    E_Input_Horizontal,

    /// <summary>
    /// ��ֱ�ȼ� -1~1���¼�����
    /// </summary>
    E_Input_Vertical,

    /// <summary>
    /// ��ʾ�¼�
    /// ������string�����������ʾ��Ϣ
    /// </summary>
    E_Tip,

    /// <summary>
    /// ר��������Ҫ�������̡߳����к�ʱ�ȴ�ʱ��������ڼ�չʾ���ض�����
    /// </summary>
    E_TimeBlocking,

    /// <summary>
    /// �����ƶ�ҡ��
    /// ������PointerEventData��ҡ��λ��
    /// </summary>
    E_PressMoveButton,

    /// <summary>
    /// ̧���ƶ�����/ҡ��
    /// </summary>
    E_UpMoveButton,


    /// <summary>
    /// ������Ծ��ť
    /// </summary>
    E_PressJumpButton,

    /// <summary>
    /// ���³�̰�ť
    /// </summary>
    E_PressDashButton,

    /// <summary>
    /// ̧���̰�ť
    /// </summary>
    E_UpDashButton,

    /// <summary>
    /// ����ҡ��
    /// ������int��ҡ�˱��
    /// </summary>
    E_PressJoystick,

    /// <summary>
    /// �϶�ҡ��
    /// ������int��ҡ�˱�ţ�PointerEventData��ҡ��λ��
    /// </summary>
    E_DragJoyStick,

    /// <summary>
    /// ̧��ҡ��
    /// ������int��ҡ�˱��
    /// </summary>
    E_UpJoyStick,

    /// <summary>
    /// ���¾��ӷ����л���ť
    /// ������int����ť���
    /// </summary>
    E_PressMirrorOrientationButton,

    /// <summary>
    /// ���¾��ӷ��ð�ť��
    /// ������int�����ӱ��
    /// </summary>
    E_PressMirrorSetButton,

    /// <summary>
    /// ���¾���ģʽ�л���ť
    /// </summary>
    E_PressChangeMirrorModeButton,

    /// <summary>
    /// �˳�����ģʽ�л�״̬
    /// </summary>
    E_OutMirrorModeChange,

    /// <summary>
    /// �˳����ģʽ�л�״̬
    /// </summary>
    E_OutPlayerModeChange,


    /// <summary>
    /// �������ģʽ�л���ť
    /// </summary>
    E_PressChangePlayerModeButton,

    /// <summary>
    /// �ӽ����þ��ӵ�λ��
    /// ������int,����Ҫ��ʾ��ť�ľ��ӱ��
    /// </summary>
    E_CloseMirrorPlace,

    /// <summary>
    /// ���뾵�ӷ��ý���״̬
    /// ������int�����ӱ��
    /// </summary>
    E_EnterMirrorInteraction,

    /// <summary>
    /// �뿪���ӽ�������
    /// </summary>
    E_LeaveMirrorPlace,

    /// <summary>
    /// ������ƾ���״̬
    /// ������int��Ҫ���Ƶľ��ӱ��
    /// </summary>
    E_ControlMirror,

    
    /// <summary>
    /// �о�����Ҫ����
    /// ������int��Ҫ���Ƶľ��ӱ��
    /// </summary>
    E_EnterMirrorRecycle,

    /// <summary>
    /// ���ؾ��ӻ��հ���
    /// ������int��Ҫ���յľ��ӱ��
    /// </summary>
    E_OutMirrorRecycle,

    /// <summary>
    /// ���¾��ӻ��հ�ť
    /// ������int��Ҫ���յľ��ӱ��
    /// </summary>
    E_PressMirrorRecycleButton,

    /// <summary>
    /// ����ܻ�
    /// </summary>
    E_PlayerAttacked,

    /// <summary>
    /// ���Ѫ�����١���������UI��ʾ�ķ���
    /// ������int�����ڼ��ٵ�x��Ѫ
    /// </summary>
    E_PlayerHPDecreasing,

    /// <summary>
    /// ���齻��
    /// </summary>
    E_TalkWithOther,

    /// <summary>
    /// ����ؿ�
    /// </summary>
    E_EnterLevel,

    /// <summary>
    /// �ؿ�ʧ��
    /// </summary>
    E_FailLevel,

    /// <summary>
    /// �ؿ��ɹ�
    /// </summary>
    E_SuccessLevel,

    /// <summary>
    /// ����ͨ��
    /// </summary>
    E_AllSuccessLevel,

    /// <summary>
    /// �ӽ��Ի���
    /// </summary>
    E_CloseTalk,

    /// <summary>
    /// �뿪�Ի���
    /// </summary>
    E_LeaveTalk,

    /// <summary>
    /// ������ʧ�ܣ�Ϊ�˽̳̹ؿ���������������ã�
    /// </summary>
    E_RealFail,
    
    /// <summary>
    /// �ﵽNpc���ȵ�Ҫ��
    /// </summary>
    E_isValid2SaveNPC

}
