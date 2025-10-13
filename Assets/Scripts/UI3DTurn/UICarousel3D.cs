using System.Collections.Generic;
using UnityEngine;

public class UICarousel3D : MonoBehaviour
{
    [Header("轮播项（手动拖入子物体）")]
    public List<Transform> items = new List<Transform>();

    [Header("参数设置")]
    public float radius = 500f;          // 半径，决定卡片摆放远近
    public float rotationSpeed = 50f;    // 自动旋转速度（可为0，完全手动拖动）
    public float scaleFactor = 1.5f;     // 中间卡片的放大倍数
    public float centerOffset = 0f;      // 调整Z轴深度，让中间更突出

    private float angle = 0f;

    void Start()
    {
        ArrangeItems();
    }

    void Update()
    {
        // 如果要自动旋转，开启这一行
        // angle += rotationSpeed * Time.deltaTime;
        ArrangeItems();
    }

    /// <summary>
    /// 按圆形摆放卡片
    /// </summary>
    private void ArrangeItems()
    {
        if (items.Count == 0) return;

        float angleStep = 360f / items.Count;

        for (int i = 0; i < items.Count; i++)
        {
            float currentAngle = angle + i * angleStep;
            float rad = currentAngle * Mathf.Deg2Rad;

            // 计算位置（X 轴圆周，Z 轴深度）
            Vector3 pos = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
            items[i].localPosition = pos;

            // 面向摄像机（如果需要始终正对屏幕）
            items[i].LookAt(Camera.main.transform);

            // 根据Z值缩放（前方的放大，后方缩小）
            float scale = Mathf.Lerp(1f, scaleFactor, Mathf.InverseLerp(-radius, radius, pos.z + centerOffset));
            items[i].localScale = Vector3.one * scale;
        }
    }

    /// <summary>
    /// 外部手动旋转（比如拖拽时调用）
    /// </summary>
    public void Rotate(float deltaAngle)
    {
        angle += deltaAngle;
    }
}
