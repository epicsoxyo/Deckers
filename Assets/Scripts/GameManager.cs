using System;

using UnityEngine;

using Deckers.Network;



namespace Deckers.Game
{

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

        private int _currentTurn = 1;
        private int _currentRound = 1;
        private GameState _currentGameState = GameState.STATE_WAITING_FOR_PLAYERS;
        private bool _waitingForAction = false;

        public event EventHandler OnGameStart;
        public event Action<int> OnRoundEnd;
        public event Action<int> OnTurnEnd;
        public event EventHandler OnWhiteWin;
        public event EventHandler OnRedWin;



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

            DeckersGameManager.Instance.OnEndTurn += OnAction;

            CaptureManager.Instance.onWhiteWin += WhiteWin;
            CaptureManager.Instance.onRedWin += RedWin;

        }



        public void StartGame()
        {
            _currentGameState = GameState.STATE_WHITE_CHECKERS;
            _waitingForAction = false;
            OnGameStart?.Invoke(this, EventArgs.Empty);
        }



        private void OnAction(object sender, EventArgs e)
        {
            _waitingForAction = false;
        }



        private void WhiteWin(object sender, EventArgs e)
        {

            // decide on what to do with the win

            _currentGameState = GameState.STATE_END_OF_GAME;
            OnWhiteWin?.Invoke(this, EventArgs.Empty);

        }

        private void RedWin(object sender, EventArgs e)
        {

            // decide on what to do with the win

            _currentGameState = GameState.STATE_END_OF_GAME;
            OnRedWin?.Invoke(this, EventArgs.Empty);

        }



        private void Update()
        {

            if(_waitingForAction) return;

            _waitingForAction = true;

            switch(_currentGameState)
            {
                case GameState.STATE_WAITING_FOR_PLAYERS:
                    if(DeckersNetworkManager.isOnline){ OnlineGameManager.Instance.StartGame(); }
                    else{ StartGame(); }
                    break;

                case GameState.STATE_START_OF_TURN:
                    _currentGameState = GameState.STATE_WHITE_CARDS;
                    DeckersGameManager.Instance.TriggerAbilities(GameState.STATE_START_OF_TURN);
                    break;

                case GameState.STATE_WHITE_CARDS:
                    _currentGameState = GameState.STATE_RED_CARDS;
                    if(CardsManager.Instance.WhiteCards > 0)
                    {
                        DeckersGameManager.Instance.BeginTurn(Team.TEAM_WHITE);
                    }
                    else { _waitingForAction = false; }
                    break;

                case GameState.STATE_RED_CARDS:
                    _currentGameState = GameState.STATE_MIDDLE_OF_TURN;
                    if(CardsManager.Instance.RedCards > 0)
                    {
                        DeckersGameManager.Instance.BeginTurn(Team.TEAM_RED);
                    }
                    else { _waitingForAction = false; }
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
                    if(_currentTurn++ % 3 == 0)
                    {
                        _currentGameState = GameState.STATE_START_OF_TURN;
                        _currentRound++;
                        OnRoundEnd?.Invoke(_currentRound);
                    }
                    else{ _currentGameState = GameState.STATE_WHITE_CHECKERS; }
                    OnTurnEnd?.Invoke(_currentTurn);
                    DeckersGameManager.Instance.TriggerAbilities(GameState.STATE_END_OF_TURN);
                    break;

                case GameState.STATE_END_OF_GAME:
                    DeckersGameManager.Instance.TriggerAbilities(GameState.STATE_END_OF_GAME);
                    break;

            }

        }

    }

}