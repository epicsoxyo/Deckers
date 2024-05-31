using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;



public class Connect4 : Card
{

    private SelectionSlot _currentSlot;



    private void Start()
    {
        Transform overlayTransform = OverlayManager.Instance.GetOverlay(Overlay.Connect4).transform;
        _currentSlot = overlayTransform.GetComponentsInChildren<SelectionSlot>()[0];
    }



    public override bool IsPlayable()
    {

        CaptureManager captureManager = CaptureManager.Instance;
        switch(team)
        {
            case Team.TEAM_WHITE:
                return captureManager.redCapturedPieces > 0;
            case Team.TEAM_RED:
                return captureManager.whiteCapturedPieces > 0;
        }

        Debug.LogError("Card with unassigned team was used");
        return false;

    }



    public override void OnPlay()
    {
        StartCoroutine("PlayConnect4");
    }

    private IEnumerator PlayConnect4()
    {

        OverlayManager.Instance.ToggleOverlayOn(Overlay.Connect4);
        SelectionSlot.onSlotPointerEnter += UpdateSlot;

        CheckForFlipOverlay();

        Team capturedPieceTeam = (team == Team.TEAM_WHITE) ? Team.TEAM_RED : Team.TEAM_WHITE;
        GamePiece piece = CaptureManager.Instance.Pop(capturedPieceTeam);

        if(piece == null)
        {
            Debug.LogError("Connect4 card was played but there are no captured pieces!");
            yield break;
        }

        bool hasSelectedSlot = false;

        while(!hasSelectedSlot)
        {
            if(piece.transform.parent != _currentSlot)
            {
                piece.transform.SetParent(_currentSlot.transform);
            }

            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                hasSelectedSlot = true;
                SelectionSlot.onSlotPointerEnter -= UpdateSlot;
            }
            else { yield return null; }
        }

        DropPiece(piece);

        CheckForPromotion(piece);

        yield return new WaitForSeconds(1f);

        OverlayManager.Instance.ToggleOverlayOn(Overlay.Connect4, false);
        DeckersGameManager.Instance.EndTurn();

    }



    private void CheckForFlipOverlay()
    {

        Transform overlay = OverlayManager.Instance.GetOverlay(Overlay.Connect4).transform;

        bool isFlipped = DeckersNetworkManager.isOnline ?
            OnlineGameManager.Instance.localTeam != team :
            team == Team.TEAM_RED;
        
        overlay.transform.rotation = isFlipped ?
            Quaternion.Euler(0, 0, 180) :
            Quaternion.Euler(0, 0, 0);
        
        overlay.GetChild(0).GetComponent<HorizontalLayoutGroup>().reverseArrangement = isFlipped;

    }

    private void UpdateSlot(object sender, EventArgs e)
    {
        SelectionSlot slot = sender as SelectionSlot;
        if(slot == null) return;
        _currentSlot = slot;
    }

    private void DropPiece(GamePiece piece)
    {

        for(int i = 1; i <= 8; i++)
        {
            Transform currentSquare = Board.grid[(_currentSlot.index, i)].transform;
            
            if(currentSquare.childCount > 0)
            {
                currentSquare.GetChild(0).GetComponent<GamePiece>().Capture();
            }
        }

        int posY = (team == Team.TEAM_WHITE) ?
            (_currentSlot.index % 2) + 1 :
            7 + (_currentSlot.index % 2);

        piece.transform.SetParent(Board.grid[(_currentSlot.index, posY)].transform);

        piece.Capture(revert: true);

    }

    private void CheckForPromotion(GamePiece piece)
    {
        int promoteOddColumns = (team == Team.TEAM_RED) ? 1 : 0;
        if((_currentSlot.index % 2) == promoteOddColumns){ piece.Promote(); }
    }

}