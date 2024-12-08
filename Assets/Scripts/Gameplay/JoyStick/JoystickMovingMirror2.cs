using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// 此摇杆暂时弃用，如需使用，要在两个函数里各自加上事件触发器
public class JoystickMovingMirror2 : Joystick
{
    //按下触发移动镜子轮盘
    public override void OnDrag(PointerEventData ped)
    {
        base.OnDrag(ped);
    }


    //抬起轮盘，触发确定轮盘位置的事件
    public override void OnPointerUp(PointerEventData ped)
    {
        print("抬起轮盘2了");
    }

    public override void OnPointerDown(PointerEventData ped)
    {
        base.OnPointerDown(ped);

    }
}
