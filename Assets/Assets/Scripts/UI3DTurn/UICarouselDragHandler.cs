using UnityEngine;
using UnityEngine.EventSystems;

public class UICarouselDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public UICarouselFlat carousel;

    private Vector2 lastPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - lastPos;
        lastPos = eventData.position;

        // 拖动时移动轮播
        carousel.Move(-delta.x);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 松手后吸附到最近的卡片
        carousel.SnapToNearest();
    }
}