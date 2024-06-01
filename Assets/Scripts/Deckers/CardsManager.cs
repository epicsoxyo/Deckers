using System.Collections.Generic;

using UnityEngine;

using Deckers.Game;
using Deckers.Network;



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

    public int WhiteCards { get; private set; }
    public int RedCards { get; private set; }

    public List<Transform> RedCardSlots = new List<Transform>();
    public List<Transform> WhiteCardSlots = new List<Transform>();

    public Dictionary<int, Card> CardsInPlay { get; private set; }



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of CapturedPiecesManager detected!");
            return;
        }

        Instance = this;

        CardsInPlay = new Dictionary<int, Card>();

    }



    private void Start()
    {
        LocalGameManager.Instance.OnGameStart += SetLocalSlots;
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
            RedCardSlots.Add(slot);
            slot.GetComponent<DropArea>().draggableElementType = DraggableElementType.DRAGGABLE_RED_CARD;
        }
        foreach(Transform slot in lowerCards)
        {
            WhiteCardSlots.Add(slot);
            slot.GetComponent<DropArea>().draggableElementType = DraggableElementType.DRAGGABLE_WHITE_CARD;
        }

    }



    public void SwapLocalSlots()
    {

        foreach(Transform slot in lowerCards)
        {
            WhiteCardSlots.Add(slot);
            slot.GetComponent<DropArea>().draggableElementType = DraggableElementType.DRAGGABLE_WHITE_CARD;
        }
        for(int i = upperCards.childCount - 1; i >= 0; i--)
        {
            Transform slot = upperCards.GetChild(i);
            RedCardSlots.Add(slot);
            slot.SetParent(lowerCards);
            slot.GetComponent<DropArea>().draggableElementType = DraggableElementType.DRAGGABLE_RED_CARD;
        }
        foreach(Transform slot in WhiteCardSlots)
        {
            slot.SetParent(upperCards);
        }

    }



    public void DrawRandomCard(Team team)
    {

        int index = CardsDealer.Instance.GetRandomCardIndex();

        if(DeckersNetworkManager.isOnline)
        {
            OnlineGameManager.Instance.Deckers_DrawCard(team, index);
            return;
        }

        LocalDrawRandomCard(team, index);

    }

    public void LocalDrawRandomCard(Team team, int index)
    {

        List<Transform> slots;

        switch(team)
        {
            case Team.TEAM_WHITE:
                WhiteCards++;
                slots = WhiteCardSlots;
                break;
            case Team.TEAM_RED:
                RedCards++;
                slots = RedCardSlots;
                break;
            default:
                Debug.LogError("Attempted to draw card but team was not given!");
                return;
        }

        if(!TryGetEmptySlot(slots, out Transform slot))
        {
            Debug.LogWarning("Attempted to draw card but there were no available slots!");
            return;
        }

        Card card = CardsDealer.Instance.GetNewCardFromIndex(index);

        card.team = team;
        card.transform.SetParent(slot);

        CardsInPlay[card.CardId] = card;

        card.OnDraw();

    }



    public void LocalGiveCardToPlayer(Team team, ref Card card)
    {

        List<Transform> slots;

        switch(team)
        {
            case Team.TEAM_WHITE:
                WhiteCards++;
                slots = WhiteCardSlots;
                break;
            case Team.TEAM_RED:
                RedCards++;
                slots = RedCardSlots;
                break;
            default:
                Debug.LogError("Attempted to give card to player but team was not given!");
                return;
        }

        if(!TryGetEmptySlot(slots, out Transform slot)) { return; }

        card.team = team;
        card.transform.SetParent(slot);

        CardsInPlay[card.CardId] = card;

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

        foreach(Card card in CardsInPlay.Values)
        {
            if(hideGroup != CardsGroup.GROUP_NULL)
            {
                card.Hidden = (card.team == Team.TEAM_WHITE) ? whiteIsHidden : redIsHidden;
            }
            if(activeGroup != CardsGroup.GROUP_NULL)
            {
                card.Active = (card.team == Team.TEAM_WHITE) ? whiteIsActive : redIsActive;
            }
        }

    }



    public void ClearPlayArea()
    {

        playArea.GetComponent<CardDropArea>().ClearDescription();

        if(playArea.childCount == 0) return;

        Card activeCard =  playArea.GetChild(0).GetComponent<Card>();

        List<Transform> slots = (activeCard.team == Team.TEAM_WHITE)
        ? WhiteCardSlots
        : RedCardSlots;

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

        switch(card.team)
        {
            case Team.TEAM_WHITE:
                WhiteCards--;
                break;
            case Team.TEAM_RED:
                RedCards--;
                break;
            default:
                Debug.LogWarning("Consumed card does not have an assigned team.");
                break;
        }

        CardsInPlay.Remove(card.CardId);

        return card.CardId;

    }

}