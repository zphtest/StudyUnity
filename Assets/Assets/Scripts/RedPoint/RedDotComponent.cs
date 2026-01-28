using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StudyUnity
{
    public class RedDotComponent : MonoBehaviour
    {
        [Tooltip("静态节点名称(可选)")]
        [SerializeField] private string _staticKeyName;
        
        // UI 引用
        [Header("UI Elements")]
        [SerializeField] private GameObject _dotObj;   // 普通圆点
        [SerializeField] private GameObject _newObj;   // New 标签
        [SerializeField] private TextMeshProUGUI _countText;      // 数字文本
        [SerializeField] private GameObject _countBg;  // 数字背景

        private int _targetKey;
        private bool _isRegistered;

        private void Awake()
        {
            if (!string.IsNullOrEmpty(_staticKeyName))
            {
                SetKey(Animator.StringToHash(_staticKeyName));
            }
        }

        public void SetKey(int key)
        {
            if (_isRegistered) Unregister();
            
            _targetKey = key;
            var node = RedDotManager.Instance.GetNode(key);
            
            // 首次刷新
            Refresh(node.FinalData);
            
            // 监听
            node.OnChange += Refresh;
            _isRegistered = true;
        }

        private void Unregister()
        {
            if (!_isRegistered) return;
            var node = RedDotManager.Instance.GetNode(_targetKey);
            if (node != null) node.OnChange -= Refresh;
            _isRegistered = false;
        }
        
        private void OnDestroy() => Unregister();

        // 根据 Data 决定显示哪个子物体
        private void Refresh(RedDotData data)
        {
            // 1. 全部隐藏
            if (_dotObj) _dotObj.SetActive(false);
            if (_newObj) _newObj.SetActive(false);
            if (_countBg) _countBg.SetActive(false);

            if (data.Type == ERedDotType.None) return;

            // 2. 根据类型显示
            switch (data.Type)
            {
                case ERedDotType.Dot:
                    if (_dotObj) _dotObj.SetActive(true);
                    break;

                case ERedDotType.New:
                    if (_newObj) _newObj.SetActive(true);
                    break;

                case ERedDotType.Number:
                    if (_countBg) _countBg.SetActive(true);
                    if (_countText) 
                    {
                        // 超过99显示99+
                        _countText.text = data.Count > 99 ? "99+" : data.Count.ToString();
                    }
                    break;
            }
        }
    }
}
