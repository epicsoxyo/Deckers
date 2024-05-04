using System;
using System.Collections.Generic;

using UnityEngine;



public class CheckersGameManager : MonoBehaviour
{

    public static CheckersGameManager Instance { get; private set; }

    [Header("Board Generation")]
    [SerializeField] private Transform _playableArea;
    [SerializeField] private GameObject _gridSquarePrefab;
    public Dictionary<(int, int), GridSquare> grid { get; private set; }

    [Header("Game Piece Generation")]
    [SerializeField] private GameObject _whitePiecePrefab;
    [SerializeField] private GameObject _redPiecePrefab;

    // remaining game pieces
    public Dictionary<int, GamePiece> whiteRemainingPieces { get; private set; }
    public Dictionary<int, GamePiece> redRemainingPieces { get; private set; }

    // current turn
    private Player _currentPlayer;
    private GamePiece _selectedPiece;
    private Dictionary<GridSquare, GamePiece> _availableMoves = new Dictionary<GridSquare, GamePiece>(); // square, captured piece
    private bool _isCapturing = false;

    // events
    public event EventHandler onWhiteBeginTurn;
    public event EventHandler onRedBeginTurn;
    public event EventHandler onEndTurn;
    public event EventHandler onWhiteWin;
    public event EventHandler onRedWin;



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of CheckersGameManager detected!");
            return;
        }

        Instance = this;

        grid = new Dictionary<(int, int), GridSquare>();

        whiteRemainingPieces = new Dictionary<int, GamePiece>();
        redRemainingPieces = new Dictionary<int, GamePiece>();

        GenerateBoard();
        InitialiseGamePieces();

    }



    private void GenerateBoard()
    {

        for(int y = 8; y >= 1; y--)
        {
            for(int x = 1; x <= 8; x++)
            {
                GridSquare gridSquare = Instantiate(_gridSquarePrefab).GetComponent<GridSquare>();
                grid[(x, y)] = gridSquare;
                gridSquare.x = x;
                gridSquare.y = y;

                RectTransform gridSquareTransform = gridSquare.transform as RectTransform;
                gridSquareTransform.SetParent(_playableArea);
            }
        }

    }



    private void InitialiseGamePieces()
    {

        for(int i = 0; i < 12; i++)
        {
            GamePiece gamePiece = Instantiate(_whitePiecePrefab).GetComponent<GamePiece>();
            whiteRemainingPieces[gamePiece.id] = gamePiece;

            int x = (2 * i) % 8 - ((i > 3) && (i < 8) ? 1 : 0) + 2;
            int y = i / 4 + 1;

            gamePiece.transform.SetParent(grid[(x, y)].GetComponent<RectTransform>());
        }

        for(int i = 0; i < 12; i++)
        {
            GamePiece gamePiece = Instantiate(_redPiecePrefab).GetComponent<GamePiece>();
            redRemainingPieces[gamePiece.id] = gamePiece;

            int x = (i * 2) % 8 - (!((i > 3) && (i < 8)) ? 1 : 0) + 2;
            int y = (i / 4) + 6;

            gamePiece.transform.SetParent(grid[(x, y)].GetComponent<RectTransform>());
        }

    }



    public void BeginTurn(Player player)
    {

        _currentPlayer = player;

        switch(_currentPlayer)
        {
            case Player.PLAYER_WHITE:
                onWhiteBeginTurn?.Invoke(this, EventArgs.Empty);
                return;
            case Player.PLAYER_RED:
                onRedBeginTurn?.Invoke(this, EventArgs.Empty);
                return;
        }

    }


    public void OnPieceClicked(GamePiece gamePiece)
    {
        if(_isCapturing) return;
        DisplayAvailableMoves(gamePiece);
    }



    public void DisplayAvailableMoves(GamePiece gamePiece)
    {

        ClearCurrentSelection();

        _selectedPiece = gamePiece;
        GridSquare currentSquare = _selectedPiece.transform.parent.GetComponent<GridSquare>();

        foreach(Move move in _selectedPiece.movesSet)
        {
            if(!TryGetNextSquare(move, currentSquare, out GridSquare nextSquare)) continue;

            if(nextSquare.transform.childCount == 0) // nothing occupying the square
            {
                if(!_isCapturing)
                {
                    nextSquare.DisplayAsAvailableMove();
                    _availableMoves[nextSquare] = null;
                }
                continue;
            }

            GamePiece occupyingPiece = nextSquare.transform.GetChild(0).GetComponent<GamePiece>();

            if(occupyingPiece.player == _selectedPiece.player) continue; // cannot jump over own pieces
            
            if(!TryGetNextSquare(move, nextSquare, out GridSquare skippedSquare)) continue;

            if(skippedSquare.transform.childCount == 0) // nothing occupying square past occupied square
            {
                skippedSquare.DisplayAsAvailableMove();
                _availableMoves[skippedSquare] = occupyingPiece;
            }
        }

        if(_isCapturing && _availableMoves.Count == 0) EndTurn();

    }



    private void ClearCurrentSelection()
    {

        foreach((GridSquare square, bool _) in _availableMoves)
        {
            square.DisplayAsAvailableMove(false);
        }

        _availableMoves.Clear();
        _selectedPiece = null;

    }



    private bool TryGetNextSquare(Move move, GridSquare currentSquare, out GridSquare nextSquare)
    {

        int deltaX = move.x;
        int deltaY = move.y;

        nextSquare = null;

        if(_currentPlayer == Player.PLAYER_RED) // flip direction
        {
            deltaX *= -1;
            deltaY *= -1;
        }

        int nextX = currentSquare.x + deltaX;
        int nextY = currentSquare.y + deltaY;

        Debug.Log("NextX: " + nextX + "\nNextY: " + nextY);
        if(!grid.ContainsKey((nextX, nextY))) return false;

        nextSquare = grid[(nextX, nextY)];
        return true;

    }



    public void MoveSelectedPiece(GridSquare destination)
    {

        // move the piece

        _selectedPiece.transform.SetParent(destination.transform);


        // promotion

        if((_currentPlayer == Player.PLAYER_WHITE && destination.y == 8)
        || (_currentPlayer == Player.PLAYER_RED && destination.y == 1))
        {
            _selectedPiece.Promote();
        }


        // capture

        GamePiece capturedPiece = _availableMoves[destination];
        if(capturedPiece == null)
        {
            EndTurn();
            return;
        }

        _isCapturing = true;

        switch(_currentPlayer)
        {
            case Player.PLAYER_WHITE:
                redRemainingPieces.Remove(capturedPiece.id);
                break;
            case Player.PLAYER_RED:
                whiteRemainingPieces.Remove(capturedPiece.id);
                break;
        }

        capturedPiece.Capture();
        capturedPiece = null;

        foreach(GamePiece piece in (_currentPlayer == Player.PLAYER_WHITE) ? whiteRemainingPieces.Values : redRemainingPieces.Values)
        {
            piece.SetActive(piece == _selectedPiece);
        }

        DisplayAvailableMoves(_selectedPiece);

    }



    public void EndTurn()
    {

        ClearCurrentSelection();

        if(whiteRemainingPieces.Count == 0)
        {
            onRedWin?.Invoke(this, EventArgs.Empty);
            return;
        }
        if(redRemainingPieces.Count == 0)
        {
            onWhiteWin?.Invoke(this, EventArgs.Empty);
            return;
        }

        _isCapturing = false;

        onEndTurn?.Invoke(this, EventArgs.Empty);

    }

}