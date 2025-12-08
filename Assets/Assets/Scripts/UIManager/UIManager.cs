using System.Collections.Generic;
using UnityEngine;

namespace StudyUnity
{
    // 假设你使用Addressables或Resources加载，这里用伪代码Load
    // 假设有一个Singleton<T>基类
    public class UIManager : Singleton<UIManager>
    {
        // 存储所有层级的父节点
        private Dictionary<UILayer, Transform> _layers = new Dictionary<UILayer, Transform>();
        
        // 缓存已加载的UI (UIName -> UIBase)
        private Dictionary<string, UIBase> _loadedUIs = new Dictionary<string, UIBase>();

        // 窗口堆栈 (用于处理 Android 返回键逻辑)
        // 记录打开的 Normal 和 Top 层窗口
        private Stack<UIBase> _uiStack = new Stack<UIBase>();

        public void Init(Transform canvasRoot)
        {
            // 初始化层级节点，确保渲染顺序正确
            _layers[UILayer.Bottom] = canvasRoot.Find("Layer_Bottom");
            _layers[UILayer.Normal] = canvasRoot.Find("Layer_Normal");
            _layers[UILayer.Top] = canvasRoot.Find("Layer_Top");
            _layers[UILayer.System] = canvasRoot.Find("Layer_System");
        }

        // 打开UI
        public void OpenUI(string uiName, params object[] args)
        {
            UIConfig config = GetConfig(uiName); // 从配置表获取配置
            
            UIBase ui = null;

            // 1. 检查缓存
            if (_loadedUIs.TryGetValue(uiName, out ui))
            {
                // 已经在缓存中，直接打开
                ShowUI(ui, config, args);
                return;
            }

            // 2. 加载资源 (这里简化为同步，实际建议用异步)
            GameObject prefab = Resources.Load<GameObject>(config.Path);
            if (prefab == null) return;

            // 3. 实例化并设置层级
            GameObject go = Object.Instantiate(prefab, _layers[config.Layer]);
            ui = go.GetComponent<UIBase>();
            ui.UIName = uiName;
            
            // 4. 初始化
            ui.OnInit();
            _loadedUIs.Add(uiName, ui);

            ShowUI(ui, config, args);
        }

        private void ShowUI(UIBase ui, UIConfig config, params object[] args)
        {
            // 调整层级顺序：让当前打开的UI处于同层级的最上方
            ui.transform.SetAsLastSibling();
            
            ui.OnOpen(args);

            // SLG特有优化：如果是全屏界面，隐藏背后的3D场景或其他UI以节省DrawCall
            if (config.IsFullScreen)
            {
                // TODO: 通知场景管理器暂停渲染，或者隐藏Layer_Bottom
            }

            // 入栈逻辑 (只对Normal和Top层入栈)
            if (config.Layer == UILayer.Normal || config.Layer == UILayer.Top)
            {
                // 如果栈顶已经是自己，就不重复入栈
                if (_uiStack.Count == 0 || _uiStack.Peek() != ui)
                {
                    _uiStack.Push(ui);
                }
            }
        }

        // 关闭UI
        public void CloseUI(string uiName)
        {
            if (!_loadedUIs.TryGetValue(uiName, out var ui)) return;
            if (!ui.IsVisible) return;

            ui.OnClose();

            // 栈处理：如果是栈顶UI被关闭，直接Pop
            // 如果不是栈顶（比如关闭了背后的窗口），需要特殊处理栈（略复杂，通常SLG只允许关闭栈顶）
            if (_uiStack.Count > 0 && _uiStack.Peek() == ui)
            {
                _uiStack.Pop();
            }

            UIConfig config = GetConfig(uiName);
            
            // SLG特有优化：关闭全屏界面，恢复背后显示
            if (config.IsFullScreen)
            {
                // TODO: 恢复场景渲染
            }

            // 缓存策略
            if (!config.IsCache)
            {
                _loadedUIs.Remove(uiName);
                ui.OnDestroyUI();
                Object.Destroy(ui.gameObject);
            }
        }

        // 物理返回键处理 (Android Back)
        public void OnBackKeyPressed()
        {
            if (_uiStack.Count > 0)
            {
                var topUI = _uiStack.Peek();
                // 允许UI自己拦截返回键 (比如正在引导中，不允许关闭)
                // if (topUI.CanCloseByBackKey()) 
                CloseUI(topUI.UIName);
            }
            else
            {
                // 弹出退出游戏确认框
            }
        }

        // 模拟配置表获取
        private UIConfig GetConfig(string name) { return new UIConfig(); /*读取配置*/ }
    }
}