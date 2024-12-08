using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// π“‘ÿ‘⁄“∆∂Ø“°∏À…œ
/// </summary>
public class JoystickMove : Joystick
{

    public override void OnDrag(PointerEventData ped)
    {
        base.OnDrag(ped);

        EventCenter.Instance.EventTrigger(E_EventType.E_PressMoveButton, InputDirection);
    }

    public override void OnPointerUp(PointerEventData ped)
    {
        base.OnPointerUp(ped);
        EventCenter.Instance.EventTrigger(E_EventType.E_UpMoveButton);
    }


}
