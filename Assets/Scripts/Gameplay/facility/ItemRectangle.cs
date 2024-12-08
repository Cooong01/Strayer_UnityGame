using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemRectangle : BaseFacility , IPointerDownHandler
{
    //����������ƶ���ֱ��·����ê��
    public GameObject anchor1;
    public GameObject anchor2;

    //��ǰ��ʩ�Ƿ��������
    public bool isContainMirror = false;
    public GameObject ContainMirrorObj;

    void Start()
    {
        anchor1 = transform.Find("Anchor1").gameObject;
        anchor2 = transform.Find("Anchor2").gameObject;
    }

    private void OnDrawGizmos()
    {         
        //��Scene��ͼ�еĸ�����
        Gizmos.color = Color.red;
        Gizmos.DrawLine(anchor1.transform.position, anchor2.transform.position);
    }


    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("�������");
    }
}
