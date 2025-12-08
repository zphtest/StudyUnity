using System.Collections.Generic;
using Unity.VisualScripting;

namespace StudyUnity
{
    public class RedDotManager : Singleton<RedDotManager>
    {
        // 扁平化存储所有节点，方便O(1)查找
        private Dictionary<string, RedDotNode> _allNodes = new Dictionary<string, RedDotNode>();
    
        // 根节点
        public RedDotNode Root { get; private set; }

        // 注册节点：构建树结构
        // 路径示例: "Main.Alliance.Tech"
        public RedDotNode RegisterNode(string path)
        {
            if (_allNodes.ContainsKey(path)) return _allNodes[path];

            var node = new RedDotNode { Path = path };
            _allNodes.Add(path, node);

            // 自动寻找或创建父节点
            int lastDotIndex = path.LastIndexOf('.');
            if (lastDotIndex != -1)
            {
                string parentPath = path.Substring(0, lastDotIndex);
                var parentNode = RegisterNode(parentPath); // 递归注册父节点
            
                node.Parent = parentNode;
                parentNode.Children.Add(node);
            }

            return node;
        }

        // 设置叶子节点的值（业务逻辑调用）
        public void SetNodeValue(string path, RedDotType type, int count = 0)
        {
            var node = GetNode(path);
            if (node == null) return; // 容错，或者自动注册

            // 避免重复计算
            if (node.Data.Type == type && node.Data.Count == count) return;

            node.Data.Type = type;
            node.Data.Count = count;
        
            // 触发向上渗透
            node.CheckState(); 
        }
        
        /// <summary>
        /// 移除节点及其所有子节点
        /// </summary>
        public void RemoveNode(string path)
        {
            if (!_allNodes.TryGetValue(path, out var node)) return;

            // 递归移除：先移除所有子孙节点
            // 必须倒序遍历或者使用副本，因为移除过程会改变集合
            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                var child = node.Children[i];
                RemoveNode(child.Path); // 递归调用
            }

            // 从父节点断开引用 (双向清理)
            if (node.Parent != null)
            {
                node.Parent.Children.Remove(node);
                node.Parent = null;
            }

            // 最后：从全局字典中移除自己
            _allNodes.Remove(path);
        }
    
        public RedDotNode GetNode(string path)
        {
            _allNodes.TryGetValue(path, out var node);
            return node;
        }
    }
}
