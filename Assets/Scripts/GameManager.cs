using System;

using UnityEngine;



public enum Team
{
    TEAM_NULL,
    TEAM_WHITE,
    TEAM_RED,
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

    private int _currentTurn = 1;
    private GameState _currentGameState = GameState.STATE_WAITING_FOR_PLAYERS;
    private bool _waitingForAction = true;

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

    }

    public void StartGame()
    {
        _currentGameState = GameState.STATE_WHITE_CHECKERS;
        _waitingForAction = false;
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

        // TODO: complete turns script

        if(_waitingForAction) return;

        switch(_currentGameState)
        {
            case GameState.STATE_WAITING_FOR_PLAYERS:
                _currentGameState = GameState.STATE_WHITE_CHECKERS;
                break;

            case GameState.STATE_START_OF_TURN:
                // start of turn card effects in order they were played
                _currentGameState = GameState.STATE_WHITE_CARDS;
                break;

            case GameState.STATE_WHITE_CARDS:
                DeckersGameManager.Instance.BeginTurn(Team.TEAM_WHITE);
                _currentGameState = GameState.STATE_RED_CARDS;
                _waitingForAction = true;
                break;

            case GameState.STATE_RED_CARDS:
                DeckersGameManager.Instance.BeginTurn(Team.TEAM_RED);
                _currentGameState = GameState.STATE_MIDDLE_OF_TURN;
                _waitingForAction = true;
                break;

            case GameState.STATE_MIDDLE_OF_TURN:
                // middle of turn card effects in order they were played
                _currentGameState = GameState.STATE_WHITE_CHECKERS;
                break;

            case GameState.STATE_WHITE_CHECKERS:
                CheckersGameManager.Instance.BeginTurn(Team.TEAM_WHITE);
                _currentGameState = GameState.STATE_RED_CHECKERS;
                _waitingForAction = true;
                break;

            case GameState.STATE_RED_CHECKERS:
                CheckersGameManager.Instance.BeginTurn(Team.TEAM_RED);
                _currentGameState = GameState.STATE_END_OF_TURN;
                _waitingForAction = true;
                break;

            case GameState.STATE_END_OF_TURN:
                // end of turn card effects in the order they were played
                _currentGameState = (++_currentTurn > 3) ?
                    GameState.STATE_START_OF_TURN : 
                    GameState.STATE_WHITE_CHECKERS;
                break;

            case GameState.STATE_END_OF_GAME:
                // end of game card effects (e.g. donkey)
                // determine a winner
                break;

        }

        // _waitingForAction = true;

    }

}