using UnityEngine;
using UnityEngine.UI;



public abstract class Card : MonoBehaviour
{

    private static int _currentId;
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

    protected bool _canBeUsed;
    public bool canBeUsed { get{ return _canBeUsed; } }

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
        _canBeUsed = true;
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



    public abstract void OnPlay();

    public abstract void OnDeckersTurnStart();

    public abstract void OnCheckersTurnStart();

    public abstract void OnTurnEnd();

    public abstract void OnGameEnd();

}