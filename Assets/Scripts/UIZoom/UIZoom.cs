using UnityEngine;
using UnityEngine.EventSystems;

namespace Tuyoo
{
    /// <summary>
    /// 主要用于一些小地图弹窗，放大后拖拽使用
    /// </summary>
    public class UIZoom : MonoBehaviour
    {
        [Header("引用设置")]
        public RectTransform viewport;      // 视口区域
        public RectTransform content;       // 地图内容
        public Canvas parentCanvas;         // 父Canvas
        
        [Header("缩放设置")]
        [Range(1f, 3f)]
        public float minScale = 1f;         // 最小缩放
        [Range(1f, 3f)]
        public float maxScale = 1.5f;       // 最大缩放
        public float zoomSensitivity = 1f;  // 缩放灵敏度
        
        [Header("拖动设置")]
        public bool enableDrag = true;      // 启用拖动
        public float dragSensitivity = 1f;  // 拖动灵敏度
        
        // 私有变量
        private Camera uiCamera;
        private bool isPinching = false;
        private bool isDragging = false;
        private float initialDistance = 0f;
        private float initialScale = 1f;
        private Vector2 initialTouchCenter;
        private Vector2 lastSingleTouchPos;

        void Start()
        {
            // 获取UI相机
            uiCamera = parentCanvas.worldCamera;
            
            // 初始化
            if (content != null)
            {
                content.localScale = Vector3.one * minScale;
                CenterContent();
            }
        }

        void Update()
        {
            HandleInput();
        }

        void HandleInput()
        {
            // PC端支持：鼠标滚轮缩放和拖拽
            #if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouseInput();
            #else
            // 处理触摸输入
            if (Input.touchCount >= 2)
            {
                HandlePinchZoom();
            }
            else if (Input.touchCount == 1)
            {
                HandleSingleTouch();
            }
            else
            {
                // 重置状态
                isPinching = false;
                isDragging = false;
            }
            #endif
        }

        void HandlePinchZoom()
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            
            Vector2 touch1Pos = touch1.position;
            Vector2 touch2Pos = touch2.position;
            
            float currentDistance = Vector2.Distance(touch1Pos, touch2Pos);
            Vector2 currentCenter = (touch1Pos + touch2Pos) * 0.5f;

            if (!isPinching)
            {
                // 开始双指缩放
                isPinching = true;
                isDragging = false;
                initialDistance = currentDistance;
                initialScale = content.localScale.x;
                initialTouchCenter = currentCenter;
            }
            else
            {
                // 计算缩放比例
                if (initialDistance > 0)
                {
                    float scaleRatio = currentDistance / initialDistance;
                    float targetScale = Mathf.Clamp(initialScale * scaleRatio, minScale, maxScale);
                    
                    // 围绕双指中心缩放
                    ZoomAroundPoint(targetScale, currentCenter);
                }
                
                // 处理双指平移
                Vector2 centerDelta = currentCenter - initialTouchCenter;
                if (centerDelta.magnitude > 5f) // 避免微小抖动
                {
                    PanContent(centerDelta);
                    initialTouchCenter = currentCenter;
                }
            }
        }

        void HandleSingleTouch()
        {
            if (isPinching) return; // 双指操作时不处理单指
            
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastSingleTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging && enableDrag)
            {
                Vector2 deltaPos = touch.position - lastSingleTouchPos;
                PanContent(deltaPos);
                lastSingleTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }

        #if UNITY_EDITOR || UNITY_STANDALONE
        void HandleMouseInput()
        {
            // 鼠标滚轮缩放
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                float currentScale = content.localScale.x;
                float targetScale = Mathf.Clamp(currentScale + scroll * zoomSensitivity, minScale, maxScale);
                ZoomAroundPoint(targetScale, Input.mousePosition);
            }
            
            // 鼠标拖拽
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastSingleTouchPos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0) && isDragging && enableDrag)
            {
                Vector2 deltaPos = (Vector2)Input.mousePosition - lastSingleTouchPos;
                PanContent(deltaPos);
                lastSingleTouchPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
        #endif

        void ZoomAroundPoint(float targetScale, Vector2 screenPoint)
        {
            if (content == null || viewport == null) return;
    
            float currentScale = content.localScale.x;
            if (Mathf.Approximately(currentScale, targetScale)) return;

            // 获取缩放前鼠标在Content中的本地坐标点
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                content, screenPoint, uiCamera, out localPoint);

            // 记录缩放前的Content位置
            Vector2 oldPosition = content.anchoredPosition;
    
            // 应用缩放
            content.localScale = Vector3.one * targetScale;
    
            // 计算缩放比例
            float scaleRatio = targetScale / currentScale;
    
            // 计算需要的位置偏移，让焦点保持在原位
            // 关键：使用 (scaleRatio - 1) 来计算偏移量
            Vector2 offset = localPoint * (scaleRatio - 1f);
    
            // 应用偏移（注意这里是减去偏移量）
            content.anchoredPosition = oldPosition - offset;

            // 限制边界
            ClampContentPosition();
        }

        void PanContent(Vector2 screenDelta)
        {
            if (content == null) return;
            
            // 将屏幕移动转换为Content坐标移动
            Vector2 localDelta = screenDelta * dragSensitivity;
            
            // 考虑Canvas缩放因子
            if (parentCanvas != null)
            {
                localDelta /= parentCanvas.scaleFactor;
            }
            
            content.anchoredPosition += localDelta;
            ClampContentPosition();
        }

        void ClampContentPosition()
        {
            if (content == null || viewport == null) return;

            Vector2 contentSize = content.rect.size * content.localScale.x;
            Vector2 viewportSize = viewport.rect.size;

            Vector2 pos = content.anchoredPosition;

            // X轴限制
            if (contentSize.x <= viewportSize.x)
            {
                pos.x = 0f; // 居中
            }
            else
            {
                float maxX = (contentSize.x - viewportSize.x) * 0.5f;
                pos.x = Mathf.Clamp(pos.x, -maxX, maxX);
            }

            // Y轴限制
            if (contentSize.y <= viewportSize.y)
            {
                pos.y = 0f; // 居中
            }
            else
            {
                float maxY = (contentSize.y - viewportSize.y) * 0.5f;
                pos.y = Mathf.Clamp(pos.y, -maxY, maxY);
            }

            content.anchoredPosition = pos;
        }

        // 公共方法
        public void CenterContent()
        {
            if (content != null)
            {
                content.anchoredPosition = Vector2.zero;
                ClampContentPosition();
            }
        }

        public void SetScale(float scale)
        {
            if (content != null)
            {
                float clampedScale = Mathf.Clamp(scale, minScale, maxScale);
                content.localScale = Vector3.one * clampedScale;
                ClampContentPosition();
            }
        }

        public void ResetMap()
        {
            SetScale(minScale);
            CenterContent();
        }
    }
}