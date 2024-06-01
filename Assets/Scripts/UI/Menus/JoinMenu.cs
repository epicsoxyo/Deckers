using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Deckers.Network;



public class JoinMenu : MonoBehaviour
{

    [SerializeField] private TMP_InputField lobbyCodeField;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button cancelButton;



    private void Awake()
    {
        joinLobbyButton.onClick.AddListener(async () => await JoinLobby());
        cancelButton.onClick.AddListener(() => MainMenu.Instance.CloseCurrentPanel());
    }
    


    private async Task JoinLobby()
    {

        string lobbyCode = lobbyCodeField.text.ToUpper();
        if(lobbyCode == null) return;
        // string playerName = playerNameField.text;

        bool lobbyJoined = await LobbyManager.Instance.JoinLobby(lobbyCode, "Ready");

        if (!lobbyJoined)
        {
            // TODO: add pop up that says lobby could not be joined

            Debug.LogError("Lobby could not be joined");
            return;
        }

        MainMenu.Instance.OpenLobbyMenu();

    }

}