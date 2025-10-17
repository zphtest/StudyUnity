using System.Collections.Generic;
using UnityEngine;

public class UICarouselFlat : MonoBehaviour
{
    [Header("轮播项（拖入子物体）")]
    public List<RectTransform> items = new List<RectTransform>();

    [Header("参数设置")]
    public float spacing = 100f;      // 卡片之间的间距
    public float scaleFactor = 1.0f;  // 中间卡片放大倍数
    public float moveSpeed = 10f;     // 平滑移动速度

    private float currentOffset = 0f;
    private float targetOffset = 0f;
    
    private float lastOffset = 0f;
    
    void Update()
    {
        currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * moveSpeed);
        ArrangeItems();
    }

    private void ArrangeItems()
    {
        if (items.Count == 0) return;
        
        int count = items.Count;
        
        int maxScaleIndex = 0;
        float maxScale = 0;

        if (lastOffset != 0 && Mathf.Approximately(lastOffset, currentOffset))
        {
            return;
        }
        lastOffset = currentOffset;
        
        for (int i = 0; i < count; i++)
        {
            // 偏移量（决定位置）
            float indexOffset = (i * spacing) - currentOffset;
        
            // 无限循环（可选）
            float loopedX = Mathf.Repeat(indexOffset + spacing * count / 2, spacing * count) - spacing * count / 2;
        
            // 距离中心
            float distanceToCenter = Mathf.Abs(loopedX);
        
            // 缩放（中间大，两边小）
            float t = Mathf.InverseLerp(spacing, 0, distanceToCenter);
            float scale = Mathf.Lerp(0.8f, scaleFactor, t);
        
            // 位置（X 上重叠，Y 保持一致）
            items[i].anchoredPosition = new Vector2(loopedX, 0f);
            items[i].localScale = Vector3.one * scale;

            if (scale > maxScale)
            {
                maxScaleIndex = i;
                maxScale = scale;
            }
            // 设置层级
            float distance = Mathf.Abs(i - maxScaleIndex);
            int siblingIndex = count - 1 - Mathf.RoundToInt(distance); // 距离中心越近越大
            items[i].SetSiblingIndex(siblingIndex);
            
            // 设置颜色变暗
            UnityEngine.UI.Image img = items[i].GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                // 中间卡片 alpha=1，两侧 alpha 0.5 ~ 0.8
                float alpha = Mathf.Lerp(0.5f, 1f, t);
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
        }
    }

    /// <summary>
    /// 拖拽时调用
    /// </summary>
    public void Move(float deltaX)
    {
        targetOffset += deltaX;
    }

    /// <summary>
    /// 松手时对齐到最近的卡片
    /// </summary>
    public void SnapToNearest()
    {
        if (items.Count == 0) return;
        float nearest = Mathf.Round(targetOffset / spacing) * spacing;
        targetOffset = nearest;

        Debug.Log("targetOffset：" + targetOffset);
    }

    /// <summary>
    /// 自动切换到下一个
    /// </summary>
    public void Next()
    {
        targetOffset += spacing;
    }

    /// <summary>
    /// 自动切换到上一个
    /// </summary>
    public void Prev()
    {
        targetOffset -= spacing;
    }
}
