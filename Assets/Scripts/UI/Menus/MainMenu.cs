using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System.Threading.Tasks;



public enum MenuPanel
{
    LOBBY,
}



// main menu UI script
public class MainMenu : MonoBehaviour
{

    public static MainMenu Instance;

    private LobbyMenu lobbyMenu;
    private Stack<Menu> openPanels = new Stack<Menu>();

    [Header("Main Menu Buttons")] 
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private TMP_InputField lobbyCodeField;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button localPlayButton;



    // gets panels and adds relevant events to all menu buttons
    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple MainMenu instances detected!");
            return;
        }
        Instance = this;

        lobbyMenu = GetComponentInChildren<LobbyMenu>(true);

        createLobbyButton.onClick.AddListener(async () => await CreateLobby());
        joinLobbyButton.onClick.AddListener(async () => await JoinLobby());
        localPlayButton.onClick.AddListener(() => LocalGameManager.Instance.StartGame());

    }



    private async Task CreateLobby()
    {

        createLobbyButton.interactable = false;

        string playerName = "Ready"; // hostPlayerName.text;

        bool lobbyCreated = await LobbyManager.Instance.CreateLobby(playerName);
        if (!lobbyCreated)
        {
            // TODO: add pop up that says lobby could not be created

            // OpenPanel(MenuPanel.ERROR);

            Debug.LogError("lobby could not be created");
            return;
        }

        OpenPanel(MenuPanel.LOBBY);
        lobbyMenu.SetLobbyInfoUI(LobbyManager.Instance.currentLobby);
        lobbyMenu.UpdateLobbyInfoUI();


    }


    private async Task JoinLobby()
    {

        joinLobbyButton.interactable = false;

        string lobbyCode = lobbyCodeField.text.ToUpper();
        if(lobbyCode == null) return;
        // string playerName = playerNameField.text;

        bool lobbyCreated = await LobbyManager.Instance.JoinLobby(lobbyCode, "Ready");

        if (!lobbyCreated)
        {
            // TODO: add pop up that says lobby could not be created

            Debug.LogError("Lobby could not be joined");
            return;
        }

        OpenPanel(MenuPanel.LOBBY);
        lobbyMenu.SetLobbyInfoUI(LobbyManager.Instance.currentLobby);
        lobbyMenu.UpdateLobbyInfoUI();

    }



    // opens the panel given by the parameter + sets it to the currently active panel
    public Menu OpenPanel(MenuPanel panel)
    {

        Menu panelObject = GetPanelFromEnum(panel);

        CanvasGroup panelCanvasGroup = panelObject.GetComponent<CanvasGroup>();
        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;

        openPanels.Push(panelObject);

        return panelObject;

    }



    // closes the current open panel
    public void CloseCurrentPanel()
    {

        if(openPanels == null)
        {
            Debug.LogWarning("ClosePanel() was called but currentOpenPanel is null.");
            return;
        }

        GameObject currentPanel = openPanels.Pop().gameObject;

        CanvasGroup panelCanvasGroup = currentPanel.GetComponent<CanvasGroup>();
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        createLobbyButton.interactable = true;
        joinLobbyButton.interactable = true;

    }



    // returns the associated panel GameObject from the enum parameter
    private Menu GetPanelFromEnum(MenuPanel panel)
    {

        switch(panel)
        {
            case MenuPanel.LOBBY:
                return lobbyMenu;
        }
        
        Debug.LogWarning("No panel assigned to " + panel);
        return null;
    }

}
