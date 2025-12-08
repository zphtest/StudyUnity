
namespace StudyUnity
{
    public enum RedDotType
    {
        None = 0,
        Dot = 1,    // 普通红点
        New = 2,    // New图标
        Number = 3  // 数字
    }

    // 节点数据
    public class RedDotData
    {
        public RedDotType Type;
        public int Count; // 仅当Type为Number时有效
    
        // 简单的合并规则：通常数字优先，其次是New，最后是普通点
        // 这里需要根据业务定制合并逻辑
    }
}


