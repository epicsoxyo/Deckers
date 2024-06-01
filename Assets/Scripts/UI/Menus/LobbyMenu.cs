using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;

using TMPro;

using Deckers.Network;



public class LobbyMenu : Menu
{

    [Header("Lobby Buttons")]
    [SerializeField] private Button leaveLobbyButton;
    [SerializeField] private Button startButton;
    private CanvasGroup _startButtonCanvasGroup;

    [Header("Lobby Info")]
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyCode;

    [Header("Player List")]
    [SerializeField] private TextMeshProUGUI[] playerNameFields = new TextMeshProUGUI[2];



    private void Awake()
    {
        _startButtonCanvasGroup = startButton.GetComponent<CanvasGroup>();
        ActivateStartButton(false);
    }



    private void ActivateStartButton(bool isActive = true)
    {
        _startButtonCanvasGroup.alpha = isActive ? 1f : 0f;
        _startButtonCanvasGroup.interactable = isActive;
        _startButtonCanvasGroup.blocksRaycasts = isActive;
    }



    protected virtual void Start()
    {
        LobbyManager.Instance.onLobbyChanged += UpdateLobbyInfoUI;
        leaveLobbyButton.onClick.AddListener(async () => await LeaveLobby());
        startButton.onClick.AddListener(StartGame);
    }



    public void SetLobbyInfoUI(Lobby lobby)
    {
        ActivateStartButton();
        lobbyCode.SetText(lobby.LobbyCode);
        lobbyName.SetText(lobby.Name);
        UpdateLobbyInfoUI();
    }



    private void UpdateLobbyInfoUI(object sender, EventArgs e)
    {
        UpdateLobbyInfoUI();
    }

    public void UpdateLobbyInfoUI()
    {

        LobbyManager lobbyManager = LobbyManager.Instance;
        if (lobbyManager == null)
        {
            Debug.LogError("Cannot update player list as lobby manager does not exist.");
            return;
        }

        Lobby lobby = LobbyManager.currentLobby;

        List<string> playerList = lobbyManager.GetPlayerNames();
        for(int i = 0; i < 2; i++)
        {
            if(i >= playerList.Count) playerNameFields[i].SetText("--");
            else playerNameFields[i].SetText(playerList[i]);
        }

        ActivateStartButton(LobbyManager.isLobbyHost);

    }



    protected virtual async Task LeaveLobby()
    {

        bool leftLobby = await LobbyManager.Instance.LeaveLobby();

        if (!leftLobby)
        {
            // TODO: add pop up that says lobby could not be created

            Debug.LogError("Lobby could not be left");
            return;
        }

        MainMenu.Instance.CloseCurrentPanel();
        Debug.Log("Left Lobby");

    }



    private async void StartGame()
    {

        bool hasStartedGame = await LobbyManager.Instance.StartGame();
        ActivateStartButton(!hasStartedGame); // reactivate if game start failed

        if(hasStartedGame)
        {
            LobbyManager.Instance.onLobbyChanged -= UpdateLobbyInfoUI;
            leaveLobbyButton.onClick.RemoveListener(async () => await LeaveLobby());
            startButton.onClick.RemoveListener(StartGame);
        }

    }

}