# 通知系统示例

本示例展示如何使用 StudyUnity 通知系统。

## 使用方法

1. 在 Unity Editor 中，将 `TestNotification.cs` 脚本添加到场景中的 GameObject
2. 点击 Play 运行场景
3. 查看 Console 输出

## 示例脚本说明

`TestNotification.cs` 提供了以下功能：

- **自动测试**：在 Start 时自动安排 3 条测试通知
- **手动测试**：提供公共方法供 UI 按钮调用

### 公共方法

```csharp
// 调度自定义通知
public void ScheduleCustomNotification(int id, string title, string content, float delaySeconds)

// 取消指定通知
public void CancelNotification(int id)

// 取消所有通知
public void CancelAllNotifications()

// 快速测试（5秒后触发）
public void QuickTest()
```

## Inspector 参数

| 参数 | 说明 | 默认值 |
|------|------|--------|
| Auto Schedule On Start | 是否在 Start 时自动安排测试通知 | true |
| First Notification Delay | 第一条通知延迟秒数 | 10 |
| Second Notification Delay Minutes | 第二条通知延迟分钟数 | 1 |
| Show Debug Info | 是否显示调试信息 | true |
