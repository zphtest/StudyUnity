using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestItem : MonoBehaviour
{
    public Image ImgIcon;
    public TextMeshProUGUI TxtName;
    public Button AddBtn;

    public void SetData(int _index)
    {
        TxtName.text = $"测试：<color=#F55F55>{_index}</color>";
    }

    // 设置添加按钮的点击事件
    public void SetAddBtnListener(UnityEngine.Events.UnityAction action)
    {
        if (AddBtn != null)
        {
            AddBtn.onClick.RemoveAllListeners();
            AddBtn.onClick.AddListener(action);
        }
    }
}