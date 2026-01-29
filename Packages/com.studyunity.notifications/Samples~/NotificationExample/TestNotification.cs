using UnityEngine;
using System;

namespace StudyUnity.Notification
{
    /// <summary>
    /// 通知测试脚本
    /// 在 Start 时自动安排几条测试通知
    /// </summary>
    public class TestNotification : MonoBehaviour
    {
        [Header("测试配置")]
        [Tooltip("是否在 Start 时自动安排测试通知")]
        [SerializeField] private bool autoScheduleOnStart = true;

        [Tooltip("第一条通知延迟秒数")]
        [SerializeField] private float firstNotificationDelay = 10f;

        [Tooltip("第二条通知延迟分钟数")]
        [SerializeField] private float secondNotificationDelayMinutes = 1f;

        [Tooltip("是否显示调试信息")]
        [SerializeField] private bool showDebugInfo = true;

        private void Start()
        {
            if (autoScheduleOnStart)
            {
                ScheduleTestNotifications();
            }

            if (showDebugInfo)
            {
                ShowDebugInfo();
            }
        }

        private void ScheduleTestNotifications()
        {
            Debug.Log("[TestNotification] 开始安排测试通知...");

            // 测试通知 1：10 秒后触发
            try
            {
                NotificationData notification1 = new NotificationData
                {
                    Id = 1,
                    Title = "活动提醒",
                    Content = "您设置的活动即将开始！点击查看详情。",
                    FireTimeMillis = NotificationManager.GetMillis(DateTime.Now.AddSeconds(firstNotificationDelay)),
                    SmallIcon = "ic_launcher"
                };
                Debug.Log($"[TestNotification] 准备调度通知1: {notification1.Title}");
                NotificationManager.Instance.ScheduleNotification(notification1);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[TestNotification] 通知1调度失败: {e.Message}");
            }

            // 测试通知 2：1 分钟后触发
            try
            {
                NotificationData notification2 = new NotificationData
                {
                    Id = 2,
                    Title = "签到提醒",
                    Content = "快来签到领取每日奖励！连续签到有惊喜哦~",
                    FireTimeMillis = NotificationManager.GetMillis(DateTime.Now.AddMinutes(secondNotificationDelayMinutes)),
                    SmallIcon = "ic_launcher"
                };
                Debug.Log($"[TestNotification] 准备调度通知2: {notification2.Title}");
                NotificationManager.Instance.ScheduleNotification(notification2);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[TestNotification] 通知2调度失败: {e.Message}");
            }

            // 测试通知 3：30 秒后触发
            try
            {
                NotificationData notification3 = new NotificationData
                {
                    Id = 3,
                    Title = "好友邀请",
                    Content = "您的好友刚刚上线了，快来一起游戏吧！",
                    FireTimeMillis = NotificationManager.GetMillis(DateTime.Now.AddSeconds(30)),
                    SmallIcon = "ic_launcher"
                };
                Debug.Log($"[TestNotification] 准备调度通知3: {notification3.Title}");
                NotificationManager.Instance.ScheduleNotification(notification3);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[TestNotification] 通知3调度失败: {e.Message}");
            }

            Debug.Log("[TestNotification] 测试通知安排完成");
        }

        private void ShowDebugInfo()
        {
            Debug.Log("=== 通知系统测试信息 ===");
            Debug.Log($"当前时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Debug.Log($"通知1 将在 {firstNotificationDelay} 秒后触发");
            Debug.Log($"通知2 将在 {secondNotificationDelayMinutes} 分钟后触发");
            Debug.Log($"通知3 将在 30 秒后触发");
            Debug.Log($"当前平台: {Application.platform}");
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log("使用 Android 原生通知系统");
#else
            Debug.Log("使用 Editor 模拟通知（仅 Console 输出）");
#endif
            Debug.Log("========================");
        }

        // 手动调度通知的公共方法（供 UI 按钮调用）
        public void ScheduleCustomNotification(int id, string title, string content, float delaySeconds)
        {
            NotificationData notification = new NotificationData
            {
                Id = id,
                Title = title,
                Content = content,
                FireTimeMillis = NotificationManager.GetMillis(DateTime.Now.AddSeconds(delaySeconds)),
                SmallIcon = "ic_launcher"
            };
            NotificationManager.Instance.ScheduleNotification(notification);
        }

        // 取消指定通知
        public void CancelNotification(int id)
        {
            NotificationManager.Instance.CancelNotification(id);
        }

        // 取消所有通知
        public void CancelAllNotifications()
        {
            NotificationManager.Instance.CancelAllNotifications();
        }

        // 快速测试：5秒后触发通知
        public void QuickTest()
        {
            ScheduleCustomNotification(999, "快速测试", "这是一条快速测试通知！", 5f);
        }
    }
}
