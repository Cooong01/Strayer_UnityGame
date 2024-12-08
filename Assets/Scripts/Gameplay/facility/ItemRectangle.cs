using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemRectangle : BaseFacility , IPointerDownHandler
{
    //用来计算可移动的直线路径的锚点
    public GameObject anchor1;
    public GameObject anchor2;

    //当前设施是否包含镜子
    public bool isContainMirror = false;
    public GameObject ContainMirrorObj;

    void Start()
    {
        anchor1 = transform.Find("Anchor1").gameObject;
        anchor2 = transform.Find("Anchor2").gameObject;
    }

    private void OnDrawGizmos()
    {         
        //在Scene视图中的辅助线
        Gizmos.color = Color.red;
        Gizmos.DrawLine(anchor1.transform.position, anchor2.transform.position);
    }


    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("被点击了");
    }
}
