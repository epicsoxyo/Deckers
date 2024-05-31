using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;



public class Board : MonoBehaviour
{

    public static Board Instance { get; private set;}

    public Transform playableArea;
    [SerializeField] private GameObject _gridSquarePrefab;

    [SerializeField] private GameObject _whitePiecePrefab;
    [SerializeField] private GameObject _redPiecePrefab;

    public static Dictionary<(int, int), GridSquare> grid { get; private set; }
    public static Dictionary<int, GamePiece> gamePieces { get; private set; }



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of BoardManager detected!");
            return;
        }
        Instance = this;

        grid = new Dictionary<(int, int), GridSquare>();
        gamePieces = new Dictionary<int, GamePiece>();

    }



    private void Start()
    {
        LocalGameManager.Instance.onGameStart += OnGameStart;
    }



    private void OnGameStart(object sender, EventArgs e)
    {

        GamePiece.CurrentId = 0; // enforces both players have same piece IDs

        GenerateBoard();
        InitialiseGamePieces();

        if(!DeckersNetworkManager.isOnline) return;

        if(OnlineGameManager.Instance.localTeam == Team.TEAM_RED)
        {
            playableArea.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.LowerRight;
        }

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
            gamePiece.Player = Team.TEAM_WHITE;
            gamePieces[gamePiece.Id] = gamePiece;

            int x = (2 * i) % 8 - ((i > 3) && (i < 8) ? 1 : 0) + 2;
            int y = (i / 4) + 1;

            gamePiece.transform.SetParent(grid[(x, y)].GetComponent<RectTransform>());
        }

        for(int i = 0; i < 12; i++)
        {
            GamePiece gamePiece = Instantiate(_redPiecePrefab).GetComponent<GamePiece>();
            gamePiece.Player = Team.TEAM_RED;
            gamePieces[gamePiece.Id] = gamePiece;

            int x = (i * 2) % 8 - (!((i > 3) && (i < 8)) ? 1 : 0) + 2;
            int y = (i / 4) + 6;

            gamePiece.transform.SetParent(grid[(x, y)].GetComponent<RectTransform>());
        }

    }



    public static GamePiece FindPiece(int pieceId)
    {

        GamePiece foundPiece = null;

        if(gamePieces.ContainsKey(pieceId))
        {
            foundPiece = gamePieces[pieceId];
        }

        if(foundPiece == null)
        {
            Debug.LogError("Could not find piece with id" + pieceId);
            return null;
        }

        return foundPiece;

    }



    public static bool TryGetNextSquare(Move move, GridSquare currentSquare, out GridSquare nextSquare)
    {

        int deltaX = move.x;
        int deltaY = move.y;

        Team currentPlayer = CheckersGameManager.Instance.CurrentPlayer;

        nextSquare = null;

        if(currentPlayer == Team.TEAM_RED) // flip direction
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

}