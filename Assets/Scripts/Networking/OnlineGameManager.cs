using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineGameManager : NetworkBehaviour
{

    public static OnlineGameManager Instance { get; private set; }



    private void Awake()
    {
        
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of OnlineGameManager detected!");
            return;
        }

        Instance = this;

    }



    public void StartGame()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        LocalGameManager.Instance.StartGame();
    }




}