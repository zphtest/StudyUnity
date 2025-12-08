namespace StudyUnity
{
    public enum UILayer
    {
        Bottom = 0, // 主界面
        Normal = 1, // 一般功能窗口 (背包、邮件、建筑详情)
        Top = 2,    // 弹窗 (确认框、购买框)
        System = 3  // 系统级 (Loading、Toast、断线)
    }

// UI配置数据 (建议配合ScriptableObject或表配置使用)
    public class UIConfig
    {
        public string Name;          // UI唯一标识 (如 "UIMail")
        public string Path;          // 资源路径 (Resources或Addressables地址)
        public UILayer Layer;        // 所属层级
        public bool IsCache;         // 关闭时是否缓存 (不销毁GameObject)
        public bool IsFullScreen;    // 是否是全屏窗口 (用于优化，打开全屏时隐藏下层)
    }
}
