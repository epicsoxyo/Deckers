using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class DraggableElement : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public DraggableElementType draggableElementType;
    public bool isDraggable;

    private RectTransform _rectTransform;

    private CanvasGroup _canvasGroup;
    [SerializeField] private float _dragAlpha = 0.6f;
    private float _defaultAlpha;

    private DropArea _mostRecentHover;
    public Transform mostRecentSlot { get; private set; }



    private void Awake()
    {

        _rectTransform = GetComponent<RectTransform>();

        _canvasGroup = GetComponent<CanvasGroup>();
        _defaultAlpha = _canvasGroup.alpha;

    }



    private void Start()
    {
        _rectTransform.localScale = new Vector3(1, 1, 1);
    }



    public void SetMostRecentSlot(Transform newParent)
    {
        mostRecentSlot = newParent;
        transform.SetParent(mostRecentSlot);
        LayoutRebuilder.ForceRebuildLayoutImmediate(newParent as RectTransform);
    }



    public void SetMostRecentHover(DropArea dropArea)
    {
        _mostRecentHover = dropArea;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {

        if(!isDraggable) return;

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = _dragAlpha;

        SetMostRecentSlot(transform.parent);

    }



    public void OnDrag(PointerEventData eventData)
    {

        if(!isDraggable) return;

        _rectTransform.position = new Vector3
        (
            eventData.position.x,
            eventData.position.y,
            _rectTransform.position.z
        );

    }



    public void OnEndDrag(PointerEventData eventData)
    {

        if(!isDraggable) return;

        if(_mostRecentHover == null)
        {
            SetMostRecentSlot(mostRecentSlot);
        }
        else
        {
            _mostRecentHover.SwapForThis(this);
        }

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = _defaultAlpha;

    }

}