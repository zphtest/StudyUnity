using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StudyUnity
{
    public class ActivityModel
    {
        // public void OnUpdateActivities(List<ActivityData> list)
        // {
        //     var mgr = RedDotManager.Instance;
        //
        //     // 1. 获取父节点
        //     var parentNode = mgr.GetNode(RKey.Main_Act);
        //
        //     // 2. 动态节点更新三部曲
        //     // A. 标记
        //     foreach (var child in parentNode.Children) child.IsVisited = false;
        //
        //     // B. 更新 / 新增
        //     foreach (var act in list)
        //     {
        //         int childKey = RKey.GetActivityKey(act.Id);
        //     
        //         // 动态建立父子关系 (Link 内部会自动处理去重)
        //         // 这里父节点的 Mode 已经在 InitTree 里配过了，不需要重复配
        //         mgr.Link(childKey, RKey.Main_Act); 
        //
        //         var node = mgr.GetNode(childKey);
        //         node.IsVisited = true;
        //
        //         // 计算具体状态
        //         if (act.IsNew)
        //         {
        //             // 显示 NEW
        //             node.SetData(ERedDotType.New, 0);
        //         }
        //         else if (act.CanClaimReward)
        //         {
        //             // 显示数字或普通红点
        //             node.SetData(ERedDotType.Number, act.RewardCount);
        //         }
        //         else
        //         {
        //             node.SetData(ERedDotType.None, 0);
        //         }
        //     }
        //
        //     // C. 清理 (配合之前的 RemoveNode 逻辑)
        //     for (int i = parentNode.Children.Count - 1; i >= 0; i--)
        //     {
        //         var child = parentNode.Children[i];
        //         if (!child.IsVisited)
        //         {
        //             mgr.RemoveNode(child.Key);
        //         }
        //     }
        //
        //     // 强制父节点刷新一次聚合状态（因为移除了子节点可能导致状态变了）
        //     // SetData 内部其实触发了 MarkDirty，但如果全是移除操作，可能需要手动触发
        //     // (在上面的 RemoveNode 实现里已经加了 Parent.MarkDirty，所以这里不用管)
        // }
    }
}