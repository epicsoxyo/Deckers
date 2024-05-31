using UnityEngine;
using UnityEngine.UI;



public abstract class Card : MonoBehaviour
{

    public static int CurrentId;
    public int CardId { get; private set; }

    protected Image _cardImage;
    protected Button _cardButton;
    protected DraggableElement _dragComponent;

    [SerializeField] protected Sprite cardFront;
    [SerializeField] protected Sprite cardBack;
    [SerializeField] protected CardInfo description;

    protected Team _team;
    public Team team
    {
        get { return _team; }
        set
        {
            _team = value;

            DraggableElement dragComponent = GetComponent<DraggableElement>();

            switch(_team)
            {
                case Team.TEAM_WHITE:
                    dragComponent.draggableElementType = DraggableElementType.DRAGGABLE_WHITE_CARD;
                    break;
                case Team.TEAM_RED:
                    dragComponent.draggableElementType = DraggableElementType.DRAGGABLE_RED_CARD;
                    break;
                default:
                    dragComponent.draggableElementType = DraggableElementType.DRAGGABLE_DEFAULT;
                    break;
            }
        }
    }

    public bool Hidden{ set{ _cardImage.sprite = value ? cardBack : cardFront; } }
    public bool Active
    {
        set
        {
            _cardButton.interactable = value;
            _dragComponent.IsDraggable = value;
        }
    }



    protected virtual void Awake()
    {

        CardId = CurrentId++;

        _cardImage = GetComponent<Image>();
        _cardButton = GetComponent<Button>();
        _dragComponent = GetComponent<DraggableElement>();

        RectTransform rectTransform = transform as RectTransform;
        rectTransform.localScale = new Vector3(1, 1, 1);

    }



    private void OnClick()
    {
        // open info panel
    }



    public string GetName()
    {
        return description.cardName;
    }



    public string GetDescription()
    {
        return description.cardDescription;
    }



    public virtual bool IsPlayable() { return true; }

    public virtual void OnPlay() { DeckersGameManager.Instance.EndTurn(); }

    public virtual void OnDeckersTurnStart() {}

    public virtual void OnCheckersTurnStart() {}

    public virtual void OnTurnEnd() {}

    public virtual void OnGameEnd() {}

}