using SuperScrollView;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopViewDemo : MonoBehaviour
{
    public Button button;
    public LoopListView2 LoopListView;

    private void Awake()
    {
        button.onClick.AddListener(() => 
        {
            StartTest();
        });
        //初始化循环列表
        LoopListView.InitListView(0, refreshListInfo);
    }

    private void StartTest()
    {
        //刷新循环列表
        LoopListView.SetListItemCount(1000, true);
    }

    private LoopListViewItem2 refreshListInfo(LoopListView2 loopListView, int index)
    {
        if (index < 0 || index > 1000) return null;
        
        
        if (index % 2 == 0)
        {
            LoopListViewItem2 item = loopListView.NewListViewItem("item");
            TestItem _itemInfo = item.GetComponent<TestItem>();
        
            _itemInfo.SetData(index);
            return item;
        }
        else
        {
            LoopListViewItem2 item = loopListView.NewListViewItem("item2");
            TestItem _itemInfo = item.GetComponent<TestItem>();
        
            _itemInfo.SetData(index);
            return item;
        }


        
    }
}