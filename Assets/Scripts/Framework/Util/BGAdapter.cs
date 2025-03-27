using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����ڱ���ͼƬ�ϣ��Ա���ͼƬ������������
/// </summary>
[ExecuteInEditMode]
public class BGAdapter : MonoBehaviour
{
    public Vector2 textureOriginSize = new Vector2(2048, 1024);
    void Start()
    {
        Scaler();
    }

    //����
    void Scaler()
    {
        Vector2 canvasSize = gameObject.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        float screenxyRate = canvasSize.x / canvasSize.y;
        Vector2 bgSize = textureOriginSize;
        float texturexyRate = bgSize.x / bgSize.y;

        RectTransform rt = (RectTransform)transform;
        if (texturexyRate > screenxyRate)
        {
            int newSizeY = Mathf.CeilToInt(canvasSize.y);
            int newSizeX = Mathf.CeilToInt((float)newSizeY / bgSize.y * bgSize.x);
            rt.sizeDelta = new Vector2(newSizeX, newSizeY);
        }
        else
        {
            int newVideoSizeX = Mathf.CeilToInt(canvasSize.x);
            int newVideoSizeY = Mathf.CeilToInt((float)newVideoSizeX / bgSize.x * bgSize.y);
            rt.sizeDelta = new Vector2(newVideoSizeX, newVideoSizeY);
        }
    }

    public void Update()
    {
#if UNITY_EDITOR
        //editorģʽ�²�����
        Scaler();
#endif
    }

}


