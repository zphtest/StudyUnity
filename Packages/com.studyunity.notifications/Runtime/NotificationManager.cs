using System;
using System.Collections.Generic;
using UnityEngine;

namespace StudyUnity.Notification
{
    /// <summary>
    /// 通知管理器 - 单例
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {
        private static NotificationManager _instance;
        private static readonly object _lock = new object();
        public static NotificationManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("NotificationManager");
                        _instance = go.AddComponent<NotificationManager>();
                        DontDestroyOnLoad(go);

                        // 确保立即初始化
                        _instance.InitializePlatform();
                    }
                    return _instance;
                }
            }
        }

        private INotificationPlatform _platformImpl;
        private bool _isInitialized = false;

        private void Awake()
        {
            lock (_lock)
            {
                if (_instance != null && _instance != this)
                {
                    Destroy(gameObject);
                    return;
                }
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void InitializePlatform()
        {
            if (_isInitialized) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            _platformImpl = new AndroidNotificationManager();
#else
            _platformImpl = new EditorNotificationManager();
#endif
            _isInitialized = true;
            Debug.Log($"[NotificationManager] 初始化平台实现: {_platformImpl?.GetType().Name ?? "NULL"}");
        }

        /// <summary>
        /// 安排通知
        /// </summary>
        public void ScheduleNotification(NotificationData data)
        {
            if (_platformImpl == null)
            {
                Debug.LogError("[NotificationManager] _platformImpl 为空，尝试重新初始化...");
                InitializePlatform();
            }

            if (data == null)
            {
                Debug.LogError("[NotificationManager] 通知数据为空，无法安排通知");
                return;
            }

            if (string.IsNullOrEmpty(data.Title))
            {
                Debug.LogWarning("[NotificationManager] 通知标题为空");
            }

            _platformImpl.ScheduleNotification(data);
            Debug.Log($"[NotificationManager] 已安排通知: ID={data.Id}, Title={data.Title}, Time={DateTimeFromMillis(data.FireTimeMillis):yyyy-MM-dd HH:mm:ss}");
        }

        /// <summary>
        /// 取消指定通知
        /// </summary>
        public void CancelNotification(int id)
        {
            _platformImpl.CancelNotification(id);
            Debug.Log($"[NotificationManager] 已取消通知: ID={id}");
        }

        /// <summary>
        /// 取消所有通知
        /// </summary>
        public void CancelAllNotifications()
        {
            _platformImpl.CancelAllNotifications();
            Debug.Log("[NotificationManager] 已取消所有通知");
        }

        /// <summary>
        /// 获取当前毫秒时间戳
        /// </summary>
        public static long GetCurrentMillis()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 从 DateTime 获取毫秒时间戳
        /// </summary>
        public static long GetMillis(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 从毫秒时间戳获取 DateTime
        /// </summary>
        public static DateTime DateTimeFromMillis(long millis)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(millis).LocalDateTime;
        }
    }

    /// <summary>
    /// 平台通知接口
    /// </summary>
    internal interface INotificationPlatform
    {
        void ScheduleNotification(NotificationData data);
        void CancelNotification(int id);
        void CancelAllNotifications();
    }

    /// <summary>
    /// Editor 模式下的模拟实现（用于测试）
    /// </summary>
    internal class EditorNotificationManager : INotificationPlatform
    {
        private readonly Dictionary<int, NotificationData> _scheduledNotifications = new Dictionary<int, NotificationData>();
        private readonly Dictionary<int, Coroutine> _activeCoroutines = new Dictionary<int, Coroutine>();

        public void ScheduleNotification(NotificationData data)
        {
            // 取消之前的通知（如果存在）
            if (_activeCoroutines.ContainsKey(data.Id))
            {
                NotificationManager.Instance.StopCoroutine(_activeCoroutines[data.Id]);
                _activeCoroutines.Remove(data.Id);
            }

            _scheduledNotifications[data.Id] = data;

            long currentTime = NotificationManager.GetCurrentMillis();
            long delay = data.FireTimeMillis - currentTime;

            if (delay > 0)
            {
                float delaySeconds = delay / 1000f;
                Debug.Log($"[EditorNotificationManager] 模拟安排通知: {data.Title} at {NotificationManager.DateTimeFromMillis(data.FireTimeMillis):yyyy-MM-dd HH:mm:ss} (将在 {delaySeconds:F1} 秒后触发)");

                // 启动协程，在指定时间后触发通知
                Coroutine coroutine = NotificationManager.Instance.StartCoroutine(DelayedNotification(data.Id, delaySeconds));
                _activeCoroutines[data.Id] = coroutine;
            }
            else
            {
                Debug.LogWarning($"[EditorNotificationManager] 通知时间已过: {data.Title}");
            }
        }

        private System.Collections.IEnumerator DelayedNotification(int id, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);

            if (_scheduledNotifications.TryGetValue(id, out NotificationData data))
            {
                Debug.Log($"<color=#FF9800><b>🔔 [EditorNotificationManager] 通知触发!</b></color>");
                Debug.Log($"<color=#FF9800>┌─────────────────────────────────</color>");
                Debug.Log($"<color=#FF9800>│ <b>标题:</b> {data.Title}</color>");
                Debug.Log($"<color=#FF9800>│ <b>内容:</b> {data.Content}</color>");
                Debug.Log($"<color=#FF9800>│ <b>ID:</b> {data.Id}</color>");
                Debug.Log($"<color=#FF9800>└─────────────────────────────────</color>");

                _scheduledNotifications.Remove(id);
                _activeCoroutines.Remove(id);
            }
        }

        public void CancelNotification(int id)
        {
            // 停止协程
            if (_activeCoroutines.TryGetValue(id, out Coroutine coroutine))
            {
                if (coroutine != null)
                {
                    NotificationManager.Instance.StopCoroutine(coroutine);
                }
                _activeCoroutines.Remove(id);
            }

            if (_scheduledNotifications.Remove(id))
            {
                Debug.Log($"[EditorNotificationManager] 模拟取消通知: ID={id}");
            }
        }

        public void CancelAllNotifications()
        {
            // 停止所有协程
            foreach (var kvp in _activeCoroutines)
            {
                if (kvp.Value != null)
                {
                    NotificationManager.Instance.StopCoroutine(kvp.Value);
                }
            }
            _activeCoroutines.Clear();

            int count = _scheduledNotifications.Count;
            _scheduledNotifications.Clear();
            Debug.Log($"[EditorNotificationManager] 模拟取消所有通知，共 {count} 条");
        }
    }
}
