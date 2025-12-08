
namespace StudyUnity
{
    public static class RedDotPath
    {
        // 一些带子界面得静态路径
        public const string Main_Bag = "Main.Bag";
        public const string Main_Activity = "Main.Activity";

        // 动态路径模板 (用 {0} 占位)
        public const string Main_Activity_Item = "Main.Activity.{0}"; 
    
        // 辅助方法：生成动态路径
        public static string GetActivityPath(int activityId)
        {
            // 后续使用 StringBuilder 或 string.Create (.NET Core/Unity 2021+) 优化GC
            // 这里为了演示简单直接 Format
            return string.Format(Main_Activity_Item, activityId);
        }
    }
}
