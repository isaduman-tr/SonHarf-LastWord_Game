// SwipeHandler.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private AvatarMarketManager marketManager;
    [SerializeField] private RectTransform swipeArea;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(swipeArea, eventData.position, eventData.pressEventCamera))
            return;

        marketManager.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(swipeArea, eventData.position, eventData.pressEventCamera))
            return;

        marketManager.OnEndDrag(eventData);
    }
}
