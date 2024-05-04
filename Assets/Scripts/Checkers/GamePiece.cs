using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;



public enum GamePieceType
{
    PIECE_NORMAL,
    PIECE_KING,

    // add piece types here...
}



public class GamePiece : MonoBehaviour
{

    [Header("ID")]
    private static int _currentId = 0;
    public int id { get; private set; }

    [Header("Piece Info")]
    public Team player;
    public GamePieceType pieceType;
    public List<Move> movesSet
    {
        get
        {
            switch(pieceType)
            {
                case GamePieceType.PIECE_NORMAL:
                    return NormalMovesSet;
                case GamePieceType.PIECE_KING:
                    return KingMovesSet;
            }

            Debug.LogError(pieceType.ToString() + " does not have an implemented moves set!");
            return new List<Move>();
        }
    }

    [Header("Piece Components")]
    [SerializeField] private Sprite normalImage;
    [SerializeField] private Sprite kingImage;
    private Image _image;
    private Button _gamePieceButton;

    [Header("Piece Status")]
    private bool _isCaptured;



    private void Awake()
    {

        id = _currentId;
        _currentId++;

        _gamePieceButton = GetComponent<Button>();
        _gamePieceButton.onClick.AddListener(OnPieceClicked);
        _gamePieceButton.interactable = false;

        _image = GetComponent<Image>();
        _image.sprite = normalImage;

    }



    private void Start()
    {
        
        CheckersGameManager.Instance.onWhiteActive += OnWhiteBeginTurn;
        CheckersGameManager.Instance.onRedActive += OnRedBeginTurn;

    }



    private void OnWhiteBeginTurn(object sender, EventArgs e)
    {
        if(_isCaptured) return;
        _gamePieceButton.interactable = (player == Team.TEAM_WHITE);
    }



    private void OnRedBeginTurn(object sender, EventArgs e)
    {
        if(_isCaptured) return;
        _gamePieceButton.interactable = (player == Team.TEAM_RED);
    }



    public void SetActive(bool active)
    {
        _gamePieceButton.interactable = active;
    }



    private void OnPieceClicked()
    {
        if(_isCaptured) return;
        CheckersGameManager.Instance.OnPieceClicked(this);
    }



    public void Promote(bool demote = false)
    {
        pieceType = demote ? GamePieceType.PIECE_NORMAL : GamePieceType.PIECE_KING;
        _image.sprite = demote ? normalImage : kingImage;
    }



    public void Capture(bool revert = false, GridSquare gridSquare = null)
    {

        GetComponent<AspectRatioFitter>().enabled = revert;

        if(!revert) _gamePieceButton.interactable = true;
        _gamePieceButton.enabled = !revert;

        _isCaptured = !revert;

        if(revert)
        {
            if(gridSquare == null || gridSquare.transform.childCount > 0) return;

            transform.SetParent(gridSquare.transform);
            return;
        }

        CapturedPiecesManager.Instance.CapturePiece(this);

    }



    // moves sets

    public static readonly List<Move> NormalMovesSet = new List<Move>
    {
        new Move(-1, 1),    // top-left
        new Move(1, 1),     // top-right
    };

    public static readonly List<Move> KingMovesSet = new List<Move>
    {
        new Move(-1, 1),    // top-left
        new Move(1, 1),     // top-right
        new Move(-1, -1),   // bottom-left
        new Move(1, -1),    // bottom-right
    };

}