using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    
    [Header("地图设置")]
    public static int mapWidth = 8;   // 地图宽度
    public static int mapHeight = 8;  // 地图高度
    public float gridSizeX = 100f; // ⭐格子间距（UI单位）
    public float gridSizeY = 100f; // ⭐格子间距（UI单位）
    public int[] mapInfo = new int[mapWidth * mapHeight];
    public int[] mapIndexInfo = new int[mapWidth * mapHeight];
    public int[] layerIndexInfo =  new int[mapWidth * mapHeight];
    
    [Header("预制体")]
    public GameObject gridPrefab;
    public Transform gridContainer;
    
    [Header("UI设置")]
    public Canvas canvas;
    
    public Dictionary<int, GridNode> gridNodes = new Dictionary<int, GridNode>();
    public Dictionary<int, GridNode> layerGridNodes = new Dictionary<int, GridNode>();
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        // ⭐地图数据：1=显示格子，0=不显示
        // 从上到下，从左到右（Y=7到Y=0，X=0到X=7）
        mapInfo = new int[]
        {
            // Y=7 (最上面一行) - 索引0-7
            0, 0, 0, 1, 1, 1, 0, 0,
            
            // Y=6 - 索引8-15
            0, 0, 1, 1, 0, 1, 1, 0,
            
            // Y=5 - 索引16-23
            1, 1, 1, 0, 0, 0, 1, 0,
            
            // Y=4 - 索引24-31 (中心位置4,4是奖励格)
            1, 1, 0, 0, 0, 0, 1, 1,
            
            // Y=3 - 索引32-39
            0, 1, 0, 0, 0, 0, 0, 1,
            
            // Y=2 - 索引48-55
            0, 1, 1, 1, 0, 0, 1, 1,
            
            // Y=1(最下面一行) - 索引56-63
            0, 0, 0, 1, 1, 1, 1, 0
        };
        
        mapIndexInfo = new int[]
        {
            // Y=7 (最上面一行) - 索引0-7
            0, 0, 0, 5, 6, 7, 0, 0,
            
            // Y=6 - 索引8-15
            0, 0, 3, 4, 0, 8, 9, 0,
            
            // Y=5 - 索引16-23
            0, 1, 2, 0, 0, 0, 10, 0,
            
            // Y=4 - 索引24-31 (中心位置4,4是奖励格)
            25, 24, 0, 0, 0, 0, 11, 12,
            
            // Y=3 - 索引32-39
            0, 23, 0, 0, 0, 0, 0, 13,
            
            // Y=2 - 索引48-55
            0, 22, 21, 20, 0, 0, 15, 14,
            
            // Y=1(最下面一行) - 索引56-63
            0, 0, 0, 19, 18, 17, 16, 0
        };
        
        layerIndexInfo = new int[]
        {
            // Y=7 (最上面一行) - 索引0-7
            0, 0, 0, 3, 2, 1, 0, 0,
            
            // Y=6 - 索引8-15
            0, 0, 7, 6, 0, 5, 4, 0,
            
            // Y=5 - 索引16-23
            11, 10, 9, 0, 0, 0, 8, 0,
            
            // Y=4 - 索引24-31 (中心位置4,4是奖励格)
            15, 14, 0, 0, 0, 0, 13, 12,
            
            // Y=3 - 索引32-39
            0, 17, 0, 0, 0, 0, 0, 16,
            
            // Y=2 - 索引48-55
            0, 22, 21, 20, 0, 0, 19, 18,
            
            // Y=1(最下面一行) - 索引56-63
            0, 0, 0, 26, 25, 24, 23, 0
        };
        
        
        
        GenerateGridMap();
    }
    
    void GenerateGridMap()
    {
        gridNodes.Clear();
        
        // 清空旧格子
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
        
        int nodeIndex = 0;
        int arrayIndex = 0; // 一维数组索引
        
        float centerX = (mapWidth - 1) * 0.5f;
        float centerY = (mapHeight - 1) * 0.5f;
        
        // ⭐从上到下（Y从大到小 = 从远到近）
        for (int y = mapHeight - 1; y >= 1; y--)
        {
            // ⭐从左到右
            for (int x = 0; x < mapWidth; x++)
            {
                // ⭐检查地图数据，如果为0则跳过不创建
                if (mapInfo[arrayIndex] == 0)
                {
                    arrayIndex++;
                    continue;
                }
                
                // 创建格子UI
                GameObject gridObj = Instantiate(gridPrefab, gridContainer);
                RectTransform rectTransform = gridObj.GetComponent<RectTransform>();
                
                // ⭐居中计算
                float dx = x - centerX;
                float dy = y - centerY;
                
                // float posX = (x - y) * gridSizeX * 0.5f;
                // float posY = (x + y) * gridSizeY * 0.5f;
                
                // ⭐你的公式 + Y轴取反（向上旋转）
                float posX = (dx + dy) * gridSizeX * 0.52f;
                float posY = (dy - dx) * gridSizeY * 0.45f;
                
                rectTransform.anchoredPosition = new Vector2(posX, posY);
                
                // 初始化GridNode
                GridNode gridNode = gridObj.GetComponent<GridNode>();
                if (gridNode == null)
                {
                    gridNode = gridObj.AddComponent<GridNode>();
                }
                
                gridNode.nodeIndex = nodeIndex;
                gridNode.gridX = x;
                gridNode.gridY = y;
                gridNode.Initialize();
                
                gridNodes.Add(mapIndexInfo[arrayIndex], gridNode);
                layerGridNodes.Add(layerIndexInfo[arrayIndex], gridNode);
                gridObj.name = "GridNode_" + layerIndexInfo[arrayIndex] + arrayIndex;
                nodeIndex++;
                arrayIndex++;
            }
        }

        for (int i = 1; i >=26; i--)
        {
            if (layerGridNodes.ContainsKey(i))
            {
                var go = layerGridNodes[i];
                go?.gameObject.transform.SetAsLastSibling();
            }
        }
        
        Debug.Log($"✅ 生成了 {gridNodes.Count} 个格子（地图大小 {mapWidth}x{mapHeight}）");
    }
    
    // 根据网格坐标获取格子
    public GridNode GetGridNode(int x, int y)
    {
        foreach (var node in gridNodes)
        {
            if (node.Value.gridX == x && node.Value.gridY == y)
                return node.Value;
        }
        return null;
    }
    
    // 根据索引获取网格坐标
    public Vector2Int GetGridCoords(int index)
    {
        if (index < 0 || index >= gridNodes.Count)
            return Vector2Int.zero;
        
        GridNode node = gridNodes[index];
        return new Vector2Int(node.gridX, node.gridY);
    }
    
    // ⭐检查某个位置是否有格子
    public bool HasGridAt(int x, int y)
    {
        if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
            return false;
        
        int arrayIndex = (mapHeight - 1 - y) * mapWidth + x;
        return mapInfo[arrayIndex] == 1;
    }
}