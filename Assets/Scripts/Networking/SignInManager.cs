using System.Threading.Tasks;

using UnityEngine;

using Unity.Services.Core;
using Unity.Services.Authentication;



namespace Deckers.Network
{

    public class SignInManager : MonoBehaviour
    {

        public static SignInManager Instance { get; private set; }
        public string localId;



        private async void Awake()
        {

            if (!await Initialise()){
                Debug.LogError("Failed to initialise LobbyManager");
                return;
            }

            localId = AuthenticationService.Instance.PlayerId;

        }



        private async Task<bool> Initialise()
        {

            Instance = this;

            await UnityServices.InitializeAsync();

            bool isSignedIn = await SignInAnonymouslyAsync(); 
            return isSignedIn;

        }



        private async Task<bool> SignInAnonymouslyAsync()
        {

            try
            {
                SwitchProfilesIfClone();

                if(!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                
                Debug.Log("Anonymous sign-in successful!\n" +
                    $"Profile: {AuthenticationService.Instance.Profile}\n" +
                    $"PlayerID: {AuthenticationService.Instance.PlayerId}");

                return true;
            }
            catch (AuthenticationException ex) { Debug.LogException(ex); }
            catch (RequestFailedException ex) { Debug.LogException(ex); }

            return false;

        }



        private void SwitchProfilesIfClone()
        {

            /* 
                creates a custom player ID for use with ParallelSync in UNITY_EDITOR
                (or else both will be identical)

                does this by forcing ParallelSync clone switch to a different authentication
                profile to make it sign in as a different anonymous user account
            */

            #if UNITY_EDITOR
            if (ParrelSync.ClonesManager.IsClone()){
                string customArgument = ParrelSync.ClonesManager.GetArgument();
                AuthenticationService.Instance.SwitchProfile($"Clone_{customArgument}_Profile");
            }
            #endif

        }

    }

}