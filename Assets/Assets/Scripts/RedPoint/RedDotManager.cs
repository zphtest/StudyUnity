using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace StudyUnity
{
    public class RedDotManager : Singleton<RedDotManager>
    {
        private Dictionary<int, RedDotNode> _allNodes = new Dictionary<int, RedDotNode>();

        /// <summary>
        /// 获取或创建节点
        /// </summary>
        public RedDotNode GetNode(int key)
        {
            if (!_allNodes.TryGetValue(key, out var node))
            {
                node = new RedDotNode(key);
                _allNodes.Add(key, node);
            }
            return node;
        }

        /// <summary>
        /// 建立父子关系（带环形检测）
        /// </summary>
        /// <param name="childKey">子节点Key</param>
        /// <param name="parentKey">父节点Key</param>
        /// <param name="mode">父节点的聚合模式</param>
        /// <returns>返回子节点对象，方便链式操作</returns>
        public RedDotNode Link(int childKey, int parentKey, EAggregatorMode mode = EAggregatorMode.AnyToDot)
        {
            // 1. 基本参数检查
            if (childKey == parentKey)
            {
                Debug.LogError($"[RedDot] Self-Cycle detected: Node {childKey} cannot act as its own parent.");
                return GetNode(childKey);
            }

            var child = GetNode(childKey);
            var parent = GetNode(parentKey);

            // 2. 如果关系没变，仅更新模式直接返回
            if (child.Parent == parent)
            {
                if (parent.Aggregator != mode)
                {
                    parent.Aggregator = mode;
                    parent.MarkDirty(); // 模式变了，父节点需要重算
                }
                return child;
            }

            // 3. 【重要】死循环检测：检查 parent 是否是 child 的后代
            if (IsAncestor(child, parent)) // 意思是：child 是 parent 的祖先吗？
            {
                Debug.LogError($"[RedDot] Circular dependency detected: Cannot set {parentKey} as parent of {childKey}, because {childKey} is already an ancestor of {parentKey}.");
                return child;
            }

            // 4. 断开旧连接
            if (child.Parent != null)
            {
                child.Parent.Children.Remove(child);
                child.Parent.MarkDirty(); // 旧父亲也要重算
            }

            // 5. 建立新连接
            child.Parent = parent;
            if (!parent.Children.Contains(child))
            {
                parent.Children.Add(child);
            }

            // 6. 更新父节点设置
            parent.Aggregator = mode;
            parent.MarkDirty(); // 新父亲重算

            return child;
        }

        /// <summary>
        /// 移除节点及其所有子节点 (防止内存泄漏)
        /// </summary>
        public void RemoveNode(int key)
        {
            if (!_allNodes.TryGetValue(key, out var node)) return;

            // 1. 递归移除所有子孙节点
            // 倒序遍历，因为RemoveNode会修改集合
            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                RemoveNode(node.Children[i].Key);
            }

            // 2. 从父节点中断开
            if (node.Parent != null)
            {
                node.Parent.Children.Remove(node);
                node.Parent.MarkDirty(); // 父节点需要刷新状态
                node.Parent = null;
            }

            // 3. 从全局字典移除
            _allNodes.Remove(key);
        }

        /// <summary>
        /// 设置叶子节点数据
        /// </summary>
        public void SetData(int key, ERedDotType type, int count = 0)
        {
            GetNode(key).SetData(type, count);
        }

        // --- 辅助方法 ---

        /// <summary>
        /// 检查 potentialAncestor 是否是 startNode 的祖先
        /// (用于防止 A->B->A 的死循环)
        /// </summary>
        private bool IsAncestor(RedDotNode potentialAncestor, RedDotNode startNode)
        {
            var current = startNode;
            int safetyCount = 0;
            
            while (current != null)
            {
                if (current == potentialAncestor) return true;
                
                current = current.Parent;
                
                // 极端情况下的安全阀，防止链条过长卡死
                if (++safetyCount > 1000) break; 
            }
            return false;
        }
    }
}
