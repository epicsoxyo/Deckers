using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;



namespace Deckers.Network
{

    public class LobbyManager : MonoBehaviour
    {

        public static LobbyManager Instance { get; private set; }

        public static Lobby currentLobby { get; private set; }
        public static bool isLobbyHost
        {
            get { return currentLobby.HostId == SignInManager.Instance.localId; }
        }

        private ILobbyEvents lobbyEvents;
        public event EventHandler onLobbyChanged;



        private async void Awake()
        {
            if(Instance != null)
            {
                bool leftLobby = await LeaveLobby();
                DestroyImmediate(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }



        private IEnumerator lobbyHeartbeatCoroutine(string lobbyId, float waitTimeSeconds)
        {

            WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);

            while (true)
            {
                try
                { 
                    LobbyService.Instance.SendHeartbeatPingAsync(lobbyId); 
                }
                catch(ArgumentNullException ex)
                { 
                    Debug.LogError(ex);
                    Debug.LogError("Failed to send heartbeat due to invalid Parameters");
                    yield break;
                }
                catch(LobbyServiceException ex)
                { 
                    Debug.LogError("Failed to send heartbeat due to exception: " + ex);
                    yield break;
                }

                yield return delay;
            }

        }



        private async Task<bool> createLobby(string playerName)
        {

            if (currentLobby != null)
            {
                Debug.LogWarning("Tried to create lobby but a lobby already exists!");
                return false;
            }

            try
            {
                string lobbyName = "Lobby";
                int maxPlayers = 2;
                CreateLobbyOptions options = new CreateLobbyOptions();

                options.IsPrivate = true;
                options.Player = new Player
                (
                    id: SignInManager.Instance.localId,
                    data: new Dictionary<string, PlayerDataObject>()
                    {
                        { "PlayerName", new PlayerDataObject( visibility: PlayerDataObject.VisibilityOptions.Member, playerName ) },
                        { "Team", new PlayerDataObject( visibility: PlayerDataObject.VisibilityOptions.Member, Game.Team.TEAM_WHITE.ToString() ) },
                    }
                );
                options.Data = new Dictionary<string, DataObject>
                {
                    { "RelayCode", new DataObject( visibility: DataObject.VisibilityOptions.Member, "0" ) },
                };

                currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

                StartCoroutine(lobbyHeartbeatCoroutine(currentLobby.Id, 15f));
            }
            catch(Exception ex)
            {
                Debug.LogError("Could not create lobby due to exception: " + ex);
                return false;
            }

            Debug.Log("Created Lobby: " + currentLobby.LobbyCode);

            return true;

        }



        private async Task<bool> joinLobby(string lobbyCode, string playerName)
        {

            if (currentLobby != null)
            {
                Debug.LogWarning("Tried to join a lobby but there is already a lobby joined!");
                return false;
            }

            try
            {
                JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();

                options.Player = new Player(
                    id: AuthenticationService.Instance.PlayerId,
                    data: new Dictionary<string, PlayerDataObject>()
                    {
                        { "PlayerName", new PlayerDataObject( visibility: PlayerDataObject.VisibilityOptions.Member, playerName) },
                        { "Team", new PlayerDataObject( visibility: PlayerDataObject.VisibilityOptions.Member, Game.Team.TEAM_RED.ToString()) },
                    }
                );

                currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);

                StartCoroutine(lobbyHeartbeatCoroutine(currentLobby.Id, 15f));

                Debug.Log("Joined Lobby: " + currentLobby.LobbyCode);
                return true;
            }
            catch(Exception ex)
            {
                Debug.LogError("Could not join lobby due to exception: " + ex);
            }

            return false;

        }



        private async Task<bool> leaveLobby()
        {

            if (currentLobby == null)
            {
                Debug.LogError("Tried to leave lobby but there was no lobby to leave!");
                return false;
            }

            try
            {
                string playerId = SignInManager.Instance.localId;
                string lobbyId = currentLobby.Id;

                if(lobbyEvents != null) { await lobbyEvents.UnsubscribeAsync(); }

                if(currentLobby.Players.Count <= 1)
                {
                    await deleteLobby(lobbyId);
                }
                else
                {
                    await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
                }

                DeckersNetworkManager.Instance.Shutdown();

                // stop heartbeat & lobby updates
                StopAllCoroutines();
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogError("Tried to leave lobby but currentLobby does not exist! Exception: " + ex);
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError("Could not leave lobby due to exception: " + ex);
                return false;
            }

            currentLobby = null;
            return true;

        }



        private async Task<bool> deleteLobby(string lobbyId)
        {

            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to delete lobby due to exception: " + e);
                return false;
            }

            currentLobby = null;
            return true;

        }



        private async Task<bool> subscribeToLobbyEvents()
        {

            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.LobbyChanged += OnLobbyChanged;

            try
            {
                lobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(currentLobby.Id, callbacks);
                return true;
            }
            catch (LobbyServiceException ex)
            {
                switch (ex.Reason)
                {
                    case LobbyExceptionReason.AlreadySubscribedToLobby:
                        Debug.LogWarning($"Already subscribed to lobby[{currentLobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}");
                        break;
                    case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy:
                        Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}");
                        throw;
                    case LobbyExceptionReason.LobbyEventServiceConnectionError:
                        Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}");
                        throw;
                    default:
                        throw;
                }
            }

            return false;

        }



        private async void OnLobbyChanged(ILobbyChanges changes)
        {

            if(changes == null) return;

            changes.ApplyToLobby(currentLobby);
            onLobbyChanged.Invoke(this, EventArgs.Empty);

            if(isLobbyHost) return;

            if(currentLobby.Data["RelayCode"].Value != "0")
            {
                await RelayManager.Instance.JoinRelay(currentLobby.Data["RelayCode"].Value);
            }

        }



        public async Task<bool> CreateLobby(string playerName)
        {

            // TODO: add retry 3 times if failed

            bool lobbyCreated = await createLobby(playerName);

            if (!lobbyCreated) return false;

            lobbyCreated = await subscribeToLobbyEvents();

            return lobbyCreated;

        }



        public async Task<bool> JoinLobby(string lobbyCode, string playerName)
        {

            // TODO: add retry 3 times if failed
            bool lobbyJoined = await joinLobby(lobbyCode, playerName);

            if (!lobbyJoined) return false;

            lobbyJoined = await subscribeToLobbyEvents();

            return lobbyJoined;

        }



        public async Task<bool> LeaveLobby()
        {

            // TODO: add retry 3 times if failed

            bool leftLobby = await leaveLobby();
            return leftLobby;

        }



        public List<string> GetPlayerNames()
        {

            if (currentLobby == null) return new List<string>();

            //update list of players in lobby
            List<string> list = new List<string>();
            foreach (Player player in currentLobby.Players)
            {
                list.Add(player.Data["PlayerName"].Value);
            }

            return list;

        }



        public async Task<bool> StartGame()
        {

            if(currentLobby.Players.Count != 2) return false;

            try
            {
                string relayCode = await RelayManager.Instance.CreateRelay();

                UpdateLobbyOptions options = new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode) },
                    }
                };

                currentLobby = await Lobbies.Instance.UpdateLobbyAsync(currentLobby.Id, options);

                SceneLoader.Instance.LoadSceneOverNetwork(GameScene.Game);

                return true;
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogError("Failed to start game due to exception: " + ex);
            }

            return false;

        }

    }

}