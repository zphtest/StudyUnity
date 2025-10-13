using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestItem : MonoBehaviour
{
    public Image ImgIcon;
    public TextMeshProUGUI TxtName;

    public void SetData(int _index)
    {
        TxtName.text = $"测试：<color=#F55F55>{_index}</color>";
    }
}