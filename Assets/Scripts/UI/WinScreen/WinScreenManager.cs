using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



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
        LocalGameManager.Instance.onWhiteWin += OnWhiteWin;
        LocalGameManager.Instance.onRedWin += OnRedWin;
    }



    private void OnWhiteWin(object sender, EventArgs e)
    {

        whiteWinText.enabled = true;
        redWinText.enabled = false;

        ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_GAME_OVER);

        if(!DeckersNetworkManager.isOnline)
        {

            rematchButton.interactable = true;
            quitButton.interactable = true;
            waitForHostText.enabled = false;

            whiteWinText.SetText("White wins!");

            return;

        }

        bool isHost = DeckersNetworkManager.Instance.IsHost;

        rematchButton.interactable = isHost;
        quitButton.interactable = isHost;
        waitForHostText.enabled = !isHost;
    
        if(isHost)
        {
            whiteWinText.SetText("You win!");
        }
        else
        {
            whiteWinText.SetText("You lose...");
        }

    }



    private void OnRedWin(object sender, EventArgs e)
    {

        whiteWinText.enabled = false;
        redWinText.enabled = true;

        ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_GAME_OVER);

        if(!DeckersNetworkManager.isOnline)
        {

            rematchButton.interactable = true;
            quitButton.interactable = true;
            waitForHostText.enabled = false;

            redWinText.SetText("Red wins!");

            return;

        }

        bool isHost = DeckersNetworkManager.Instance.IsHost;

        rematchButton.interactable = isHost;
        quitButton.interactable = isHost;
        waitForHostText.enabled = !isHost;
    
        if(isHost)
        {
            redWinText.SetText("You win!");
        }
        else
        {
            redWinText.SetText("You lose...");
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