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
        get
        {
            if (Singleton == null) return false;
            return Singleton.IsServer || Singleton.IsClient;
        }
    }

}