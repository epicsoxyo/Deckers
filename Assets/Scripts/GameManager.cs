using System;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



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
    public bool isOnline = false;

    private int _currentTurn = 1;
    private GameState _currentGameState = GameState.STATE_WAITING_FOR_PLAYERS;
    private bool _waitingForAction = true;

    [SerializeField] private Canvas mainMenuScreen;

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

        // whiteWinScreen.GetComponentInChildren<Button>().onClick.AddListener(Rematch);
        // redWinScreen.GetComponentInChildren<Button>().onClick.AddListener(Rematch);

    }



    public void StartGame(bool isOnlineGame)
    {

        isOnline = isOnlineGame;

        if(isOnline)
        {
            if(LobbyManager.Instance.currentLobby.Players.Count != 2) return;
            OnlineGameManager.Instance.StartGame();
            return;
        }

        StartGame();

    }

    public void StartGame()
    {
        mainMenuScreen.enabled = false;
        _currentGameState = GameState.STATE_WHITE_CHECKERS;
        _waitingForAction = false;
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
        _currentGameState = GameState.STATE_END_OF_GAME;
    }

    private void OnRedWin(object sender, EventArgs e)
    {
        _currentGameState = GameState.STATE_END_OF_GAME;
        redWinScreen.SetActive(true);
    }



    private void Rematch()
    {
        DeckersNetworkManager.Instance.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }



    private void Update()
    {

        // TODO: complete turns script

        if(_waitingForAction) return;

        switch(_currentGameState)
        {
            case GameState.STATE_WAITING_FOR_PLAYERS:
                break;

            case GameState.STATE_START_OF_TURN:
                // start of turn card effects in order they were played
                break;

            case GameState.STATE_WHITE_CARDS:
                // white play a card
                break;

            case GameState.STATE_RED_CARDS:
                // red play a card
                break;

            case GameState.STATE_MIDDLE_OF_TURN:
                // middle of turn card effects in order they were played
                break;

            case GameState.STATE_WHITE_CHECKERS:
                // white play checkers
                CheckersGameManager.Instance.BeginTurn(Team.TEAM_WHITE);
                break;

            case GameState.STATE_RED_CHECKERS:
                // red play checkers
                CheckersGameManager.Instance.BeginTurn(Team.TEAM_RED);
                break;

            case GameState.STATE_END_OF_TURN:
                // end of turn card effects
                // check for a win
                // if someone has won, do on win card effects
                // if neither player won yet, increment turn
                break;

            case GameState.STATE_END_OF_GAME:
                // ask if players want to play the game again
                // if yes, reload scene
                // if no, load main menu
                break;

        }

        _waitingForAction = true;

    }

}