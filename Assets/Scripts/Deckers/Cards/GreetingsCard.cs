using System;

using Deckers.Game;



public class GreetingsCard : Card
{

    private GamePiece _birthdayPiece;

    public override void OnPlay()
    {
        PieceSelectionManager.Instance.UpdateActivePieces(team);
        GamePiece.onClick += SelectPiece;
    }



    private void SelectPiece(object sender, EventArgs e)
    {

        GamePiece gamePiece = sender as GamePiece;

        if(gamePiece == null){ return; }

        GamePiece.onClick -= SelectPiece;

        gamePiece.Protect(GamePieceOverlay.GreetingsCard);
        _birthdayPiece = gamePiece;

        LocalGameManager.Instance.OnRoundEnd += OnRoundEnd;

        PieceSelectionManager.Instance.UpdateActivePieces(Team.TEAM_NULL);
        DeckersGameManager.Instance.EndTurn();

    }

    private void OnRoundEnd(int obj)
    {
        LocalGameManager.Instance.OnRoundEnd -= OnRoundEnd;
        _birthdayPiece.Unprotect();
    }

}