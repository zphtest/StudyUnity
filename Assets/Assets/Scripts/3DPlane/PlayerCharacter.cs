using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class PlayerCharacter : MonoBehaviour
{
    public int currentPosition = 0;
    
    [Header("移动设置")]
    public float moveSpeed = 5f;          // 移动速度
    public float jumpHeight = 0.8f;       // 跳跃高度
    public float rotationSpeed = 15f;     // 旋转速度
    public float hoverHeight = 0.2f;        // 悬浮高度（调整这个值！）
    
    [Header("调试")]
    public bool showDebug = true;
    
    private bool isMoving = false;
    
    void Start()
    {
        // 延迟初始化，等地图生成完成
        StartCoroutine(DelayedStart());
    }
    
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.2f);
        
        if (MapManager.Instance != null && MapManager.Instance.gridNodes.Count > 0)
        {
            SetPositionImmediate(0);
            Debug.Log("角色初始化完成，位置：" + transform.position);
        }
        else
        {
            Debug.LogError("地图未生成！");
        }
    }
    
    // 立即设置位置（不动画）
    void SetPositionImmediate(int gridIndex)
    {
        if (gridIndex < 0 || gridIndex >= MapManager.Instance.gridNodes.Count)
            return;
        
        currentPosition = gridIndex;
        GridNode grid = MapManager.Instance.gridNodes[gridIndex];
        
        Vector3 targetPos = grid.GetWorldPosition();
        targetPos.y = targetPos.y + 1.0f;
        transform.position = targetPos;
        
        if (showDebug)
            Debug.Log($"角色移动到格子 {gridIndex}，世界坐标：{targetPos}");
    }
    
    // 移动N步（逐格跳跃）⭐⭐⭐
    public void MoveSteps(int steps)
    {
        if (!isMoving && MapManager.Instance.gridNodes.Count > 0)
        {
            StartCoroutine(MoveStepsCoroutine(steps));
        }
    }
    
    IEnumerator MoveStepsCoroutine(int steps)
    {
        isMoving = true;
        
        // 清除所有高亮
        foreach (var item in MapManager.Instance.gridNodes)
            item.Value.Highlight(false);
        
        Debug.Log($"开始移动 {steps} 步，当前位置：{currentPosition}");
        
        // ⭐关键：一格一格地跳
        for (int i = 0; i < steps; i++)
        {
            // 计算下一个格子索引
            int nextIndex = (currentPosition + 1) % MapManager.Instance.gridNodes.Count;
            
            Debug.Log($"第 {i + 1} 步：从格子 {currentPosition} 跳到 {nextIndex}");
            
            // 跳到下一格
            yield return StartCoroutine(JumpToGrid(nextIndex));
            
            // 更新当前位置
            currentPosition = nextIndex;
            
            // 短暂停留
            yield return new WaitForSeconds(0.05f);
        }
        
        // 高亮最终位置
        MapManager.Instance.gridNodes[currentPosition].Highlight(true);
        
        Debug.Log($"移动完成！最终位置：{currentPosition}");
        
        isMoving = false;
    }
    
    // 跳到指定格子
    IEnumerator JumpToGrid(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= MapManager.Instance.gridNodes.Count)
            yield break;
        
        GridNode targetGrid = MapManager.Instance.gridNodes[targetIndex];
        Vector3 targetPos = targetGrid.GetWorldPosition();
        targetPos.y = targetPos.y + 1.0f;
        
        Vector3 startPos = transform.position;
        
        // 1. 先转向（快速）
        yield return StartCoroutine(RotateTowards(targetPos));
        
        // 2. 跳跃移动
        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / moveSpeed;
        duration = Mathf.Clamp(duration, 0.1f, 0.4f); // 限制时长
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // 水平移动（线性插值）
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            
            // 垂直跳跃（抛物线）
            float jumpOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            currentPos.y = hoverHeight + jumpOffset;
            
            transform.position = currentPos;
            
            yield return null;
        }
        
        // 确保精准到达
        // targetPos.y = hoverHeight;
        transform.position = targetPos;
    }
    
    // 转向目标
    IEnumerator RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position);
        direction.y = 0; // 只水平旋转
        
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotDuration = 0.1f;
            float elapsed = 0f;
            
            Quaternion startRotation = transform.rotation;
            
            while (elapsed < rotDuration)
            {
                elapsed += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / rotDuration);
                yield return null;
            }
            
            transform.rotation = targetRotation;
        }
    }
    
    // 调试：绘制目标点
    void OnDrawGizmos()
    {
        if (!showDebug || !Application.isPlaying) return;
        if (MapManager.Instance == null || MapManager.Instance.gridNodes.Count == 0) return;
        
        // 绘制所有格子的锚点
        Gizmos.color = Color.cyan;
        foreach (var item in MapManager.Instance.gridNodes)
        {
            Vector3 pos = item.Value.GetWorldPosition();
            // pos.y = hoverHeight;
            Gizmos.DrawWireSphere(pos, 0.3f);
        }
        
        // 绘制当前位置
        Gizmos.color = Color.red;
        if (currentPosition < MapManager.Instance.gridNodes.Count)
        {
            Vector3 currentPos = MapManager.Instance.gridNodes[currentPosition].GetWorldPosition();
            // currentPos.y = hoverHeight;
            Gizmos.DrawSphere(currentPos, 0.4f);
        }
    }
}