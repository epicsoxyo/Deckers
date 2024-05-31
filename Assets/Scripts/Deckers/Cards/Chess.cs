using System;
using System.Collections;



public class Chess : Card
{

    private GamePiece _selectedPiece;

    public override void OnPlay()
    {
        StartCoroutine("ChooseBishop");
    }

    private IEnumerator ChooseBishop()
    {

        _selectedPiece = null;

        SelectionManager.Instance.UpdateActivePieces(team);
        GamePiece.onClick += SelectPiece;

        while (_selectedPiece == null) { yield return null; }

        _selectedPiece.PromoteToBishop();

        SelectionManager.Instance.UpdateActivePieces(Team.TEAM_NULL);
        GamePiece.onClick -= SelectPiece;

        DeckersGameManager.Instance.EndTurn();

    }

    private void SelectPiece(object sender, EventArgs e)
    {
        GamePiece gamePiece = sender as GamePiece;
        if(gamePiece == null) return;
        _selectedPiece = gamePiece;
    }

}