using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 摇杆基类。继承此摇杆基类，通过重写OnDrag来进行不同类型的摇杆个性化定制，以及为不同的摇杆加上不同的事件触发语句。
/// </summary>
public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

    private Image jsContainer;
    private Image joystick;

    public Vector3 InputDirection;

    void Start()
    {

        jsContainer = GetComponent<Image>();
        joystick = transform.GetChild(0).GetComponent<Image>();
        InputDirection = Vector3.zero;
    }
    
    //当摇杆被点击时，返回摇杆的方向向量（包含强度）
    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 position = Vector2.zero;

        //获取输入方向
        RectTransformUtility.ScreenPointToLocalPointInRectangle
                (jsContainer.rectTransform,
                ped.position,
                ped.pressEventCamera,
                out position);

        position.x = position.x / jsContainer.rectTransform.sizeDelta.x;
        position.y = position.y / jsContainer.rectTransform.sizeDelta.y;

        float x = (jsContainer.rectTransform.pivot.x == 1f) ? position.x * 2 + 1 : position.x * 2 - 1;
        float y = (jsContainer.rectTransform.pivot.y == 1f) ? position.y * 2 + 1 : position.y * 2 - 1;

        InputDirection = new Vector3(x, y, 0);

        //控制滑动幅度
        InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

        //限定Joystick能移动的区域
        joystick.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (jsContainer.rectTransform.sizeDelta.x / 3)
                                                               , InputDirection.y * (jsContainer.rectTransform.sizeDelta.y) / 3);

    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {

        InputDirection = Vector3.zero;
        joystick.rectTransform.anchoredPosition = Vector3.zero;
    }

}
