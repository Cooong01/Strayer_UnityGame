using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ҡ�˻��ࡣ�̳д�ҡ�˻��࣬ͨ����дOnDrag�����в�ͬ���͵�ҡ�˸��Ի����ƣ��Լ�Ϊ��ͬ��ҡ�˼��ϲ�ͬ���¼�������䡣
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
    
    //��ҡ�˱����ʱ������ҡ�˵ķ�������������ǿ�ȣ�
    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 position = Vector2.zero;

        //��ȡ���뷽��
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

        //���ƻ�������
        InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

        //�޶�Joystick���ƶ�������
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
