using System;

using UnityEngine;



public enum Team : byte
{
    TEAM_NULL = 0x00,
    TEAM_WHITE = 0x01,
    TEAM_RED = 0x02,
}



public enum GameState
{
    STATE_WAITING_FOR_PLAYERS,
    STATE_START_OF_TURN,
    STATE_WHITE_CHECKERS,
    STATE_RED_CHECKERS,
    STATE_MIDDLE_OF_TURN,
    STATE_WHITE_CARDS,
    STATE_RED_CARDS,
    STATE_END_OF_TURN,
    STATE_END_OF_GAME,
}



public class LocalGameManager : MonoBehaviour
{

    public static LocalGameManager Instance { get; private set;}

    private int _currentTurn = 0;
    private GameState _currentGameState = GameState.STATE_WAITING_FOR_PLAYERS;
    private bool _waitingForAction = false;

    public event EventHandler onGameStart;
    public event EventHandler onWhiteWin;
    public event EventHandler onRedWin;



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of GameManager detected!");
            return;
        }

        Instance = this;

    }



    private void Start()
    {

        CheckersGameManager.Instance.onEndTurn += OnAction;

        DeckersGameManager.Instance.onEndTurn += OnAction;

        CaptureManager.Instance.onWhiteWin += OnWhiteWin;
        CaptureManager.Instance.onRedWin += OnRedWin;

        if(DeckersNetworkManager.isOnline){ OnlineGameManager.Instance.StartGame(); }
        else{ StartGame(); }

    }



    public void StartGame()
    {

        _currentGameState = GameState.STATE_WHITE_CHECKERS;
        _waitingForAction = false;

        onGameStart?.Invoke(this, EventArgs.Empty);

    }



    private void OnAction(object sender, EventArgs e)
    {
        _waitingForAction = false;
    }



    private void OnWhiteWin(object sender, EventArgs e)
    {

        // decide on what to do with the win

        _currentGameState = GameState.STATE_END_OF_GAME;
        onWhiteWin.Invoke(this, EventArgs.Empty);

    }

    private void OnRedWin(object sender, EventArgs e)
    {

        // decide on what to do with the win

        _currentGameState = GameState.STATE_END_OF_GAME;
        onRedWin.Invoke(this, EventArgs.Empty);

    }



    private void Update()
    {

        if(_waitingForAction) return;

        _waitingForAction = true;

        switch(_currentGameState)
        {
            case GameState.STATE_WAITING_FOR_PLAYERS:
                break;

            case GameState.STATE_START_OF_TURN:
                _currentGameState = GameState.STATE_WHITE_CARDS;
                DeckersGameManager.Instance.TriggerAbilities(GameState.STATE_START_OF_TURN);
                break;

            case GameState.STATE_WHITE_CARDS:
                _currentGameState = GameState.STATE_RED_CARDS;
                DeckersGameManager.Instance.BeginTurn(Team.TEAM_WHITE);
                break;

            case GameState.STATE_RED_CARDS:
                _currentGameState = GameState.STATE_MIDDLE_OF_TURN;
                DeckersGameManager.Instance.BeginTurn(Team.TEAM_RED);
                break;

            case GameState.STATE_MIDDLE_OF_TURN:
                _currentGameState = GameState.STATE_WHITE_CHECKERS;
                DeckersGameManager.Instance.TriggerAbilities(GameState.STATE_MIDDLE_OF_TURN);
                break;

            case GameState.STATE_WHITE_CHECKERS:
                _currentGameState = GameState.STATE_RED_CHECKERS;
                CheckersGameManager.Instance.BeginTurn(Team.TEAM_WHITE);
                break;

            case GameState.STATE_RED_CHECKERS:
                _currentGameState = GameState.STATE_END_OF_TURN;
                CheckersGameManager.Instance.BeginTurn(Team.TEAM_RED);
                break;

            case GameState.STATE_END_OF_TURN:
                _currentGameState = (++_currentTurn % 3 == 0) ?
                    GameState.STATE_START_OF_TURN : 
                    GameState.STATE_WHITE_CHECKERS;
                DeckersGameManager.Instance.TriggerAbilities(GameState.STATE_END_OF_TURN);
                break;

            case GameState.STATE_END_OF_GAME:
                DeckersGameManager.Instance.TriggerAbilities(GameState.STATE_END_OF_GAME);
                break;

        }

    }

}