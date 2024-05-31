using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using UnityEngine;



public class RelayManager : MonoBehaviour
{

    public static RelayManager Instance { get; private set;}



    private void Awake()
    {
        Instance = this;
    }



    public async Task<string> CreateRelay()
    {

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            UnityTransport transport = DeckersNetworkManager.Instance.GetComponent<UnityTransport>();
            transport.SetHostRelayData
            (
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            DeckersNetworkManager.Instance.StartHost();

            return joinCode;
        }
        catch(RelayServiceException ex)
        {
            Debug.LogError("Could not create relay due to exception: " + ex);
        }

        return null;

    }



    public async Task<bool> JoinRelay(string joinCode)
    {

        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            UnityTransport transport = DeckersNetworkManager.Instance.GetComponent<UnityTransport>();
            transport.SetClientRelayData
            (
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            DeckersNetworkManager.Instance.StartClient();

            return true;
        }
        catch(RelayServiceException ex)
        {
            Debug.LogError("Could not join relay due to exception: " + ex);
        }

        return false;

    }

}