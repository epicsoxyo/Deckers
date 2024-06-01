using Unity.Netcode;



namespace Deckers.Network
{

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
                return Singleton.IsClient;
            }
        }

    }

}