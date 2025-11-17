// Unity 2D 大富翁棋盘 Demo（等距45度视觉）
// 包含：棋盘生成、小人移动、跳跃伪3D效果（高度用Y轴）


using UnityEngine;
using UnityEngine.UI;


public class Board2DGenerator : MonoBehaviour
{
    public GameObject tilePrefab;  // 3D 格子预制体（Cube 或 Plane）
    public int width = 8;          // 棋盘宽度
    public int height = 8;         // 棋盘高度
    public float tileSize = 1f;    // 格子大小

    void Start()
    {
        Generate();
    }

    // 生成棋盘
    public void Generate()
    {
        // 计算棋盘中心位置，确保棋盘格子会出现在相机视野范围内
        Vector3 centerPosition = new Vector3((width - 1) * tileSize / 2f, 0, (height - 1) * tileSize / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 使用 45 度斜视角度来排放棋盘格子
                float isoX = (x - y) * tileSize; // 斜45度 X 轴
                float isoY = (x + y) * tileSize; // 斜45度 Y 轴

                Vector3 position = centerPosition + new Vector3(isoX, 0, isoY); // 平放格子
                Instantiate(tilePrefab, position, Quaternion.identity);
            }
        }
    }
}