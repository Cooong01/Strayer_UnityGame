using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �����ڵ������ӵ�ҡ����
/// ��ҡ����ʱ���ã�����ʹ�ã�Ҫ��������������Լ����¼�������
/// </summary>
public class JoystickMovingMirror1 : Joystick
{
    
    //���´����ƶ���������
    public override void OnDrag(PointerEventData ped)
    {
        base.OnDrag(ped);

    }


    //̧�����̣�����ȷ������λ�õ��¼�
    public override void OnPointerUp(PointerEventData ped)
    {
        print("̧������1��");
    }


}
