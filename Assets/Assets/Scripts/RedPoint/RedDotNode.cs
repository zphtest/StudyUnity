using System;
using System.Collections.Generic;


namespace StudyUnity
{
    public class RedDotNode
    {
        public string Path; // 节点路径，如 "Main/Alliance/Tech"
        public int PathHash;
        public RedDotData Data = new RedDotData();
    
        public RedDotNode Parent;
        public List<RedDotNode> Children = new List<RedDotNode>();
    
        // 这里的回调专门给UI组件注册，当数据变动时通知UI刷新
        public Action<RedDotData> OnChange; 
        
        // 标记位：用于差量更新
        public bool IsVisited = false; 

        // 核心逻辑：子节点变化触发父节点重新计算
        public void CheckState()
        {
            // 如果是叶子节点，状态由具体的业务逻辑Set进来，不需要计算
            if (Children.Count == 0) 
            {
                NotifyChange();
                return;
            }

            // 如果是中间节点，状态由所有子节点聚合而来
            int totalCount = 0;
            bool hasNew = false;
            bool hasDot = false;

            foreach (var child in Children)
            {
                if (child.Data.Type == RedDotType.Number) totalCount += child.Data.Count;
                if (child.Data.Type == RedDotType.New) hasNew = true;
                if (child.Data.Type == RedDotType.Dot) hasDot = true;
            }

            // 聚合规则（根据你的需求定制）
            if (hasNew)
            {
                Data.Type = RedDotType.New;
            }
            else if (totalCount > 0)
            {
                Data.Type = RedDotType.Number;
                Data.Count = totalCount;
            }
            else if (hasDot)
            {
                Data.Type = RedDotType.Dot;
            }
            else
            {
                Data.Type = RedDotType.None;
            }

            NotifyChange();
        }

        private void NotifyChange()
        {
            // 1. 通知绑定的UI刷新
            OnChange?.Invoke(Data);
        
            // 2. 递归通知父节点检查状态 (关键：层层渗透)
            Parent?.CheckState();
        }
        
        // 准备更新，把所有子节点标记为“未访问”
        public void BeginUpdateChildren()
        {
            foreach (var child in Children)
            {
                child.IsVisited = false;
            }
        }
        
        public void EndUpdateChildren()
        {
            // 反向遍历以便安全移除
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                var child = Children[i];
                if (!child.IsVisited)
                {
                    // 这里执行真正的移除逻辑
                    // 1. 从管理器的全局字典里移除 (防止下次 Register 又找回来)
                    RedDotManager.Instance.RemoveNode(child.Path); 
            
                    // 2. 从当前子节点列表移除
                    Children.RemoveAt(i);
            
                    // 3. 断开引用
                    child.Parent = null;
                }
            }
    
            // 移除完后，重新聚合一次状态，确保父节点状态正确
            CheckState();
        }
    }
}

