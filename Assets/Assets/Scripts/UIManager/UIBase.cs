using UnityEngine;

namespace StudyUnity
{
    public abstract class UIBase : MonoBehaviour
    {
        public string UIName { get; set; }
    
        // UI状态：是否正在显示
        public bool IsVisible { get; private set; }

        // 初始化：只执行一次 (GetComponent, 添加监听)
        public virtual void OnInit() { }

        // 打开：每次Open都会执行 (数据刷新、动画播放)
        public virtual void OnOpen(params object[] args) 
        { 
            IsVisible = true;
            gameObject.SetActive(true);
        }

        // 关闭：每次Close都会执行 (清理临时数据、停掉动画)
        public virtual void OnClose() 
        { 
            IsVisible = false;
            gameObject.SetActive(false);
        }

        // 销毁：真正Destroy时执行
        public virtual void OnDestroyUI() { }

        // 提供给管理器的各种快捷方法
        protected void CloseSelf()
        {
            UIManager.Instance.CloseUI(UIName);
        }
    }
}