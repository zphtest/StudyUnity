using UnityEngine;
using System;

namespace StudyUnity.Notification
{
    /// <summary>
    /// 通知系统自动初始化脚本
    /// 在运行时自动初始化并测试通知功能
    /// </summary>
    public static class NotificationInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Debug.Log("=== 通知系统自动初始化 ===");

            // 延迟一帧，确保 NotificationManager 已初始化
            MonoBehaviour host = new GameObject("NotificationAutoTest").AddComponent<NotificationAutoTest>();
            MonoBehaviour.DontDestroyOnLoad(host.gameObject);
        }

        private class NotificationAutoTest : MonoBehaviour
        {
            private void Start()
            {
                RunNotificationTest();
            }

            private void RunNotificationTest()
            {
                Debug.Log("[NotificationInitializer] 开始测试通知系统...");
                Debug.Log($"当前时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Debug.Log($"当前平台: {Application.platform}");

#if UNITY_ANDROID && !UNITY_EDITOR
                Debug.Log("使用 Android 原生通知系统");
#else
                Debug.Log("使用 Editor 模拟通知（仅 Console 输出）");
#endif

                // 测试通知 1：5 秒后触发
                NotificationData notification1 = new NotificationData
                {
                    Id = 1,
                    Title = "📅 活动提醒",
                    Content = "您设置的活动即将开始！点击查看详情。",
                    FireTimeMillis = NotificationManager.GetMillis(DateTime.Now.AddSeconds(5))
                };
                NotificationManager.Instance.ScheduleNotification(notification1);

                // 测试通知 2：15 秒后触发
                NotificationData notification2 = new NotificationData
                {
                    Id = 2,
                    Title = "🎁 签到提醒",
                    Content = "快来签到领取每日奖励！连续签到有惊喜哦~",
                    FireTimeMillis = NotificationManager.GetMillis(DateTime.Now.AddSeconds(15))
                };
                NotificationManager.Instance.ScheduleNotification(notification2);

                // 测试通知 3：30 秒后触发
                NotificationData notification3 = new NotificationData
                {
                    Id = 3,
                    Title = "👥 好友邀请",
                    Content = "您的好友刚刚上线了，快来一起游戏吧！",
                    FireTimeMillis = NotificationManager.GetMillis(DateTime.Now.AddSeconds(30))
                };
                NotificationManager.Instance.ScheduleNotification(notification3);

                Debug.Log("[NotificationInitializer] 已安排 3 条测试通知：");
                Debug.Log("  - 通知 1：5 秒后触发");
                Debug.Log("  - 通知 2：15 秒后触发");
                Debug.Log("  - 通知 3：30 秒后触发");
                Debug.Log("========================");
            }
        }
    }
}
