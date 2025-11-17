using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    public Camera mainCamera;

    void Start()
    {
        // 设置为正交相机
        mainCamera.orthographic = true;

        // 设置相机角度
        mainCamera.transform.rotation = Quaternion.Euler(60, 45, 0);

        // 设置相机的大小，使棋盘显示完整
        mainCamera.orthographicSize = 10;

        // 调整近远裁剪面，确保所有物体都能显示
        mainCamera.nearClipPlane = 0.1f;
        mainCamera.farClipPlane = 1000f;
    }
}
