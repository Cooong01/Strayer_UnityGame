using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// ��ҡ����ʱ���ã�����ʹ�ã�Ҫ��������������Լ����¼�������
public class JoystickMovingMirror3 : Joystick
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
