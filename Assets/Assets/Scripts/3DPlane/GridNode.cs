using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridNode : MonoBehaviour
{
    public int nodeIndex;
    public int gridX; // ⭐新增：网格X坐标
    public int gridY; // ⭐新增：网格Y坐标
    
    private Image background;
    private TextMeshProUGUI nameText;
    
    [HideInInspector]
    public Transform worldAnchor;
    
    private Color normalColor = new Color(0.8f, 0.8f, 0.8f);
    private Color highlightColor = new Color(1f, 1f, 0f);
    
    private RectTransform rectTransform;
    
    public void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // 设置Image
        background = GetComponent<Image>();
        if (background == null)
        {
            background = gameObject.AddComponent<Image>();
        }
        background.color = normalColor;
        
        // 设置Text显示坐标
        nameText = GetComponentInChildren<TextMeshProUGUI>();
        if (nameText == null)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(transform);
            nameText = textObj.AddComponent<TextMeshProUGUI>();
            nameText.fontSize = 20;
            nameText.color = Color.black;
            
            RectTransform textRect = nameText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
        
        // ⭐显示坐标或索引
        nameText.text = $"{gridX},{gridY}";
        // 或者只显示索引: nameText.text = nodeIndex.ToString();
        
        CreateWorldAnchor();
    }
    
    void CreateWorldAnchor()
    {
        GameObject anchorObj = new GameObject($"Anchor_{gridX}_{gridY}");
        worldAnchor = anchorObj.transform;
        worldAnchor.SetParent(null);
        
        UpdateAnchorPosition();
    }
    
    void LateUpdate()
    {
        if (worldAnchor != null)
        {
            UpdateAnchorPosition();
        }
    }
    
    void UpdateAnchorPosition()
    {
        if (rectTransform == null) return;
        
        // ⭐直接使用UI的世界坐标
        worldAnchor.position = rectTransform.position;
    }
    
    public void Highlight(bool on)
    {
        if (background != null)
            background.color = on ? highlightColor : normalColor;
    }
    
    public Vector3 GetWorldPosition()
    {
        if (worldAnchor != null)
            return worldAnchor.position;
        
        return rectTransform.position;
    }
    
    void OnDestroy()
    {
        if (worldAnchor != null)
            Destroy(worldAnchor.gameObject);
    }
}