using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



// TODO: if a donkey is created, keep a track of the donkey (and therefore its team attribute) in a variable.


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

    private List<Card> activeCards = new List<Card>();

    // events
    public event EventHandler onEndTurn;



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of DeckersGameManager detected!");
            return;
        }

        Instance = this;

        skipButton.onClick.AddListener(EndTurn);
        playButton.onClick.AddListener(PlayCard);

    }



    private void Start()
    {
        Card._currentId = 0;
        LocalGameManager.Instance.onGameStart += InstantiateCards;
    }



    private void InstantiateCards(object sender, EventArgs e)
    {

        if(DeckersNetworkManager.isOnline && !DeckersNetworkManager.Instance.IsHost) return;

        for(int i = 0; i < 3; i++)
        {
            CardsManager.Instance.DrawCard(Team.TEAM_WHITE);
            CardsManager.Instance.DrawCard(Team.TEAM_RED);
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

        Team localTeam = OnlineGameManager.Instance.localTeam;

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

        if(!DeckersNetworkManager.isOnline) return;

        bool showPlayArea = (OnlineGameManager.Instance.localTeam == _currentPlayer);

        playAreaCanvasGroup.alpha = showPlayArea ? 1 : 0;
        playAreaCanvasGroup.blocksRaycasts = showPlayArea;
        playAreaCanvasGroup.interactable = showPlayArea;

    }



    private void PlayCard()
    {

        if(dropArea.childCount == 0) return;

        Card card = dropArea.GetChild(0).GetComponent<Card>();

        if(!card.IsPlayable()) return;

        if(DeckersNetworkManager.isOnline)
        {
            OnlineGameManager.Instance.Deckers_PlayCard(card.cardId);
            return;
        }

        LocalPlayCard(card.cardId);

    }

    public void LocalPlayCard(int cardId)
    {

        Card card = CardsManager.Instance.cardsInPlay[cardId];

        CardsManager.Instance.Consume(card);

        activeCards.Add(card);
        card.OnPlay();

    }



    public void TriggerAbilities(GameState gameState)
    {

        switch(gameState)
        {
            case GameState.STATE_START_OF_TURN:
                break;
            case GameState.STATE_MIDDLE_OF_TURN:
                break;
            case GameState.STATE_END_OF_TURN:
                break;
            case GameState.STATE_END_OF_GAME:
                break;
        }

        onEndTurn?.Invoke(this, EventArgs.Empty);

    }



    public void EndTurn()
    {

        if(DeckersNetworkManager.isOnline)
        {
            OnlineGameManager.Instance.Deckers_EndTurn();
            return;
        }

        LocalEndTurn();

    }

    public void LocalEndTurn()
    {

        CardsManager.Instance.ClearPlayArea();

        UpdateCardsUI();

        onEndTurn?.Invoke(this, EventArgs.Empty);

    }

}