using System;
using UnityEngine;



public enum Player
{
    PLAYER_WHITE,
    PLAYER_RED,
    PLAYER_NONE,
}



public enum GameState
{
    STATE_WAITING_TO_START,
    STATE_START_OF_TURN,
    STATE_WHITE_CHECKERS,
    STATE_RED_CHECKERS,
    STATE_MIDDLE_OF_TURN,
    STATE_WHITE_CARDS,
    STATE_RED_CARDS,
    STATE_END_OF_TURN,
    STATE_END_OF_GAME,
}



public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set;}

    private int _currentTurn = 1;
    private GameState _currentGameState = GameState.STATE_WAITING_TO_START;
    private bool _waitingForAction = false;

    [SerializeField] private GameObject whiteWinScreen;
    [SerializeField] private GameObject redWinScreen;




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
        CheckersGameManager.Instance.onWhiteWin += OnWhiteWin;
        CheckersGameManager.Instance.onRedWin += OnRedWin;

        whiteWinScreen.SetActive(false);
        redWinScreen.SetActive(false);

    }



    private void OnAction(object sender, EventArgs e)
    {

        _currentGameState = (_currentGameState == GameState.STATE_WHITE_CHECKERS) ?
            GameState.STATE_RED_CHECKERS : GameState.STATE_WHITE_CHECKERS;

        _waitingForAction = false;

        // (if last action of the turn)
        _currentTurn++;

    }



    private void OnWhiteWin(object sender, EventArgs e)
    {
        whiteWinScreen.SetActive(true);
        _currentGameState = GameState.STATE_END_OF_GAME;
    }


    private void OnRedWin(object sender, EventArgs e)
    {
        redWinScreen.SetActive(true);
        _currentGameState = GameState.STATE_END_OF_GAME;
    }



    private void Update()
    {

        // TODO: complete turns script

        if(_waitingForAction) return;

        switch(_currentGameState)
        {
            case GameState.STATE_WAITING_TO_START:
                // if both players have joined and readied, set state to start of turn
                _currentGameState = GameState.STATE_WHITE_CHECKERS; // FOR TESTING
                return;

            case GameState.STATE_START_OF_TURN:
                // start of turn card effects in order they were played
                _waitingForAction = true;
                return;

            case GameState.STATE_WHITE_CARDS:
                // white play a card
                _waitingForAction = true;
                return;

            case GameState.STATE_RED_CARDS:
                // red play a card
                _waitingForAction = true;
                return;

            case GameState.STATE_MIDDLE_OF_TURN:
                // middle of turn card effects in order they were played
                _waitingForAction = true;
                return;

            case GameState.STATE_WHITE_CHECKERS:
                // white play checkers
                CheckersGameManager.Instance.BeginTurn(Player.PLAYER_WHITE);
                _waitingForAction = true;
                return;

            case GameState.STATE_RED_CHECKERS:
                // red play checkers
                CheckersGameManager.Instance.BeginTurn(Player.PLAYER_RED);
                _waitingForAction = true;
                return;

            case GameState.STATE_END_OF_TURN:
                // end of turn card effects
                // check for a win
                // if someone has won, do on win card effects
                // if neither player won yet, increment turn
                return;

            case GameState.STATE_END_OF_GAME:
                // ask if players want to play the game again
                // if yes, reload scene
                // if no, load main menu
                return;

        }

    }

}