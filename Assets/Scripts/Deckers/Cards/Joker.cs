using UnityEngine;
using UnityEngine.UI;

using Deckers.Game;



public class Joker : Card
{

    private static Transform _choiceSlot;
    private static Transform[] _optionSlots = new Transform[3];
    private static Button _selectButton;
    private static bool _isInitialised;



    private void Start()
    {

        if(_isInitialised) { return; }

        Transform overlay = OverlayManager.Instance.GetOverlay(Overlay.Joker).transform;

        DropArea[] dropAreas = overlay.GetComponentsInChildren<DropArea>();
        _choiceSlot = dropAreas[0].transform;
        for(int i = 0; i < 3; i++){ _optionSlots[i] = dropAreas[i + 1].transform; }

        _selectButton = overlay.GetComponentInChildren<Button>();
        _selectButton.onClick.AddListener(OnSelectCard);

        _isInitialised = true;

    }



    public override void OnPlay()
    {

        OverlayManager.Instance.ToggleOverlayOn(Overlay.Joker);

        foreach(Transform slot in _optionSlots)
        {
            int index = CardsDealer.Instance.GetRandomCardIndex(CardsPool.POOL_JOKER);

            Card card = CardsDealer.Instance.GetNewCardFromIndex(index, slot, CardsPool.POOL_JOKER);

            card.Active = true;
        }

    }



    private void OnSelectCard()
    {

        // BUG: make new card invisible for the other player in online games.

        if(!_choiceSlot.GetChild(0).TryGetComponent(out Card selectedCard)){ return; }

        Debug.Log("Card id" + CardId);
        Debug.Log("Joker team " + _team);
        CardsManager.Instance.LocalGiveCardToPlayer(Team, ref selectedCard);

        // Destroy(selectedCard.gameObject);

        OverlayManager.Instance.ToggleOverlayOn(Overlay.Joker, isOn: false);
        DeckersGameManager.Instance.EndTurn();

        _choiceSlot.GetComponent<CardDropArea>().ClearDescription();

        foreach(Transform slot in _optionSlots)
        {
            if(slot.childCount == 0){ continue; }
            Destroy(slot.GetChild(0).gameObject);
        }

    }

}