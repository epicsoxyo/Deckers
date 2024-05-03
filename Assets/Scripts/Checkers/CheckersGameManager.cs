using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CheckersGameManager : MonoBehaviour
{

    public static CheckersGameManager Instance { get; private set; }

    [Header("Board Generation")]
    [SerializeField] private Transform playableArea;
    [SerializeField] private GameObject gridSquarePrefab;
    public Dictionary<(int, int), RectTransform> grid { get; private set; }

    [Header("Remaining Game Pieces")]
    public int whiteCapturedPieces = 0;
    public Dictionary<int, GamePiece> whiteRemainingPieces { get; private set; }
    public int redCapturedPieces = 0;
    public Dictionary<int, GamePiece> redRemainingPieces { get; private set; }



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of CheckersGameManager detected!");
            return;
        }

        Instance = this;

        grid = new Dictionary<(int, int), RectTransform>();
        whiteRemainingPieces = new Dictionary<int, GamePiece>();
        redRemainingPieces = new Dictionary<int, GamePiece>();

        GenerateBoard();
        InitialisePlayers();

    }



    private void GenerateBoard()
    {

        for(int y = 8; y >= 1; y--)
        {
            for(int x = 1; x <= 8; x++)
            {
                RectTransform gridSquare = Instantiate(gridSquarePrefab).GetComponent<RectTransform>();
                gridSquare.SetParent(playableArea);
                grid[(x, y)] = gridSquare;

                // GridSquare gridSquare = Instantiate(gridSquarePrefab).GetComponent<GridSquare>();
                // gridSquare.transform.SetParent(playableArea);
                // gridSquare.x = x;
                // gridSquare.y = y;
            }
        }

    }



    private void InitialisePlayers()
    {

        // TODO: initialise players + get player piece dictionaries

        for(int i = 0; i < 12; i++)
        {
            GamePiece gamePiece = Instantiate(gamePiecePrefab).GetComponent<GamePiece>();
            whiteRemainingPieces[gamePiece.id] = gamePiece;

            int x = (2 * i) % 8 + 1 - ((i > 3) && (i < 8) ? 1 : 0);
            int y = i / 4;

            gamePiece.transform.SetParent(grid[(x, y)]);
        }

        for(int i = 0; i < 12; i++)
        {
            GamePiece gamePiece = Instantiate(gamePiecePrePrefab).GetComponent<GamePiece>();
            redRemainingPieces[gamePiece.id] = gamePiece;

            int x = (i * 2) % 8 + 1 - (!((i > 3) && (i < 8)) ? 1 : 0);
            int y = (i / 4) + 5;

            gamePiece.transform.SetParent(grid[(x, y)]);

        }

    }



    public void TakeTurn(Player player)
    {

        // TODO: checkers turn

        // return if local player is not the current player
        // enable clicking on your pieces
        // set 

    }



    // public List<GridSquare> GetAvailableMoves()
    // {

    //     // TODO: get available moves

    // }

}