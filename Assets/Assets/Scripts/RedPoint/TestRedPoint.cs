using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StudyUnity
{
    public class RedDotTest : MonoBehaviour
    {
        // 定义测试用的 Key (模拟 RKey)
        private const int Root = 1000;
        private const int MailBtn = 2000;       // 邮件入口 (SumCount)
        private const int Mail_Sys = 2001;      // 系统邮件
        private const int Mail_Guild = 2002;    // 公会邮件
        
        private const int ActBtn = 3000;        // 活动入口 (PassThrough)
        private const int Act_Sign = 3001;      // 签到 (可能New, 可能Number)
        private const int Act_Recharge = 3002;  // 充值 (可能Dot)

        private const int BagBtn = 4000;        // 背包入口 (AnyToDot)
        private const int Bag_Item1 = 4001;

        void Start()
        {
            Debug.Log("<color=yellow>=== 开始红点系统测试 ===</color>");

            // Test_BasicAndAnyToDot();
            // Test_SumCount();
            // Test_PassThrough();
            Test_CircularDependency(); // 死循环检测
            // Test_DynamicRemove();      // 动态移除
            
            Debug.Log("<color=yellow>=== 测试结束 ===</color>");
        }

        // 1. 测试基础设置 & AnyToDot (默认模式)
        void Test_BasicAndAnyToDot()
        {
            Debug.Log("--- Test 1: 基础 & AnyToDot ---");
            var mgr = RedDotManager.Instance;

            // 建立树: Root <- BagBtn <- Bag_Item1
            // BagBtn 默认为 AnyToDot
            mgr.Link(BagBtn, Root, EAggregatorMode.AnyToDot);
            mgr.Link(Bag_Item1, BagBtn);

            // A. 初始为空
            Assert(mgr.GetNode(BagBtn).FinalData.Type == ERedDotType.None, "初始应该是 None");

            // B. 设置子节点为数字 5
            mgr.SetData(Bag_Item1, ERedDotType.Number, 5);
            
            // C. 父节点 BagBtn 应该显示 Dot (忽略了数字5，因为是 AnyToDot)
            var result = mgr.GetNode(BagBtn).FinalData;
            Assert(result.Type == ERedDotType.Dot && result.Count == 0, "BagBtn 应该是 Dot (无数字)");

            // D. 清空
            mgr.SetData(Bag_Item1, ERedDotType.None);
            Assert(mgr.GetNode(BagBtn).FinalData.Type == ERedDotType.None, "清空后应该是 None");
        }

        // 2. 测试 SumCount (累加模式 - 邮件)
        void Test_SumCount()
        {
            Debug.Log("--- Test 2: SumCount 累加模式 ---");
            var mgr = RedDotManager.Instance;

            // 建立树: Root <- MailBtn (SumCount) <- [Sys, Guild]
            mgr.Link(MailBtn, Root, EAggregatorMode.SumCount);
            mgr.Link(Mail_Sys, MailBtn);
            mgr.Link(Mail_Guild, MailBtn);

            // 设置 Sys=2, Guild=3
            mgr.SetData(Mail_Sys, ERedDotType.Number, 2);
            mgr.SetData(Mail_Guild, ERedDotType.Number, 3);

            // 验证 MailBtn = 5
            var result = mgr.GetNode(MailBtn).FinalData;
            Assert(result.Type == ERedDotType.Number && result.Count == 5, $"邮件总数应该是 5，实际是: {result.Count}");

            // 验证: 其中一个变成 0
            mgr.SetData(Mail_Sys, ERedDotType.None);
            Assert(mgr.GetNode(MailBtn).FinalData.Count == 3, "邮件总数应该是 3");
        }

        // 3. 测试 PassThrough (优先级模式 - 活动)
        void Test_PassThrough()
        {
            Debug.Log("--- Test 3: PassThrough 优先级模式 ---");
            var mgr = RedDotManager.Instance;

            // 建立树: Root <- ActBtn (PassThrough) <- [Sign, Recharge]
            mgr.Link(ActBtn, Root, EAggregatorMode.PassThrough);
            mgr.Link(Act_Sign, ActBtn);
            mgr.Link(Act_Recharge, ActBtn);

            // 场景 A: 只有普通红点
            mgr.SetData(Act_Recharge, ERedDotType.Dot, 0);
            Assert(mgr.GetNode(ActBtn).FinalData.Type == ERedDotType.Dot, "只有Dot子节点，父节点应为Dot");

            // 场景 B: 出现了数字 (优先级 Number > Dot)
            mgr.SetData(Act_Sign, ERedDotType.Number, 10);
            Assert(mgr.GetNode(ActBtn).FinalData.Type == ERedDotType.Number, "出现了Number，父节点应升级为Number");

            // 场景 C: 出现了 New (优先级 New > Number)
            // 假设充值活动变成 New 了
            mgr.SetData(Act_Recharge, ERedDotType.New, 0);
            Assert(mgr.GetNode(ActBtn).FinalData.Type == ERedDotType.New, "出现了New，父节点应升级为New");

            // 场景 D: New 消失，退回 Number
            mgr.SetData(Act_Recharge, ERedDotType.None);
            Assert(mgr.GetNode(ActBtn).FinalData.Type == ERedDotType.Number, "New消失，父节点应退回Number");
        }

        // 4. 测试死循环检测 (环形依赖)
        void Test_CircularDependency()
        {
            Debug.Log("--- Test 4: 死循环检测 ---");
            var mgr = RedDotManager.Instance;

            // 构造: A -> B -> C
            int A = 100;
            int B = 101;
            int C = 102;

            mgr.Link(B, A);
            mgr.Link(C, B);

            // 尝试: Link(A, C) -> 让 C 成为 A 的父亲
            // 期望: 报错并拒绝
            Debug.Log(">> 准备触发死循环错误日志 (预期内):");
            mgr.Link(A, C); 

            // 验证: A 的父节点依然是 null (或者是之前的状态)，而不应该是 C
            var nodeA = mgr.GetNode(A);
            Assert(nodeA.Parent != mgr.GetNode(C), "死循环防御成功：A 的父亲不应该是 C");
        }

        // 5. 测试动态移除
        void Test_DynamicRemove()
        {
            Debug.Log("--- Test 5: 动态移除 ---");
            var mgr = RedDotManager.Instance;

            int Parent = 5000;
            int Child = 5001;

            mgr.Link(Child, Parent, EAggregatorMode.SumCount);
            mgr.SetData(Child, ERedDotType.Number, 10);

            Assert(mgr.GetNode(Parent).FinalData.Count == 10, "移除前总数是 10");

            // 移除 Child
            mgr.RemoveNode(Child);

            // 验证 Parent 应该自动刷新为 0
            Assert(mgr.GetNode(Parent).FinalData.Count == 0, "移除子节点后，父节点应归零");
            
            // 验证 Child 的引用断开了
            Assert(mgr.GetNode(Parent).Children.Count == 0, "父节点的 Children 列表应为空");
        }

        // 简单的断言辅助函数
        void Assert(bool condition, string msg)
        {
            if (condition)
            {
                Debug.Log($"<color=green>[PASS]</color> {msg}");
            }
            else
            {
                Debug.LogError($"<color=red>[FAIL]</color> {msg}");
            }
        }
    }
}

