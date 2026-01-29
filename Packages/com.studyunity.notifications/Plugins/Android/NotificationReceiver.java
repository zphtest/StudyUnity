package com.yourcompany.notification;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.util.Log;
import androidx.core.app.NotificationCompat;

/**
 * 广播接收器
 * 在 AlarmManager 触发的时间点接收通知
 */
public class NotificationReceiver extends BroadcastReceiver {
    private static final String TAG = "NotificationReceiver";
    private static final String CHANNEL_ID = "unity_notification_channel";

    @Override
    public void onReceive(Context context, Intent intent) {
        try {
            // 从 Intent 中获取通知数据
            int notificationId = intent.getIntExtra("notification_id", 0);
            String title = intent.getStringExtra("title");
            String content = intent.getStringExtra("content");
            String smallIcon = intent.getStringExtra("small_icon");

            Log.d(TAG, String.format("Received notification broadcast: ID=%d, Title=%s", notificationId, title));

            // 显示通知
            showNotification(context, notificationId, title, content, smallIcon);

        } catch (Exception e) {
            Log.e(TAG, "Failed to receive notification", e);
        }
    }

    /**
     * 显示通知
     */
    private void showNotification(Context context, int id, String title, String content, String smallIconName) {
        try {
            NotificationManager notificationManager =
                    (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

            // 创建点击通知后打开应用的 Intent
            Intent launchIntent = context.getPackageManager().getLaunchIntentForPackage(context.getPackageName());
            int flags = PendingIntent.FLAG_UPDATE_CURRENT;
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                flags |= PendingIntent.FLAG_IMMUTABLE;
            }
            PendingIntent pendingIntent = PendingIntent.getActivity(context, id, launchIntent, flags);

            // 构建通知
            NotificationCompat.Builder builder = new NotificationCompat.Builder(context, CHANNEL_ID)
                    .setSmallIcon(getSmallIconResourceId(context, smallIconName))
                    .setContentTitle(title)
                    .setContentText(content)
                    .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                    .setContentIntent(pendingIntent)
                    .setAutoCancel(true)
                    .setWhen(System.currentTimeMillis());

            // 添加震动和声音
            builder.setDefaults(Notification.DEFAULT_SOUND | Notification.DEFAULT_VIBRATE);

            // 显示通知
            Notification notification = builder.build();
            notificationManager.notify(id, notification);

            Log.d(TAG, String.format("Notification displayed: ID=%d", id));

        } catch (Exception e) {
            Log.e(TAG, "Failed to show notification", e);
        }
    }

    /**
     * 获取图标资源 ID
     */
    private int getSmallIconResourceId(Context context, String iconName) {
        if (iconName == null || iconName.isEmpty()) {
            return android.R.drawable.ic_dialog_info;
        }

        try {
            int resourceId = context.getResources().getIdentifier(iconName, "drawable", context.getPackageName());
            if (resourceId == 0) {
                Log.w(TAG, "Icon resource not found: " + iconName + ", using default");
                return android.R.drawable.ic_dialog_info;
            }
            return resourceId;
        } catch (Exception e) {
            Log.e(TAG, "Failed to get icon resource", e);
            return android.R.drawable.ic_dialog_info;
        }
    }
}
