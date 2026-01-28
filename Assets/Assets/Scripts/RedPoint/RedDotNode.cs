using System;
using System.Collections.Generic;
using UnityEngine;

namespace StudyUnity
{
    public class RedDotNode
    {
        public int Key { get; private set; }
        public RedDotNode Parent { get; set; }
        public List<RedDotNode> Children { get; } = new List<RedDotNode>();

        // 节点本身的原始数据（通常叶子节点才有值）
        private RedDotData _selfData = RedDotData.Empty;

        // 经过计算后的最终数据（供 UI 显示）
        public RedDotData FinalData { get; private set; } = RedDotData.Empty;

        // 聚合模式
        public EAggregatorMode Aggregator { get; set; } = EAggregatorMode.AnyToDot;

        // 辅助字段：用于动态遍历时的标记
        public bool IsVisited;

        // UI 监听事件
        public event Action<RedDotData> OnChange;

        public RedDotNode(int key)
        {
            Key = key;
        }

        /// <summary>
        /// 设置该节点自身的红点数据
        /// </summary>
        public void SetData(ERedDotType type, int count)
        {
            var newData = new RedDotData { Type = type, Count = count };
            if (_selfData != newData)
            {
                _selfData = newData;
                MarkDirty(); // 数据变了，标记脏更新
            }
        }

        /// <summary>
        /// 标记脏，重新计算并通知父节点
        /// </summary>
        public void MarkDirty()
        {
            var oldFinal = FinalData;

            CalculateFinalData();

            // 如果计算结果变了，通知 UI 并向上传递
            if (oldFinal != FinalData)
            {
                OnChange?.Invoke(FinalData);
                Parent?.MarkDirty();
            }
        }

        /// <summary>
        /// 核心算法：根据子节点计算 FinalData
        /// </summary>
        private void CalculateFinalData()
        {
            // 1. 如果是叶子节点，FinalData 就是 SelfData
            if (Children.Count == 0)
            {
                FinalData = _selfData;
                return;
            }

            // 2. 如果是非叶子节点，根据 AggregatorMode 聚合子节点数据
            // 注意：非叶子节点的 _selfData 通常会被忽略

            if (Aggregator == EAggregatorMode.SumCount)
            {
                CalculateSumCount();
            }
            else if (Aggregator == EAggregatorMode.PassThrough)
            {
                CalculatePassThrough();
            }
            else // Default: AnyToDot
            {
                CalculateAnyToDot();
            }
        }

        // --- 具体聚合策略 ---

        private void CalculateAnyToDot()
        {
            bool hasAny = false;
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].FinalData.Type != ERedDotType.None)
                {
                    hasAny = true;
                    break;
                }
            }

            FinalData = hasAny
                ? new RedDotData { Type = ERedDotType.Dot, Count = 0 }
                : RedDotData.Empty;
        }

        private void CalculateSumCount()
        {
            int total = 0;
            bool hasAny = false;

            for (int i = 0; i < Children.Count; i++)
            {
                var data = Children[i].FinalData;
                if (data.Type != ERedDotType.None)
                {
                    hasAny = true;
                    // 如果子节点有数字则加数字，否则（如只有小红点）算1个
                    total += (data.Count > 0 ? data.Count : 1);
                }
            }

            FinalData = hasAny
                ? new RedDotData { Type = ERedDotType.Number, Count = total }
                : RedDotData.Empty;
        }

        private void CalculatePassThrough()
        {
            bool hasNew = false;
            bool hasNumber = false;
            bool hasDot = false;
            int totalNum = 0;

            for (int i = 0; i < Children.Count; i++)
            {
                var data = Children[i].FinalData;
                switch (data.Type)
                {
                    case ERedDotType.New:
                        hasNew = true;
                        break;
                    case ERedDotType.Number:
                        hasNumber = true;
                        totalNum += data.Count;
                        break;
                    case ERedDotType.Dot:
                        hasDot = true;
                        break;
                }
            }

            // 优先级：New > Number > Dot > None
            if (hasNew)
            {
                FinalData = new RedDotData { Type = ERedDotType.New, Count = 0 };
            }
            else if (hasNumber)
            {
                FinalData = new RedDotData { Type = ERedDotType.Number, Count = totalNum };
            }
            else if (hasDot)
            {
                FinalData = new RedDotData { Type = ERedDotType.Dot, Count = 0 };
            }
            else
            {
                FinalData = RedDotData.Empty;
            }
        }
    }
}