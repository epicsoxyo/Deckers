using System;

using Unity.Netcode;



public class DeckersNetworkManager : NetworkManager
{

    public static NetworkManager Instance
    {
        get { return Singleton; }
    }

    public static bool isOnline
    {
        get { return Singleton.IsServer || Singleton.IsClient; }
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
        if(LobbyManager.Instance.isLobbyHost)
        {
            StartHost();
            OnlineGameManager.Instance.localTeam = Team.TEAM_WHITE;
            return;
        }
        
        StartClient();
        OnlineGameManager.Instance.localTeam = Team.TEAM_RED;
    }

}