# StudyUnity 通知系统

## 功能特性

- ✅ 支持在指定时间点触发通知
- ✅ App 关闭状态下也能收到通知（Android）
- ✅ 基于 Android 原生 AlarmManager 实现
- ✅ Editor 模式下可模拟测试
- ✅ 单例模式，易于调用

## 快速开始

### 1. 基本使用

```csharp
using StudyUnity.Notification;

// 调度通知（5秒后触发）
NotificationManager.Instance.ScheduleNotification(new NotificationData {
    Id = 1,
    Title = "测试通知",
    Content = "这是通知内容",
    FireTimeMillis = NotificationManager.GetMillis(DateTime.Now.AddSeconds(5))
});

// 取消指定通知
NotificationManager.Instance.CancelNotification(1);

// 取消所有通知
NotificationManager.Instance.CancelAllNotifications();
```

### 2. 配置 Android

确保项目的 `package name` 与 Java 文件中的包名一致，或修改以下文件中的包名：

- `Plugins/Android/NotificationHelper.java`
- `Plugins/Android/NotificationReceiver.java`

默认包名：`com.yourcompany.notification`

修改为自己的包名：
```java
package com.yourcompany.notification; // 改为你的包名
```

### 3. Android 权限

系统会自动配置以下权限（在 `AndroidManifest.xml` 中）：

- `POST_NOTIFICATIONS` - Android 13+ 通知权限
- `SCHEDULE_EXACT_ALARM` - Android 12+ 精确闹钟权限
- `USE_EXACT_ALARM` - 使用精确闹钟
- `WAKE_LOCK` - 唤醒锁
- `RECEIVE_BOOT_COMPLETED` - 开机自启

## API 参考

### NotificationData

通知数据类

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int | 唯一标识 |
| Title | string | 通知标题 |
| Content | string | 通知内容 |
| FireTimeMillis | long | 触发时间（毫秒时间戳） |
| SmallIcon | string | 小图标资源名称（可选） |

### NotificationManager

通知管理器（单例）

| 方法 | 说明 |
|------|------|
| `ScheduleNotification(data)` | 安排通知 |
| `CancelNotification(id)` | 取消指定通知 |
| `CancelAllNotifications()` | 取消所有通知 |

## 测试

### Editor 模式测试

1. 将 `TestNotification.cs` 添加到场景中的 GameObject
2. 点击 Play 运行
3. 查看 Console 输出

### Android 设备测试

1. 打包到 Android 设备
2. 运行应用
3. 等待通知触发（即使关闭应用也会收到）

## 常见问题

**Q: 通知没有触发？**

A: 检查以下几点：
- Android 设备的权限是否开启
- 系统设置中是否允许应用发送通知
- 通知时间是否设置正确

**Q: 如何修改包名？**

A: 修改 `NotificationHelper.java` 和 `NotificationReceiver.java` 中的 package 声明，以及 `AndroidManifest.xml` 中的 package 属性。

**Q: 支持哪些 Android 版本？**

A: 支持 Android 5.0 (API 21) 及以上版本。

## 版本历史

- **1.0.0** (2026-01-28)
  - 初始版本
  - 支持基本的本地通知功能
