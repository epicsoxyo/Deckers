using System.Collections.Generic;

using UnityEngine;



public enum CardsGroup
{
    GROUP_NULL,
    GROUP_NONE,
    GROUP_RED,
    GROUP_WHITE,
    GROUP_BOTH,
}



public class CardsManager : MonoBehaviour
{

    public static CardsManager Instance { get; private set; }

    [SerializeField] private Transform upperCards;
    [SerializeField] private Transform lowerCards;
    [SerializeField] private Transform playArea;
    [SerializeField] private Transform discardArea;

    private List<Transform> _redCardSlots = new List<Transform>();
    private List<Transform> _whiteCardSlots = new List<Transform>();

    [SerializeField] private List<GameObject> cardsPool = new List<GameObject>();
    [SerializeField] private List<GameObject> jokerPool = new List<GameObject>();

    public Dictionary<int, Card> cardsInPlay { get; private set; }



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of CapturedPiecesManager detected!");
            return;
        }

        Instance = this;

        cardsInPlay = new Dictionary<int, Card>();

    }



    private void Start()
    {
        LocalGameManager.Instance.onGameStart += SetLocalSlots;
    }



    private void SetLocalSlots(object sender, System.EventArgs e)
    {

        if(DeckersNetworkManager.isOnline)
        {
            if(!DeckersNetworkManager.Instance.IsHost) return;
            OnlineGameManager.Instance.Deckers_SwapLocalSlots();
        }

        foreach(Transform slot in upperCards)
        {
            _redCardSlots.Add(slot);
            slot.GetComponent<DropArea>().draggableElementType = DraggableElementType.DRAGGABLE_RED_CARD;
        }
        foreach(Transform slot in lowerCards)
        {
            _whiteCardSlots.Add(slot);
            slot.GetComponent<DropArea>().draggableElementType = DraggableElementType.DRAGGABLE_WHITE_CARD;
        }

    }



    public void SwapLocalSlots()
    {

        foreach(Transform slot in lowerCards)
        {
            _whiteCardSlots.Add(slot);
            slot.GetComponent<DropArea>().draggableElementType = DraggableElementType.DRAGGABLE_WHITE_CARD;
        }
        for(int i = upperCards.childCount - 1; i >= 0; i--)
        {
            Transform slot = upperCards.GetChild(i);
            _redCardSlots.Add(slot);
            slot.SetParent(lowerCards);
            slot.GetComponent<DropArea>().draggableElementType = DraggableElementType.DRAGGABLE_RED_CARD;
        }
        foreach(Transform slot in _whiteCardSlots)
        {
            slot.SetParent(upperCards);
        }

    }



    public void DrawCard(Team team)
    {

        int maxIndex = cardsPool.Count >= 0 ? cardsPool.Count : jokerPool.Count;
        int index = Random.Range(0, maxIndex);

        if(DeckersNetworkManager.isOnline)
        {
            OnlineGameManager.Instance.Deckers_DrawCard(team, index);
            return;
        }

        LocalDrawCard(team, index);

    }

    public void LocalDrawCard(Team team, int index)
    {

        Team localTeam = OnlineGameManager.Instance.localTeam;

        List<Transform> slots = (team == Team.TEAM_WHITE) ? _whiteCardSlots : _redCardSlots;

        if(!TryGetEmptySlot(slots, out Transform slot)) return;

        bool useJokerPool = (cardsPool.Count == 0);
        GameObject cardGameObject = useJokerPool ? jokerPool[index] : cardsPool[index];

        Card card = Instantiate(cardGameObject).GetComponent<Card>();
        card.team = team;
        card.transform.SetParent(slot);

        cardsInPlay[card.cardId] = card;

        if(!useJokerPool) cardsPool.RemoveAt(index);

    }



    private bool TryGetEmptySlot(List<Transform> slots, out Transform slot)
    {

        foreach(Transform t in slots)
        {
            if(t.childCount == 0)
            {
                slot = t;
                return true;
            }
        }

        slot = null;
        return false;

    }



    public void SelectCardGroup(CardsGroup hideGroup = CardsGroup.GROUP_NULL, CardsGroup activeGroup = CardsGroup.GROUP_NULL)
    {

        bool whiteIsHidden = (hideGroup == CardsGroup.GROUP_BOTH) || (hideGroup == CardsGroup.GROUP_WHITE);
        bool redIsHidden = (hideGroup == CardsGroup.GROUP_BOTH) || (hideGroup == CardsGroup.GROUP_RED);
        bool whiteIsActive = (activeGroup == CardsGroup.GROUP_BOTH) || (activeGroup == CardsGroup.GROUP_WHITE);
        bool redIsActive = (activeGroup == CardsGroup.GROUP_BOTH) || (activeGroup == CardsGroup.GROUP_RED);

        foreach(Card card in cardsInPlay.Values)
        {
            if(hideGroup != CardsGroup.GROUP_NULL)
            {
                card.hidden = (card.team == Team.TEAM_WHITE) ? whiteIsHidden : redIsHidden;
            }
            if(activeGroup != CardsGroup.GROUP_NULL)
            {
                card.active = (card.team == Team.TEAM_WHITE) ? whiteIsActive : redIsActive;
            }
        }

    }



    public void ClearPlayArea()
    {

        playArea.GetComponent<CardDropArea>().ClearDescription();

        if(playArea.childCount == 0) return;

        Card activeCard =  playArea.GetChild(0).GetComponent<Card>();

        List<Transform> slots = (activeCard.team == Team.TEAM_WHITE)
        ? _whiteCardSlots
        : _redCardSlots;

        Transform slot;

        if(TryGetEmptySlot(slots, out slot))
        {
            activeCard.transform.SetParent(slot);
        }
        else
        {
            Destroy(activeCard);
        }

    }



    public int Consume(Card card)
    {

        card.transform.SetParent(discardArea);
        cardsInPlay.Remove(card.cardId);
        return card.cardId;

    }

}