using UnityEngine;
using UnityEngine.EventSystems;



public enum DraggableElementType
{

    DRAGGABLE_DEFAULT,
    DRAGGABLE_WHITE_CARD,
    DRAGGABLE_RED_CARD,

    // add more draggable elements here...
}



[RequireComponent(typeof(RectTransform))]
public class DropArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public int areaId;

    public DraggableElementType draggableElementType;



    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!ValidItem(eventData.pointerDrag, out DraggableElement draggedItem)) return;
        draggedItem.SetMostRecentHover(this);
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        if(!ValidItem(eventData.pointerDrag, out DraggableElement draggedItem)) return;
        draggedItem.SetMostRecentHover(null);
    }



    private bool ValidItem(GameObject pointerDrag, out DraggableElement draggedItem)
    {

        draggedItem = null;
        if(pointerDrag == null) return false;

        draggedItem = pointerDrag.GetComponent<DraggableElement>();
        if(draggedItem == null) return false;

        if(draggableElementType != DraggableElementType.DRAGGABLE_DEFAULT
        && draggedItem.draggableElementType != DraggableElementType.DRAGGABLE_DEFAULT
        && draggedItem.draggableElementType != draggableElementType) return false;

        return true;

    }



    public virtual void SwapForThis(DraggableElement draggableElement)
    {
        Debug.Log("Swapping to slot " + name);
        DropArea areaToSwapWith = draggableElement.transform.parent.GetComponent<DropArea>();
        DropAreaManager.Instance.Swap(this, areaToSwapWith);
    }

}