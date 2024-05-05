using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OnlineGameManager : NetworkBehaviour
{

    public static OnlineGameManager Instance { get; private set; }
    public Team localTeam;



    private void Awake()
    {
        
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of OnlineGameManager detected!");
            return;
        }

        Instance = this;

    }



    // GAME MANAGER RPCs

    public void StartGame()
    {
        StartGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        if(localTeam != Team.TEAM_WHITE)
        {
            Transform playableArea = CheckersGameManager.Instance.playableArea;
            playableArea.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.LowerRight;
        }
        LocalGameManager.Instance.StartGame();
    }



    // CHECKERS MANAGER RPCs

    public void MovePiece(int pieceId, int x, int y)
    {
        MovePieceServerRpc(pieceId, x, y);
    }

    [ServerRpc(RequireOwnership = false)]
    private void MovePieceServerRpc(int pieceId, int x, int y)
    {
        MovePieceClientRpc(pieceId, x, y);
    }

    [ClientRpc]
    private void MovePieceClientRpc(int pieceId, int x, int y)
    {
        CheckersGameManager.Instance.MovePiece(pieceId, x, y);
    }



    public void PromotePiece(int pieceId)
    {
        PromotePieceServerRpc(pieceId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PromotePieceServerRpc(int pieceId)
    {
        PromotePieceClientRpc(pieceId);
    }

    [ClientRpc]
    private void PromotePieceClientRpc(int pieceId)
    {
        CheckersGameManager.Instance.PromotePiece(pieceId);
    }



    public void CapturePiece(int pieceId)
    {
        Debug.Log("Triggering capture server rpc");
        CapturePieceServerRpc(pieceId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CapturePieceServerRpc(int pieceId)
    {
        Debug.Log("Triggering capture client rpc");
        CapturePieceClientRpc(pieceId);
    }

    [ClientRpc]
    private void CapturePieceClientRpc(int pieceId)
    {
        Debug.Log("Triggering local capture");
        CheckersGameManager.Instance.CapturePiece(pieceId);
    }



    public void CheckersEndTurn()
    {
        CheckersEndTurnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckersEndTurnServerRpc()
    {
        CheckersEndTurnClientRpc();
    }

    [ClientRpc]
    private void CheckersEndTurnClientRpc()
    {
        CheckersGameManager.Instance.EndTurn();
    }

}