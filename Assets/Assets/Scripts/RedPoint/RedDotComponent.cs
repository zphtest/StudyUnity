using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StudyUnity
{
    public class RedDotComponent : MonoBehaviour
    {
        [SerializeField] private string _staticPath; // Inspector里填写的静态路径，例如 "Main.Bag"
        private string _runtimePath; // 运行时确定的实际路径

        // 拖拽引用
        public GameObject DotObj;
        public GameObject NewObj;
        public TextMeshProUGUI CountText;
        public GameObject CountBg;

        private void Start()
        {
            if (!string.IsNullOrEmpty(_staticPath))
            {
                SetPath(_staticPath);
            }
        }
        
        // 2. 动态节点：提供API供外部调用 (关键!)
        // 在代码里 Instantiate 页签后，立即调用这个方法
        public void SetPath(string path)
        {
            // 如果之前注册过，先反注册（防止复用Item时的Bug）
            Unregister();

            _runtimePath = path;
        
            var node = RedDotManager.Instance.RegisterNode(path);
            RefreshView(node.Data);
            node.OnChange += RefreshView;
        }
        
        private void Unregister()
        {
            if (string.IsNullOrEmpty(_runtimePath)) return;
            var node = RedDotManager.Instance.GetNode(_runtimePath);
            if (node != null) node.OnChange -= RefreshView;
        }

        private void OnDestroy()
        {
            Unregister();
        }

        private void RefreshView(RedDotData data)
        {
            // 隐藏所有
            if(DotObj) DotObj.SetActive(false);
            if(NewObj) NewObj.SetActive(false);
            if(CountBg) CountBg.SetActive(false);

            switch (data.Type)
            {
                case RedDotType.Dot:
                    if(DotObj) DotObj.SetActive(true);
                    break;
                case RedDotType.New:
                    if(NewObj) NewObj.SetActive(true);
                    break;
                case RedDotType.Number:
                    if(CountBg) CountBg.SetActive(true);
                    if(CountText) CountText.text = data.Count > 99 ? "99+" : data.Count.ToString();
                    break;
            }
        }
    }
}
