using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;



public enum GamePieceType
{
    PIECE_NORMAL,
    PIECE_KING,
    PIECE_BISHOP,

    // add piece types here...
}



public class GamePiece : MonoBehaviour
{

    [Header("ID")]
    public static int CurrentId = 0;
    public int Id { get; private set; }

    [Header("Piece Info")]
    public Team Player;
    public GamePieceType PieceType;
    public List<Move> MovesSet
    {
        get
        {
            switch(PieceType)
            {
                case GamePieceType.PIECE_NORMAL:
                    return MonodirectionalMovesSet;
                case GamePieceType.PIECE_KING:
                    return OmnidirectionalMovesSet;
                case GamePieceType.PIECE_BISHOP:
                    return OmnidirectionalMovesSet;
            }

            Debug.LogError(PieceType.ToString() + " does not have an implemented moves set!");
            return new List<Move>();
        }
    }

    [Header("Piece Components")]
    [SerializeField] private Sprite normalImage;
    [SerializeField] private Sprite kingImage;
    [SerializeField] private Sprite bishopImage;
    [SerializeField] private Sprite capturedImage;
    private Image _image;
    private Button _gamePieceButton;

    public bool IsCaptured { get; private set; }

    public static event EventHandler onClick;



    private void Awake()
    {

        Id = CurrentId++;

        _gamePieceButton = GetComponent<Button>();
        _gamePieceButton.onClick.AddListener(OnPieceClicked);
        _gamePieceButton.interactable = false;

        _image = GetComponent<Image>();
        _image.sprite = normalImage;

        IsCaptured = false;

    }



    private void Start()
    {
        SelectionManager.Instance.onWhiteActive += OnWhiteActive;
        SelectionManager.Instance.onRedActive += OnRedActive;
        SelectionManager.Instance.onNullActive += OnNullActive;
        CheckersGameManager.Instance.onEndTurn += OnNullActive;
    }



    private void OnWhiteActive(object sender, EventArgs e)
    {
        if(IsCaptured) return;
        SetActive(Player == Team.TEAM_WHITE);
    }



    private void OnRedActive(object sender, EventArgs e)
    {
        if(IsCaptured) return;
        SetActive(Player == Team.TEAM_RED);
    }



    private void OnNullActive(object sender, EventArgs e)
    {
        if(IsCaptured) return;
        SetActive(false);
    }



    public void SetActive(bool active = true)
    {
        _gamePieceButton.interactable = active;
        _image.raycastTarget = active;
    }



    private void OnPieceClicked()
    {

        if(IsCaptured) return;

        if(DeckersNetworkManager.isOnline
        && OnlineGameManager.Instance.localTeam != Player)
        {
            return;
        }

        onClick?.Invoke(this, EventArgs.Empty);

    }



    public void Promote(bool demote = false)
    {
        PieceType = demote ? GamePieceType.PIECE_NORMAL : GamePieceType.PIECE_KING;
        _image.sprite = demote ? normalImage : kingImage;
    }

    public void PromoteToBishop(bool demote = false)
    {
        PieceType = demote ? GamePieceType.PIECE_NORMAL : GamePieceType.PIECE_BISHOP;
        _image.sprite = demote ? normalImage : bishopImage;
    }



    public void Capture(bool revert = false)
    {

        IsCaptured = !revert;
        _gamePieceButton.enabled = revert;
        _image.sprite = revert ? (PieceType == GamePieceType.PIECE_KING ? kingImage : normalImage) : capturedImage;

        if(revert){ return; }

        CaptureManager.Instance.Capture(this);

    }




    // moves sets

    public static readonly List<Move> MonodirectionalMovesSet = new List<Move>
    {
        new Move(-1, 1),    // top-left
        new Move(1, 1),     // top-right
    };

    public static readonly List<Move> OmnidirectionalMovesSet = new List<Move>
    {
        new Move(-1, 1),    // top-left
        new Move(1, 1),     // top-right
        new Move(-1, -1),   // bottom-left
        new Move(1, -1),    // bottom-right
    };

}