using System;

using UnityEngine;
using UnityEngine.UI;



public class DeckersGameManager : MonoBehaviour
{

    public static DeckersGameManager Instance { get; private set; }

    [Header("Buttons")]
    [SerializeField] private Button skipButton;
    [SerializeField] private Button playButton;

    // current turn
    private Team _currentPlayer;

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

        skipButton.onClick.AddListener(() => onEndTurn.Invoke(this, EventArgs.Empty));

    }



    public void BeginTurn(Team player)
    {

        _currentPlayer = player;

        UpdateActiveCards();

        if(!DeckersNetworkManager.isOnline
        || player == OnlineGameManager.Instance.localTeam)
        {
            ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_CHECKERS);
            return;
        }

        ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_EMPTY);

    }



    private void UpdateActiveCards()
    {

    }

}