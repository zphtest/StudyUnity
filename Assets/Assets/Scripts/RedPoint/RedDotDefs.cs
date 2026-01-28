namespace StudyUnity
{
    // 红点类型枚举
    using System;

    /// <summary>
    /// 红点显示类型
    /// </summary>
    public enum ERedDotType
    {
        None = 0,
        Dot = 1,    // 普通红点
        New = 2,    // New 标签 (优先级最高)
        Number = 3  // 数字 (可累加)
    }

    /// <summary>
    /// 父节点聚合子节点的方式
    /// </summary>
    public enum EAggregatorMode
    {
        /// <summary>
        /// 默认：只要有子节点亮，父节点就显示 Dot (不关心具体数字)
        /// </summary>
        AnyToDot, 

        /// <summary>
        /// 穿透/优先模式：
        /// 1. 如果有子节点是 New -> 父节点显示 New
        /// 2. 否则如果有子节点是 Number -> 父节点显示数字总和
        /// 3. 否则如果有子节点是 Dot -> 父节点显示 Dot
        /// </summary>
        PassThrough, 
    
        /// <summary>
        /// 纯计数模式：只累加所有子节点的数字，强制显示为 Number
        /// </summary>
        SumCount
    }

    /// <summary>
    /// 红点数据载荷 (结构体避免GC)
    /// </summary>
    public struct RedDotData : IEquatable<RedDotData>
    {
        public ERedDotType Type;
        public int Count;

        public static readonly RedDotData Empty = new RedDotData { Type = ERedDotType.None, Count = 0 };

        // 实现 IEquatable 接口以提高比较性能
        public bool Equals(RedDotData other)
        {
            return Type == other.Type && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            return obj is RedDotData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, Count);
        }

        public static bool operator ==(RedDotData left, RedDotData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RedDotData left, RedDotData right)
        {
            return !left.Equals(right);
        }
    }
}



