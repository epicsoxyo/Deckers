using UnityEngine;
using UnityEngine.UI;



public abstract class Card : MonoBehaviour
{

    public static int _currentId;
    public int cardId { get; private set; }

    private Image _cardImage;
    private Button _cardButton;
    private DraggableElement _dragComponent;

    [SerializeField] private Sprite cardFront;
    [SerializeField] private Sprite cardBack;
    [SerializeField] private CardInfo description;

    private Team _team;
    public Team team
    {
        get { return _team; }
        set
        {
            _team = value;

            DraggableElement dragComponent = GetComponent<DraggableElement>();

            dragComponent.draggableElementType = (team == Team.TEAM_WHITE)
            ? DraggableElementType.DRAGGABLE_WHITE_CARD
            : DraggableElementType.DRAGGABLE_RED_CARD;
        }
    }

    public bool hidden{ set{ _cardImage.sprite = value ? cardBack : cardFront; } }
    public bool active
    {
        set
        {
            _cardButton.interactable = value;
            _dragComponent.isDraggable = value;
        }
    }



    protected virtual void Awake()
    {

        cardId = _currentId++;

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