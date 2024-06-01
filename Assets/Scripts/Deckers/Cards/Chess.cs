using System;

using Deckers.Game;



public class Chess : Card
{

    public override void OnPlay()
    {
        PieceSelectionManager.Instance.UpdateActivePieces(team);
        GamePiece.onClick += SelectPiece;
    }



    private void SelectPiece(object sender, EventArgs e)
    {

        GamePiece gamePiece = sender as GamePiece;

        if((gamePiece == null)
        || (gamePiece.PieceType == GamePieceType.PIECE_BISHOP))
        {
            return;
        }

        gamePiece.PromoteToBishop();

        PieceSelectionManager.Instance.UpdateActivePieces(Team.TEAM_NULL);
        GamePiece.onClick -= SelectPiece;

        DeckersGameManager.Instance.EndTurn();

    }

}