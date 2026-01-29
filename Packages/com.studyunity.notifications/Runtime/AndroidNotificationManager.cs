using UnityEngine;

namespace StudyUnity.Notification
{
    /// <summary>
    /// Android 平台通知管理器实现
    /// 使用 AndroidJavaObject 调用原生 Android API
    /// </summary>
    internal class AndroidNotificationManager : INotificationPlatform
    {
        private const string HelperClassName = "com.yourcompany.notification.NotificationHelper";

        private AndroidJavaObject _helper;

        public AndroidNotificationManager()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                Debug.LogError("[AndroidNotificationManager] 当前平台不是 Android");
                return;
            }

            try
            {
                // 获取当前 Activity
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // 创建 NotificationHelper 实例
                _helper = new AndroidJavaObject(HelperClassName, activity);

                Debug.Log("[AndroidNotificationManager] 初始化成功");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidNotificationManager] 初始化失败: {e.Message}");
            }
        }

        public void ScheduleNotification(NotificationData data)
        {
            if (_helper == null)
            {
                Debug.LogError("[AndroidNotificationManager] Helper 未初始化");
                return;
            }

            try
            {
                // 设置默认图标
                string smallIcon = string.IsNullOrEmpty(data.SmallIcon) ? "ic_launcher" : data.SmallIcon;

                // 调用 Android 方法
                _helper.Call("scheduleNotification",
                    data.Id,
                    data.Title,
                    data.Content,
                    data.FireTimeMillis,
                    smallIcon);

                Debug.Log($"[AndroidNotificationManager] 已调度通知: ID={data.Id}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidNotificationManager] 调度通知失败: {e.Message}");
            }
        }

        public void CancelNotification(int id)
        {
            if (_helper == null)
            {
                Debug.LogError("[AndroidNotificationManager] Helper 未初始化");
                return;
            }

            try
            {
                _helper.Call("cancelNotification", id);
                Debug.Log($"[AndroidNotificationManager] 已取消通知: ID={id}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidNotificationManager] 取消通知失败: {e.Message}");
            }
        }

        public void CancelAllNotifications()
        {
            if (_helper == null)
            {
                Debug.LogError("[AndroidNotificationManager] Helper 未初始化");
                return;
            }

            try
            {
                _helper.Call("cancelAllNotifications");
                Debug.Log("[AndroidNotificationManager] 已取消所有通知");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidNotificationManager] 取消所有通知失败: {e.Message}");
            }
        }
    }
}
