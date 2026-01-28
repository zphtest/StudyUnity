package com.yourcompany.notification;

import android.app.Activity;
import android.app.AlarmManager;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.util.Log;
import androidx.core.app.NotificationCompat;

/**
 * Android 通知辅助类
 * 负责调度和显示本地通知
 */
public class NotificationHelper {
    private static final String TAG = "NotificationHelper";
    private static final String CHANNEL_ID = "unity_notification_channel";
    private static final String CHANNEL_NAME = "Unity Notifications";
    private static final int NOTIFICATION_ICON = android.R.drawable.ic_dialog_info;

    private Context context;
    private AlarmManager alarmManager;
    private android.app.NotificationManager notificationManager;

    public NotificationHelper(Activity activity) {
        this.context = activity.getApplicationContext();
        this.alarmManager = (AlarmManager) context.getSystemService(Context.ALARM_SERVICE);
        this.notificationManager = (android.app.NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

        createNotificationChannel();
        Log.d(TAG, "NotificationHelper initialized");
    }

    /**
     * 创建通知渠道（Android 8.0+ 需要）
     */
    private void createNotificationChannel() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationChannel channel = new NotificationChannel(
                    CHANNEL_ID,
                    CHANNEL_NAME,
                    NotificationManager.IMPORTANCE_DEFAULT
            );
            channel.setDescription("Unity game notifications");
            channel.enableVibration(true);
            notificationManager.createNotificationChannel(channel);
            Log.d(TAG, "Notification channel created");
        }
    }

    /**
     * 调度通知
     * @param id 通知唯一标识
     * @param title 通知标题
     * @param content 通知内容
     * @param fireTimeMillis 触发时间（毫秒时间戳）
     * @param smallIcon 小图标资源名称
     */
    public void scheduleNotification(int id, String title, String content, long fireTimeMillis, String smallIcon) {
        try {
            // 创建 PendingIntent 用于触发通知
            Intent intent = new Intent(context, NotificationReceiver.class);
            intent.putExtra("notification_id", id);
            intent.putExtra("title", title);
            intent.putExtra("content", content);
            intent.putExtra("small_icon", smallIcon);

            int flags = PendingIntent.FLAG_UPDATE_CURRENT;
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                flags |= PendingIntent.FLAG_IMMUTABLE;
            }

            PendingIntent pendingIntent = PendingIntent.getBroadcast(context, id, intent, flags);

            // 计算触发时间
            long currentTime = System.currentTimeMillis();
            long delay = fireTimeMillis - currentTime;

            if (delay <= 0) {
                Log.w(TAG, "Notification time is in the past, scheduling immediately");
                delay = 1000;
            }

            // 设置精确闹钟
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
                if (alarmManager.canScheduleExactAlarms()) {
                    alarmManager.setExactAndAllowWhileIdle(
                            AlarmManager.RTC_WAKEUP,
                            fireTimeMillis,
                            pendingIntent
                    );
                } else {
                    alarmManager.setAndAllowWhileIdle(
                            AlarmManager.RTC_WAKEUP,
                            fireTimeMillis,
                            pendingIntent
                    );
                }
            } else if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                alarmManager.setExactAndAllowWhileIdle(
                        AlarmManager.RTC_WAKEUP,
                        fireTimeMillis,
                        pendingIntent
                );
            } else {
                alarmManager.set(
                        AlarmManager.RTC_WAKEUP,
                        fireTimeMillis,
                        pendingIntent
                );
            }

            Log.d(TAG, String.format("Notification scheduled: ID=%d, Title=%s, Delay=%d ms", id, title, delay));

        } catch (Exception e) {
            Log.e(TAG, "Failed to schedule notification", e);
        }
    }

    /**
     * 取消指定通知
     */
    public void cancelNotification(int id) {
        try {
            Intent intent = new Intent(context, NotificationReceiver.class);

            int flags = PendingIntent.FLAG_NO_CREATE;
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                flags |= PendingIntent.FLAG_IMMUTABLE;
            }

            PendingIntent pendingIntent = PendingIntent.getBroadcast(context, id, intent, flags);

            if (pendingIntent != null) {
                alarmManager.cancel(pendingIntent);
                notificationManager.cancel(id);
                Log.d(TAG, "Notification cancelled: ID=" + id);
            }
        } catch (Exception e) {
            Log.e(TAG, "Failed to cancel notification", e);
        }
    }

    /**
     * 取消所有通知
     */
    public void cancelAllNotifications() {
        try {
            notificationManager.cancelAll();
            Log.d(TAG, "All notifications cancelled");
        } catch (Exception e) {
            Log.e(TAG, "Failed to cancel all notifications", e);
        }
    }
}
