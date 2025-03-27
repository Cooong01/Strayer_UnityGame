using UnityEngine;

/// <summary>
/// ����ˮ��߶Ƚ�������ӿ�����
/// �ѷ�������Ϊ��Ŀ�п�����ˮ����ơ�
/// </summary>
public class CameraWater : MonoBehaviour
{
    public Transform BG; //�ؿ�����

    void Start()
    {
        FitCameraToObject();
    }

    //����ؿ��������Ч���淶Χ���涨������Ŀ��ÿһ�ص�BG���������Ч���淶Χ��������ϷҪ�ز�����BG��
    void FitCameraToObject()
    {

        Camera camera = this.GetComponent<Camera>();
        transform.position = new Vector3(BG.position.x, BG.position.y,transform.position.z);
        float objectWidth = BG.lossyScale.x;
        float objectHeight = BG.lossyScale.y;
        float targetAspect = objectWidth / objectHeight;
        camera.aspect = targetAspect;
        camera.orthographicSize = objectHeight / 2;
    }
}
