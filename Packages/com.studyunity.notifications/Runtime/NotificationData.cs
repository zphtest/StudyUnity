using System;

namespace StudyUnity.Notification
{
    /// <summary>
    /// 通知数据类
    /// </summary>
    [Serializable]
    public class NotificationData
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public int Id;

        /// <summary>
        /// 通知标题
        /// </summary>
        public string Title;

        /// <summary>
        /// 通知内容
        /// </summary>
        public string Content;

        /// <summary>
        /// 触发时间（毫秒时间戳）
        /// </summary>
        public long FireTimeMillis;

        /// <summary>
        /// 通知图标（可选，Android small icon 资源名称）
        /// </summary>
        public string SmallIcon;

        /// <summary>
        /// 通知大图标（可选）
        /// </summary>
        public string LargeIcon;
    }
}
