using SuperScrollView;
using UnityEngine;

public class LoopListViewExample : MonoBehaviour
{
    [SerializeField] private LoopListView2 m_view = null;

    private void Start()
    {
        m_view.InitListView(100, OnUpdate);
    }

    private void LateUpdate()
    {
        m_view.UpdateAllShownItemSnapData();

        int count = m_view.ShownItemCount;

        for (int i = 0; i < count; ++i)
        {
            var itemObj = m_view.GetShownItemByIndex(i);
            var itemUI = itemObj.GetComponent<LoopTestItem>();
            Debug.Log($"center: {itemObj.DistanceWithViewPortSnapCenter}");
            var amount = 1 - Mathf.Abs(itemObj.DistanceWithViewPortSnapCenter) / 720f;
            var scale = Mathf.Clamp(amount, 0.4f, 1);

            itemUI.SetScale(scale);
        }
    }

    private LoopListViewItem2 OnUpdate(LoopListView2 view, int index)
    {
        if (index < 0 || index > 100) return null;

        var itemObj = view.NewListViewItem("item");
        var itemUI = itemObj.GetComponent<LoopTestItem>();

        itemUI.SetData(index);
        return itemObj;
    }
}