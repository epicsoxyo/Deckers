using System;

using UnityEngine;

using Deckers.Network;



namespace Deckers.Game
{

    public enum Team
    {
        TEAM_NULL,
        TEAM_WHITE,
        TEAM_RED,
    }



    public class LocalGameManager : MonoBehaviour
    {

        public static LocalGameManager Instance { get; private set;}

        private enum GameState
        {
            STATE_WAITING_FOR_PLAYERS,
            STATE_START_OF_TURN,
            STATE_WHITE_CHECKERS,
            STATE_RED_CHECKERS,
            // STATE_MIDDLE_OF_TURN,
            STATE_WHITE_CARDS,
            STATE_RED_CARDS,
            STATE_END_OF_TURN,
            STATE_END_OF_GAME,
        }
        private GameState _currentGameState = GameState.STATE_WAITING_FOR_PLAYERS;

        private bool _waitingForAction = false;

        private int _currentTurn = 1;
        private int _currentRound = 1;

        public event Action OnGameStart;
        public event Action<int> OnTurnStart; // (int turnNumber)
        public event Action<Team> OnCheckersStart; // (Team currentTeam)
        public event Action<Team> OnDeckersStart; // (Team currentTeam)
        public event Action<int> OnTurnEnd; // (int turnNumber)
        public event Action<int> OnRoundEnd; // (int roundNumber)
        public event Action<Team> OnWin; // (Team winningTeam)



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
            CheckersGameManager.Instance.OnEndTurn += OnAction;
            DeckersGameManager.Instance.OnEndTurn += OnAction;
            CaptureManager.Instance.OnCheckersWin += OnCheckersWin;
        }



        private void StartGame()
        {

            _waitingForAction = true;

            if(DeckersNetworkManager.isOnline)
            {
                OnlineGameManager.Instance.StartGame();
            }
            else{ LocalStartGame(); }

        }

        public void LocalStartGame()
        {
            OnGameStart?.Invoke();
            _waitingForAction = false;
        }



        private void StartTurn()
        {
            OnTurnStart?.Invoke(_currentTurn);
            // DeckersGameManager.Instance.TriggerAbilities(GameState.STATE_START_OF_TURN);
        }

        private void StartDeckersTurn(Team team)
        {
            if(CardsManager.Instance.WhiteCards > 0)
            {
                OnDeckersStart?.Invoke(team);
                _waitingForAction = true;
            }
            else { _waitingForAction = false; }
        }

        private void StartCheckersTurn(Team team)
        {
            OnCheckersStart?.Invoke(team);
            _waitingForAction = true;
        }

        private void EndTurn()
        {

            OnTurnEnd?.Invoke(_currentTurn);

            if(_currentTurn++ % 3 == 0)
            {
                _currentGameState = GameState.STATE_START_OF_TURN;
                OnRoundEnd?.Invoke(_currentRound++);
            }
            else
            {
                _currentGameState = GameState.STATE_WHITE_CHECKERS;
            }

            // DeckersGameManager.Instance.TriggerAbilities(GameState.STATE_END_OF_TURN);

        }



        private void OnAction()
        {
            _waitingForAction = false;
        }



        private void OnCheckersWin(Team winningTeam)
        {

            _currentGameState = GameState.STATE_END_OF_GAME;

            /*
                decide on what to do with the checkers win here
                (trigger donkey logic etc)
                for now this just triggers the win event.
            */

            OnWin?.Invoke(winningTeam);

        }



        private void Update()
        {

            if(_waitingForAction) return;

            switch(_currentGameState)
            {
                case GameState.STATE_WAITING_FOR_PLAYERS:
                    _currentGameState = GameState.STATE_WHITE_CHECKERS;
                    StartGame();
                    break;

                case GameState.STATE_START_OF_TURN:
                    _currentGameState = GameState.STATE_WHITE_CARDS;
                    StartTurn();
                    break;

                case GameState.STATE_WHITE_CARDS:
                    _currentGameState = GameState.STATE_RED_CARDS;
                    StartDeckersTurn(Team.TEAM_WHITE);
                    break;

                case GameState.STATE_RED_CARDS:
                    _currentGameState = GameState.STATE_WHITE_CHECKERS;
                    StartDeckersTurn(Team.TEAM_RED);
                    break;

                // case GameState.STATE_MIDDLE_OF_TURN:
                //     _currentGameState = GameState.STATE_WHITE_CHECKERS;
                //     DeckersGameManager.Instance.TriggerAbilities(GameState.STATE_MIDDLE_OF_TURN);
                //     break;

                case GameState.STATE_WHITE_CHECKERS:
                    _currentGameState = GameState.STATE_RED_CHECKERS;
                    StartCheckersTurn(Team.TEAM_WHITE);
                    break;

                case GameState.STATE_RED_CHECKERS:
                    _currentGameState = GameState.STATE_END_OF_TURN;
                    StartCheckersTurn(Team.TEAM_RED);
                    break;

                case GameState.STATE_END_OF_TURN:
                    EndTurn();
                    break;

                case GameState.STATE_END_OF_GAME:
                    _waitingForAction = true;
                    break;

            }

        }

    }

}