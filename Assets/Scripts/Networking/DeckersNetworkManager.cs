using System;

using Unity.Netcode;



public class DeckersNetworkManager : NetworkManager
{

    public static NetworkManager Instance
    {
        get { return Singleton; }
    }



    private void Start()
    {
        LobbyManager.Instance.onLobbyChanged += CheckForHostUpdate;
    }



    private void CheckForHostUpdate(object sender, EventArgs e)
    {

        if(LobbyManager.Instance.isLobbyHost == IsHost) return;

        Shutdown();
        Invoke("UpdateIsHost", 0.1f);

    }



    private void UpdateIsHost()
    {
        if(LobbyManager.Instance.isLobbyHost) StartHost();
        else StartClient();
    }

}