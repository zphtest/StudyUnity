using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoopTestItem : MonoBehaviour
{
    public Transform RootTrans;
    public Image ImgIcon;
    public TextMeshProUGUI TxtName;

    public void SetData(int _index)
    {
        TxtName.text = $"Test：<color=#F55F55>{_index}</color>";
    }

    public void SetScale(float scale)
    {
        RootTrans.GetComponent<CanvasGroup>().alpha = scale;
        RootTrans.transform.localScale = new Vector3(scale, scale, 1);
    }
}