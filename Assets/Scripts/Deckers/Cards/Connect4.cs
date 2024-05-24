using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class Connect4 : Card
{

    private Transform currentSlot;


    private void Start()
    {

        Transform overlayTransform = OverlayManager.Instance.GetOverlay(Overlay.Connect4).transform;
        SelectionSlot[] slots = overlayTransform.GetComponentsInChildren<SelectionSlot>();
        currentSlot = slots[0].transform;

    }



    public override void OnPlay()
    {
        StartCoroutine("PlayConnect4");
    }

    private IEnumerator PlayConnect4()
    {

        OverlayManager.Instance.ToggleOverlayOn(Overlay.Connect4);
        SelectionSlot.onSlotPointerEnter += UpdateSlot;

        Team capturedPieceTeam = (team == Team.TEAM_WHITE) ? Team.TEAM_WHITE : Team.TEAM_RED;
        Transform piece = CaptureManager.Instance.Pop(capturedPieceTeam).transform;

        if(piece == null) yield break;

        bool hasSelectedSlot = false;

        while(!hasSelectedSlot)
        {

            if(piece.parent != currentSlot){ piece.SetParent(currentSlot); }

            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                hasSelectedSlot = true;
                SelectionSlot.onSlotPointerEnter -= UpdateSlot;
            }

            yield return null;
        }

        yield return new WaitForSeconds(3f);

        OverlayManager.Instance.ToggleOverlayOn(Overlay.Connect4, false);

        DeckersGameManager.Instance.EndTurn();

    }

    private void UpdateSlot(object sender, EventArgs e)
    {
        SelectionSlot slot = sender as SelectionSlot;
        if(slot == null) return;
        currentSlot = slot.transform;
    }




    public override void OnDeckersTurnStart() {}

    public override void OnCheckersTurnStart() {}

    public override void OnTurnEnd() {}

    public override void OnGameEnd() {}

}