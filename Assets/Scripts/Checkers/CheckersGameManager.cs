using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;



public class CheckersGameManager : MonoBehaviour
{

    public static CheckersGameManager Instance { get; private set; }

    [Header("Board Generation")]
    public Transform playableArea;
    [SerializeField] private GameObject _gridSquarePrefab;
    public Dictionary<(int, int), GridSquare> grid { get; private set; }

    [Header("Game Piece Generation")]
    [SerializeField] private GameObject _whitePiecePrefab;
    [SerializeField] private GameObject _redPiecePrefab;

    [Header("Skip Turn")]
    [SerializeField] private Button _skipTurnButton;

    // remaining game pieces
    public Dictionary<int, GamePiece> gamePieces { get; private set; }

    // current turn
    private Team _currentPlayer;
    private GamePiece _selectedPiece;
    private Dictionary<GridSquare, GamePiece> _availableMoves = new Dictionary<GridSquare, GamePiece>(); // square, captured piece
    private bool _isCapturing = false;

    // events
    public event EventHandler onWhiteActive;
    public event EventHandler onRedActive;
    public event EventHandler onEndTurn;



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of CheckersGameManager detected!");
            return;
        }

        Instance = this;

        grid = new Dictionary<(int, int), GridSquare>();

        gamePieces = new Dictionary<int, GamePiece>();

        _skipTurnButton.onClick.AddListener(EndTurn);

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
                gridSquareTransform.SetParent(playableArea);
            }
        }

    }



    private void InitialiseGamePieces()
    {

        for(int i = 0; i < 12; i++)
        {
            GamePiece gamePiece = Instantiate(_whitePiecePrefab).GetComponent<GamePiece>();
            gamePieces[gamePiece.id] = gamePiece;

            int x = (2 * i) % 8 - ((i > 3) && (i < 8) ? 1 : 0) + 2;
            int y = i / 4 + 1;

            gamePiece.transform.SetParent(grid[(x, y)].GetComponent<RectTransform>());
        }

        for(int i = 0; i < 12; i++)
        {
            GamePiece gamePiece = Instantiate(_redPiecePrefab).GetComponent<GamePiece>();
            gamePieces[gamePiece.id] = gamePiece;

            int x = (i * 2) % 8 - (!((i > 3) && (i < 8)) ? 1 : 0) + 2;
            int y = (i / 4) + 6;

            gamePiece.transform.SetParent(grid[(x, y)].GetComponent<RectTransform>());
        }

    }



    public void BeginTurn(Team player)
    {

        _currentPlayer = player;

        UpdateActivePieces();

        if(!DeckersNetworkManager.isOnline
        || player == OnlineGameManager.Instance.localTeam)
        {
            ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_CHECKERS);
            return;
        }

        ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_EMPTY);

    }



    private void UpdateActivePieces()
    {

        switch(_currentPlayer)
        {
            case Team.TEAM_WHITE:
                onWhiteActive?.Invoke(this, EventArgs.Empty);
                return;
            case Team.TEAM_RED:
                onRedActive?.Invoke(this, EventArgs.Empty);
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

        if(_selectedPiece == null) return;

        Transform selectedPieceParent = _selectedPiece.transform.parent;
        if(selectedPieceParent == null
        || !selectedPieceParent.TryGetComponent(out GridSquare currentSquare))
        {
            return;
        }

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

    }



    private bool TryGetNextSquare(Move move, GridSquare currentSquare, out GridSquare nextSquare)
    {

        int deltaX = move.x;
        int deltaY = move.y;

        nextSquare = null;

        if(_currentPlayer == Team.TEAM_RED) // flip direction
        {
            deltaX *= -1;
            deltaY *= -1;
        }

        int nextX = currentSquare.x + deltaX;
        int nextY = currentSquare.y + deltaY;

        if(!grid.ContainsKey((nextX, nextY))) return false;

        nextSquare = grid[(nextX, nextY)];
        return true;

    }



    public void MoveSelectedPiece(GridSquare destination)
    {

        bool isOnline = DeckersNetworkManager.isOnline;
        OnlineGameManager online = OnlineGameManager.Instance;

        if(isOnline)
        {
            online.Checkers_MovePiece(_selectedPiece.id, destination.x, destination.y);
        }
        else
        {
            MovePiece(_selectedPiece.id, destination.x, destination.y);
        }

        // promotion

        if((_currentPlayer == Team.TEAM_WHITE && destination.y == 8)
        || (_currentPlayer == Team.TEAM_RED && destination.y == 1))
        {
            if(isOnline)
            {
                online.Checkers_PromotePiece(_selectedPiece.id);
            }
            else
            {
                PromotePiece(_selectedPiece.id);
            }
        }

        // capture

        GamePiece capturedPiece = _availableMoves[destination];
        if(capturedPiece == null)
        {
            EndTurn();
            return;
        }

        if(isOnline)
        {
            online.Checkers_CapturePiece(capturedPiece.id);
            return;
        }

        CapturePiece(capturedPiece.id);

        foreach(GamePiece piece in gamePieces.Values)
        {
            piece.SetActive(piece == _selectedPiece);
        }

        DisplayAvailableMoves(_selectedPiece);

    }

    private GamePiece FindPiece(int pieceId)
    {

        GamePiece foundPiece = null;

        if(gamePieces.ContainsKey(pieceId)) foundPiece = gamePieces[pieceId];

        if(foundPiece == null)
        {
            Debug.LogError("Could not find piece with id" + pieceId);
            return null;
        }

        return foundPiece;

    }

    public void MovePiece(int pieceId, int destinationX, int destinationY)
    {
        GamePiece pieceToMove = FindPiece(pieceId);
        GridSquare destination = grid[(destinationX, destinationY)];
        pieceToMove.transform.SetParent(destination.transform);
    }

    public void PromotePiece(int pieceId)
    {
        GamePiece pieceToPromote = FindPiece(pieceId);
        pieceToPromote.Promote();
    }

    public void CapturePiece(int pieceId)
    {

        GamePiece capturedPiece = FindPiece(pieceId);

        _isCapturing = true;

        if(gamePieces.ContainsKey(capturedPiece.id))
        {
            gamePieces.Remove(capturedPiece.id);
        }

        capturedPiece.Capture();

        foreach(GamePiece piece in gamePieces.Values)
        {
            piece.SetActive(piece == _selectedPiece);
        }

        DisplayAvailableMoves(_selectedPiece);

    }



    public void EndTurn()
    {

        if(DeckersNetworkManager.isOnline)
        {
            OnlineGameManager.Instance.Checkers_EndTurn();
            return;
        }

        LocalEndTurn();

    }

    public void LocalEndTurn()
    {

        ClearCurrentSelection();

        if(_selectedPiece != null)
        {
            _selectedPiece.SetActive(false);
            _selectedPiece = null;
        }

        _isCapturing = false;

        onEndTurn?.Invoke(this, EventArgs.Empty);

    }

}