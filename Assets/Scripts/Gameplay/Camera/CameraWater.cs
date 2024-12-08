using UnityEngine;

public class CameraWater : MonoBehaviour
{
    public Transform BG; //关卡背景

    void Start()
    {
        FitCameraToObject();
    }

    //拍摄关卡的最大有效画面范围（规定：本项目里每一关的BG即是最大有效画面范围，放置游戏要素不超过BG）
    void FitCameraToObject()
    {

        Camera camera = this.GetComponent<Camera>();
        transform.position = new Vector3(BG.position.x, BG.position.y,transform.position.z);
        float objectWidth = BG.lossyScale.x;
        float objectHeight = BG.lossyScale.y;
        float targetAspect = objectWidth / objectHeight;
        // 设置相机的宽高比
        camera.aspect = targetAspect;

        camera.orthographicSize = objectHeight / 2;



    }
}
