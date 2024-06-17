using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

// using Deckers.Network;



namespace Deckers.Game
{

    public class DeckersGameManager : MonoBehaviour
    {

        public static DeckersGameManager Instance { get; private set; }

        [Header("Play Area")]
        [SerializeField] private Transform dropArea;
        [SerializeField] private CanvasGroup playAreaCanvasGroup;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button playButton;

        // current turn
        private Team _currentPlayer;

        private List<Card> _activeCards = new List<Card>();

        // events
        public event Action OnEndTurn;



        private void Awake()
        {

            if(Instance != null)
            {
                Debug.LogWarning("Multiple instances of DeckersGameManager detected!");
                return;
            }

            Instance = this;

            skipButton.onClick.AddListener(EndTurn);
            playButton.onClick.AddListener(PlayCardInDropArea);

        }



        private void Start()
        {
            Card.CurrentId = 0;
            LocalGameManager.Instance.OnGameStart += InstantiateCards;
            LocalGameManager.Instance.OnDeckersStart += BeginTurn;
        }



        private void InstantiateCards()
        {

            // if(DeckersNetworkManager.isOnline && !DeckersNetworkManager.Instance.IsHost) return;

            for(int i = 0; i < 3; i++)
            {
                CardsManager.Instance.DrawRandomCard(Team.TEAM_WHITE);
                CardsManager.Instance.DrawRandomCard(Team.TEAM_RED);
            }

        }



        public void BeginTurn(Team player)
        {

            _currentPlayer = player;

            UpdateCardsUI();

            ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_DECKERS);

        }



        private void UpdateCardsUI()
        {

            // Team localTeam = OnlineGameManager.Instance.localTeam;
            Team localTeam = Team.TEAM_NULL;

            switch(_currentPlayer)
            {
                case Team.TEAM_WHITE:
                    if(localTeam == Team.TEAM_RED)
                    {
                        CardsManager.Instance.SelectCardGroup(hideGroup: CardsGroup.GROUP_WHITE, activeGroup: CardsGroup.GROUP_NONE);
                        break;
                    }
                    CardsManager.Instance.SelectCardGroup(hideGroup: CardsGroup.GROUP_RED, activeGroup: CardsGroup.GROUP_WHITE);
                    break;
                case Team.TEAM_RED:
                    if(localTeam == Team.TEAM_WHITE)
                    {
                        CardsManager.Instance.SelectCardGroup(hideGroup: CardsGroup.GROUP_RED, activeGroup: CardsGroup.GROUP_NONE);
                        break;
                    }
                    CardsManager.Instance.SelectCardGroup(hideGroup: CardsGroup.GROUP_WHITE, activeGroup: CardsGroup.GROUP_RED);
                    break;
            }

            // if(!DeckersNetworkManager.isOnline) return;

            // bool showPlayArea = (OnlineGameManager.Instance.localTeam == _currentPlayer);

            // playAreaCanvasGroup.alpha = showPlayArea ? 1 : 0;
            // playAreaCanvasGroup.blocksRaycasts = showPlayArea;
            // playAreaCanvasGroup.interactable = showPlayArea;

        }



        private void PlayCardInDropArea()
        {

            if(dropArea.childCount == 0) return;

            Card card = dropArea.GetChild(0).GetComponent<Card>();

            if(!card.IsPlayable())
            {
                // PRIORITY: add card error text
                Debug.Log("Card is unplayable.");
                return;
            }

            PlayCard(card);

        }

        public void PlayCard(Card card)
        {

            Debug.Log("Playing card " + card.name);

            // if(DeckersNetworkManager.isOnline)
            // {
            //     OnlineGameManager.Instance.Deckers_PlayCard(card.CardId);
            //     return;
            // }

            LocalPlayCard(card.CardId);

        }

        public void LocalPlayCard(int cardId)
        {

            Card card = CardsManager.Instance.CardsInPlay[cardId];

            if(card.IsPlayable()){ CardsManager.Instance.Discard(card, playCard: true); }

            _activeCards.Add(card);

        }



        // public void TriggerAbilities(GameState gameState)
        // {

        //     switch(gameState)
        //     {
        //         case GameState.STATE_START_OF_TURN:
        //             break;
        //         case GameState.STATE_MIDDLE_OF_TURN:
        //             break;
        //         case GameState.STATE_END_OF_TURN:
        //             break;
        //         case GameState.STATE_END_OF_GAME:
        //             break;
        //     }

        //     OnEndTurn?.Invoke(this, EventArgs.Empty);

        // }



        public void EndTurn()
        {

            // if(DeckersNetworkManager.isOnline)
            // {
            //     OnlineGameManager.Instance.Deckers_EndTurn();
            //     return;
            // }

            LocalEndTurn();

        }

        public void LocalEndTurn()
        {

            CardsManager.Instance.ClearPlayArea();

            UpdateCardsUI();

            OnEndTurn?.Invoke();

        }

    }

}