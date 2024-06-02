using System;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

using Deckers.Game;
using Deckers.Network;



public class WinScreenManager : MonoBehaviour
{

    [Header("Win Text")]
    [SerializeField] private TextMeshProUGUI whiteWinText;
    [SerializeField] private TextMeshProUGUI redWinText;

    [Header("Buttons")]
    [SerializeField] private Button rematchButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI waitForHostText;

    private CanvasGroup _canvasGroup;



    private void Awake()
    {

        _canvasGroup = GetComponent<CanvasGroup>();

        rematchButton.onClick.AddListener(Rematch);
        quitButton.onClick.AddListener(Quit);

    }



    private void Start()
    {
        LocalGameManager.Instance.OnWin += OnWin;
    }



    private void OnWin(Team winningTeam)
    {

        whiteWinText.enabled = (winningTeam == Team.TEAM_WHITE);
        redWinText.enabled = (winningTeam == Team.TEAM_RED);

        ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_GAME_OVER);

        if (!DeckersNetworkManager.isOnline)
        {
            SetOfflineUI(winningTeam);
            return;
        }

        SetOnlineUI(winningTeam);

    }

    private void SetOfflineUI(Team winningTeam)
    {
        rematchButton.interactable = true;
        quitButton.interactable = true;
        waitForHostText.enabled = false;

        if (winningTeam == Team.TEAM_WHITE)
        {
            whiteWinText.SetText("White wins!");
        }
        else if (winningTeam == Team.TEAM_RED)
        {
            redWinText.SetText("Red wins!");
        }

        return;
    }

    private void SetOnlineUI(Team winningTeam)
    {
        bool isHost = DeckersNetworkManager.Instance.IsHost;

        rematchButton.interactable = isHost;
        quitButton.interactable = isHost;
        waitForHostText.enabled = !isHost;

        if (winningTeam == Team.TEAM_WHITE)
        {
            if (isHost)
            {
                whiteWinText.SetText("You win!");
            }
            else
            {
                whiteWinText.SetText("You lose...");
            }
        }
        else if (winningTeam == Team.TEAM_RED)
        {
            if (isHost)
            {
                redWinText.SetText("You lose...");
            }
            else
            {
                redWinText.SetText("You win!");
            }
        }
    }



    private void Rematch()
    {

        if(DeckersNetworkManager.isOnline)
        {
            OnlineGameManager.Instance.Game_Rematch();
            return;
        }

        SceneManager.LoadScene("Game");

    }



    private void Quit()
    {

        if(DeckersNetworkManager.isOnline)
        {
            OnlineGameManager.Instance.Game_Quit();
            return;
        }

        Application.Quit();

    }

}