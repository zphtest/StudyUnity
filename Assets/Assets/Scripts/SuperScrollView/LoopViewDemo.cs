using SuperScrollView;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopViewDemo : MonoBehaviour
{
    public Button button;
    public LoopListView2 LoopListView;

    // 单个 item 的高度（在 Unity Inspector 中设置）
    public float itemHeight = 100f;

    // 最大可见数量（4.5 个普通 item + 1 个 item2）
    private const float maxVisibleCount = 5.5f;

    // 普通 item 的数量（不包括 item2）
    private int normalItemCount = 0;

    private void Awake()
    {
        // 初始化列表：默认只有 1 个 item（item2）
        LoopListView.InitListView(0, OnItemUpdate);
    }


    void Start()
    {
        LoopListView.SetListItemCount(1, true);
        UpdateContainerHeight();
    }

    /// <summary>
    /// 点击 item2 的 addBtn，在它前面插入新的普通 item
    /// </summary>
    private void OnAddBtnClicked()
    {
        normalItemCount++;
        LoopListView.SetListItemCount(normalItemCount + 1, true);
        UpdateContainerHeight();
    }

    private LoopListViewItem2 OnItemUpdate(LoopListView2 loopListView, int index)
    {
        if (index < 0 || index > 1000) return null;

        // 获取当前总数量
        int totalCount = loopListView.ItemTotalCount;

        // 判断是否是最后一个 item（item2）
        bool isItem2 = (index == totalCount - 1);

        LoopListViewItem2 item;
        TestItem itemInfo;

        if (isItem2)
        {
            // 最后一个位置：使用 item2 预制体
            item = loopListView.NewListViewItem("item2");
            itemInfo = item.GetComponent<TestItem>();
            itemInfo.TxtName.text = "item2 (点击左侧添加)";
            itemInfo.SetAddBtnListener(OnAddBtnClicked);
        }
        else
        {
            // 其他位置：使用普通 item 预制体
            item = loopListView.NewListViewItem("item");
            itemInfo = item.GetComponent<TestItem>();
            itemInfo.SetData(index);
            itemInfo.SetAddBtnListener(null);
        }

        return item;
    }

    /// <summary>
    /// 更新容器高度，使其随着 item 数量自适应
    /// </summary>
    private void UpdateContainerHeight()
    {
        int totalCount = LoopListView.ItemTotalCount;
        float visibleCount = Mathf.Min(totalCount, maxVisibleCount);
        float newHeight = visibleCount * itemHeight;

        // 调整 Viewport 的高度来控制可见区域
        // LoopListView2 的第一个子对象通常是 Viewport
        if (LoopListView.transform.childCount > 0)
        {
            RectTransform viewportRect = LoopListView.transform.GetChild(0) as RectTransform;
            if (viewportRect != null)
            {
                viewportRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            }
        }
    }
}