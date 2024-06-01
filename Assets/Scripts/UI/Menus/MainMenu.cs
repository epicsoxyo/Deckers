using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using Deckers.Network;



public enum MenuPanel
{
    LOBBY,
}



// main menu UI script
public class MainMenu : MonoBehaviour
{

    public static MainMenu Instance;
    private Animator menuStateMachine;

    private LobbyMenu lobbyMenu;
    private JoinMenu joinMenu;
    // private Stack<Menu> openPanels = new Stack<Menu>();

    [Header("Main Menu Buttons")] 
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button localPlayButton;

    [Header("Information UI")]
    [SerializeField] private InfoUI infoUI;



    private void Awake()
    {

        if(!Init()) return;

        menuStateMachine = GetComponent<Animator>();
        lobbyMenu = GetComponentInChildren<LobbyMenu>();

        createLobbyButton.onClick.AddListener(async () => await CreateLobby());
        joinLobbyButton.onClick.AddListener(JoinLobby);
        localPlayButton.onClick.AddListener(TriggerLocalGameStart);

    }



    private bool Init()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple MainMenu instances detected!");
            return false;
        }

        Instance = this;
        return true;

    }



    private void ActivateMenuButtons(bool isActive)
    {
        createLobbyButton.interactable = isActive;
        joinLobbyButton.interactable = isActive;
        localPlayButton.interactable = isActive;
    }



    private async Task CreateLobby()
    {

        ActivateMenuButtons(false);

        string playerName = "Ready";

        bool lobbyCreated = await LobbyManager.Instance.CreateLobby(playerName);
        if (!lobbyCreated)
        {
            // TODO: add pop up that says lobby could not be created
            // infoUI.ShowError("Lobby could not be created");
            Debug.LogError("lobby could not be created");
            return;
        }

        OpenLobbyMenu();

    }



    public void OpenLobbyMenu()
    {
        menuStateMachine.SetTrigger("ToLobby");
        lobbyMenu.SetLobbyInfoUI(LobbyManager.currentLobby);
    }



    private void JoinLobby()
    {
        ActivateMenuButtons(false);
        menuStateMachine.SetTrigger("ToJoin");
    }


    
    private void TriggerLocalGameStart()
    {
        menuStateMachine.SetTrigger("StartGame");
    }

    private void LocalGameStart()
    {
        SceneLoader.Instance.LoadSceneLocally(GameScene.Game);
    }



    // opens the panel given by the parameter + sets it to the currently active panel
    // public Menu OpenPanel(MenuPanel panel)
    // {

    //     Menu panelObject = GetPanelFromEnum(panel);

    //     CanvasGroup panelCanvasGroup = panelObject.GetComponent<CanvasGroup>();
    //     panelCanvasGroup.alpha = 1f;
    //     panelCanvasGroup.interactable = true;
    //     panelCanvasGroup.blocksRaycasts = true;

    //     openPanels.Push(panelObject);

    //     return panelObject;

    // }



    // closes the current open panel
    public void CloseCurrentPanel()
    {

        menuStateMachine.SetTrigger("ToMain");
        ActivateMenuButtons(true);

        // if(openPanels == null)
        // {
        //     Debug.LogWarning("ClosePanel() was called but currentOpenPanel is null.");
        //     return;
        // }

        // GameObject currentPanel = openPanels.Pop().gameObject;

        // CanvasGroup panelCanvasGroup = currentPanel.GetComponent<CanvasGroup>();
        // panelCanvasGroup.alpha = 0f;
        // panelCanvasGroup.interactable = false;
        // panelCanvasGroup.blocksRaycasts = false;

        // createLobbyButton.interactable = true;
        // joinLobbyButton.interactable = true;

    }



    // returns the associated panel GameObject from the enum parameter
    // private Menu GetPanelFromEnum(MenuPanel panel)
    // {

    //     switch(panel)
    //     {
    //         case MenuPanel.LOBBY:
    //             return lobbyMenu;
    //     }
        
    //     Debug.LogWarning("No panel assigned to " + panel);
    //     return null;
    // }

}
