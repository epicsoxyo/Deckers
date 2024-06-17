using System.Collections.Generic;

using UnityEngine;

using Deckers.Game;
// using Deckers.Network;



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



    public void SwapLocalSlots()
    {

        // foreach(Transform slot in lowerCards)
        // {
        //     WhiteCardSlots.Add(slot);
        //     slot.GetComponent<DropArea>().draggableElementType = DraggableElementType.DRAGGABLE_WHITE_CARD;
        // }
        // for(int i = upperCards.childCount - 1; i >= 0; i--)
        // {
        //     Transform slot = upperCards.GetChild(i);
        //     RedCardSlots.Add(slot);
        //     slot.SetParent(lowerCards);
        //     slot.GetComponent<DropArea>().draggableElementType = DraggableElementType.DRAGGABLE_RED_CARD;
        // }
        // foreach(Transform slot in WhiteCardSlots)
        // {
        //     slot.SetParent(upperCards);
        // }

    }



    public void DrawRandomCard(Team team)
    {

        int index = CardsDealer.Instance.GetRandomCardIndex();

        // if(DeckersNetworkManager.isOnline)
        // {
        //     OnlineGameManager.Instance.Deckers_DrawCard(team, index);
        //     return;
        // }

        LocalDrawRandomCard(team, index);

    }

    public void LocalDrawRandomCard(Team team, int index)
    {

        Transform slot;

        switch(team)
        {
            case Team.TEAM_WHITE:
                Debug.Log("Drawing for white team");
                if(TryGetEmptySlot(WhiteCardSlots, out slot)) { WhiteCards++; }
                break;
            case Team.TEAM_RED:
                Debug.Log("Drawing for red team");
                if(TryGetEmptySlot(RedCardSlots, out slot)) { RedCards++; }
                break;
            default:
                Debug.LogError("Attempted to draw card but team was not given!");
                return;
        }

        Card card = CardsDealer.Instance.GetNewCardFromIndex(index, slot);

        card.Team = team;

        CardsInPlay[card.CardId] = card;

        card.OnDraw();

    }



    public void LocalGiveCardToPlayer(Team team, ref Card card)
    {

        Transform slot;

        switch(team)
        {
            case Team.TEAM_WHITE:
                WhiteCards++;
                if(!TryGetEmptySlot(WhiteCardSlots, out slot)) { return; }
                break;
            case Team.TEAM_RED:
                RedCards++;
                if(!TryGetEmptySlot(RedCardSlots, out slot)) { return; }
                break;
            default:
                Debug.LogError("Attempted to give card to player but team was not given!");
                return;
        }

        card.Team = team;
        card.transform.SetParent(slot);

        CardsInPlay[card.CardId] = card;

    }



    private bool TryGetEmptySlot(List<Transform> slots, out Transform selectedSlot)
    {

        foreach(Transform slot in slots)
        {
            if(slot.childCount == 0)
            {
                selectedSlot = slot;
                return true;
            }
            Debug.Log(slot.GetChild(0).name);
        }

        Debug.LogWarning("Slot not found!");
        selectedSlot = null;
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
                card.Hidden = (card.Team == Team.TEAM_WHITE) ? whiteIsHidden : redIsHidden;
            }
            if(activeGroup != CardsGroup.GROUP_NULL)
            {
                card.Active = (card.Team == Team.TEAM_WHITE) ? whiteIsActive : redIsActive;
            }
        }

    }



    public void ClearPlayArea()
    {

        playArea.GetComponent<CardDropArea>().ClearDescription();

        if(playArea.childCount == 0) return;

        Card activeCard =  playArea.GetChild(0).GetComponent<Card>();

        List<Transform> slots = (activeCard.Team == Team.TEAM_WHITE)
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



    public int Discard(Card card, bool playCard = false)
    {

        Debug.Log("DISCARDING " + card.name + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

        card.transform.SetParent(discardArea);
        Debug.Log("Setting parent of card to " + discardArea);

        switch(card.Team)
        {
            case Team.TEAM_WHITE:
                WhiteCards--;
                Debug.Log("team is white. whitecards: " + WhiteCards);
                break;
            case Team.TEAM_RED:
                RedCards--;
                Debug.Log("team is red. redcards: " + RedCards);
                break;
            default:
                Debug.LogWarning("Consumed card does not have an assigned team.");
                break;
        }

        Debug.Log("Removing card from cardsinplay.");
        CardsInPlay.Remove(card.CardId);

        if(playCard) {Debug.Log("Playing card."); card.OnPlay(); }

        return card.CardId;

    }

}