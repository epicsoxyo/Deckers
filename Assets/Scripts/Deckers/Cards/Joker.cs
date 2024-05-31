using System.Collections;

using UnityEngine;
using UnityEngine.UI;



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
            int index = CardsDealer.Instance.GetRandomCardIndex();

            Card card = CardsDealer.Instance.GetNewCardFromIndex(index, forceUseJokerPool: true);

            card.transform.SetParent(slot);

            card.Active = true;
        }

    }



    private void OnSelectCard()
    {

        // BUG: make new card invisible for the other player in online games.
        // BUG: joker does not clear cards after choosing one.

        if(!_choiceSlot.GetChild(0).TryGetComponent(out Card selectedCard)){ return; }

        CardsManager.Instance.LocalGiveCardToPlayer(team, ref selectedCard);

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