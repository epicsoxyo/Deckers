using System;
using System.Collections.Generic;

using UnityEngine;

using Deckers.Game;



public class PieceSelectionManager : MonoBehaviour
{

    public static PieceSelectionManager Instance { get; private set; }

    public GamePiece SelectedPiece { get; private set; }
    public Dictionary<GridSquare, GamePiece> AvailableMoves = new Dictionary<GridSquare, GamePiece>();

    public event Action OnWhiteActive;
    public event Action OnRedActive;
    public event Action OnNullActive;



    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of MoveSelector detected!");
            return;
        }
        Instance = this;
    }



    private void Start()
    {
        GamePiece.onClick += OnPieceClicked;
    }



    public void UpdateActivePieces(Team activeTeam)
    {
        switch(activeTeam)
        {
            case Team.TEAM_WHITE:
                OnWhiteActive?.Invoke();
                return;
            case Team.TEAM_RED:
                OnRedActive?.Invoke();
                return;
            case Team.TEAM_NULL:
                OnNullActive?.Invoke();
                return;
        }
    }



    public void UpdateActivePiece(GamePiece gamePiece)
    {
        foreach(GamePiece piece in Board.gamePieces.Values)
        {
            piece.SetActive(piece == gamePiece);
        }
    }



    private void OnPieceClicked(object sender, EventArgs e)
    {
        SelectedPiece = sender as GamePiece;
    }



    /// <summary>
    /// Triggers the visibility of all available moves on the board for the specified
    /// game piece.
    /// </summary>
    /// <param name="gamePiece">The piece for which to display available moves.</param>
    /// <param name="isCapturing">Whether or not the piece is selecting a move after capturing.</param>
    /// <returns>True if moves were found.</returns>
    public bool DisplayAvailableMoves(GamePiece gamePiece, bool isCapturing = false)
    {

        if(SelectedPiece == null){ return false; }

        ClearCurrentSelection();

        Transform selectedPieceParent = SelectedPiece.transform.parent;
        if(selectedPieceParent == null
        || !selectedPieceParent.TryGetComponent(out GridSquare currentSquare))
        {
            return false;
        }

        if(SelectedPiece.PieceType == GamePieceType.PIECE_BISHOP && !isCapturing)
        {
            DisplayBishopMoves(currentSquare, isCapturing);
        }
        else
        {
            DisplayCheckersMoves(currentSquare, isCapturing);
        }

        if(AvailableMoves.Count == 0)
        {
            ClearCurrentSelection();
            return false;
        }

        return true;

    }

    private void DisplayBishopMoves(GridSquare currentSquare, bool isCapturing)
    {

        foreach(Move move in SelectedPiece.MovesSet)
        {
            int i = 0;

            while(true)
            {

                i++;

                // edge of board
                if(!Board.TryGetNextSquare(i * move, currentSquare, out GridSquare nextSquare))
                {
                    break;
                }

                // nothing occupying the square
                if(nextSquare.transform.childCount == 0)
                {
                    if(!isCapturing)
                    {
                        nextSquare.DisplayAsAvailableMove();
                        AvailableMoves[nextSquare] = null;
                    }
                    continue;
                }

                // piece occupying square
                GamePiece occupyingPiece = nextSquare.transform.GetChild(0).GetComponent<GamePiece>();

                if((occupyingPiece.Player != SelectedPiece.Player) && !occupyingPiece.IsProtected)
                {
                    nextSquare.DisplayAsAvailableMove();
                    AvailableMoves[nextSquare] = occupyingPiece;
                }

                break;

            }
        }

    }

    private void DisplayCheckersMoves(GridSquare currentSquare, bool isCapturing)
    {

        foreach(Move move in SelectedPiece.MovesSet)
        {
            if(!Board.TryGetNextSquare(move, currentSquare, out GridSquare nextSquare))
            {
                continue;
            }

            if(nextSquare.transform.childCount == 0) // nothing occupying the square
            {
                if(!isCapturing)
                {
                    nextSquare.DisplayAsAvailableMove();
                    AvailableMoves[nextSquare] = null;
                }
                continue;
            }

            GamePiece occupyingPiece = nextSquare.transform.GetChild(0).GetComponent<GamePiece>();

            if((occupyingPiece.Player == SelectedPiece.Player) || occupyingPiece.IsProtected)
            {
                continue;
            }


            if(Board.TryGetNextSquare(move, nextSquare, out GridSquare skippedSquare)
            && skippedSquare.transform.childCount == 0)
            {
                skippedSquare.DisplayAsAvailableMove();
                AvailableMoves[skippedSquare] = occupyingPiece;
            }
        }

    }



    public void ClearCurrentSelection()
    {
        foreach(GridSquare square in AvailableMoves.Keys)
        {
            square.DisplayAsAvailableMove(false);
        }
        AvailableMoves.Clear();
    }

    

}