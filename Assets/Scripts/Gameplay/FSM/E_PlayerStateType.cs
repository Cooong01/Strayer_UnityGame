using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_PlayerStateType
{
    Idle, //����
    Move, //�ƶ�
    Dash, //���
    Hit, //�ܻ�
    Death,//����
    Jump,//��Ծ
    Fall, //����
    Land,//���
    ChangePlayerMode,//�л������̬

    //�����Ǿ�����ز���
    Mirror_ChangeMirrorMode,//�л����Ӳٿ�ģʽ
    Mirror_CloseMirrorPlace,//�������Ӳٿ����� 1
    Mirror_ControlMirror,//���ƾ��� 1
    Mirror_HoldMirror,//������
    Mirror_RecycleMirror,//�ջؾ���
    Mirror_transfer,//�ھ���֮�䴫��

    //ת�ƹؿ��Ĳ���
    enterNextLevel,
}
