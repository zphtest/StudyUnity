using System.Collections;
using System.Collections.Generic;
using UITest;
using UnityEngine;
/// <summary>
/// 注意，这个默认不要在面板勾选Toggle得IsOn，不然默认不会让他选中
/// </summary>
public class TestToogle : MonoBehaviour
{
    [SerializeField] RectTransform _rootUIToggleGroup = null;
    
    private UIToggleGroup uiToggleGroup;
    private CommonPage curPage;
    
    // Start is called before the first frame update
    void Start()
    {
        uiToggleGroup = _rootUIToggleGroup.GetComponent<UIToggleGroup>();
        curPage = CommonPage.None;
        uiToggleGroup.Init(OnClickToggle);
        uiToggleGroup.SetSelect(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnClickToggle(int curIndex, int lastIndex = 0)
    {
        if (curIndex == (int)curPage)
            return;
        curPage = (CommonPage)curIndex;
        Debug.Log($"curPage:{curPage}, curIndex:{curIndex}");
        // uiToggleGroup.SetSelect(curIndex);
        if (curIndex == 0)
        {
            
        }
        else
        {
            
        }
    }
}
