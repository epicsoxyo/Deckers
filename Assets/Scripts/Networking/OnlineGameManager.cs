using System;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class OnlineGameManager : NetworkBehaviour
{

    public static OnlineGameManager Instance { get; private set; }

    public Team localTeam
    {
        get
        {
            return (DeckersNetworkManager.Instance.IsHost) ? Team.TEAM_WHITE : Team.TEAM_RED;
        }
    }



    private void Awake()
    {
        
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of OnlineGameManager detected!");
            return;
        }

        Instance = this;

    }



    private void Start()
    {
        LobbyManager.Instance.onGameStart += StartGame;
    }



    // GAME MANAGER RPCs

    private void StartGame(object sender, EventArgs e)
    {

        if(localTeam == Team.TEAM_RED)
        {
            Transform playableArea = CheckersGameManager.Instance.playableArea;
            playableArea.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.LowerRight;
        }

        LocalGameManager.Instance.StartGame();

    }



    // CHECKERS MANAGER RPCs

    public void Checkers_MovePiece(int pieceId, int x, int y)
    {
        Checkers_MovePieceServerRpc(pieceId, x, y);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Checkers_MovePieceServerRpc(int pieceId, int x, int y)
    {
        Checkers_MovePieceClientRpc(pieceId, x, y);
    }

    [ClientRpc]
    private void Checkers_MovePieceClientRpc(int pieceId, int x, int y)
    {
        CheckersGameManager.Instance.MovePiece(pieceId, x, y);
    }



    public void Checkers_PromotePiece(int pieceId)
    {
        Checkers_PromotePieceServerRpc(pieceId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Checkers_PromotePieceServerRpc(int pieceId)
    {
        Checkers_PromotePieceClientRpc(pieceId);
    }

    [ClientRpc]
    private void Checkers_PromotePieceClientRpc(int pieceId)
    {
        CheckersGameManager.Instance.PromotePiece(pieceId);
    }



    public void Checkers_CapturePiece(int pieceId)
    {
        Debug.Log("Triggering capture server rpc");
        Checkers_CapturePieceServerRpc(pieceId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Checkers_CapturePieceServerRpc(int pieceId)
    {
        Debug.Log("Triggering capture client rpc");
        Checkers_CapturePieceClientRpc(pieceId);
    }

    [ClientRpc]
    private void Checkers_CapturePieceClientRpc(int pieceId)
    {
        Debug.Log("Triggering local capture");
        CheckersGameManager.Instance.CapturePiece(pieceId);
    }



    public void Checkers_EndTurn()
    {
        Checkers_EndTurnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void Checkers_EndTurnServerRpc()
    {
        Checkers_EndTurnClientRpc();
    }

    [ClientRpc]
    private void Checkers_EndTurnClientRpc()
    {
        CheckersGameManager.Instance.EndTurn();
    }




    // WIN SCREEN RPCs

    public void Game_Rematch()
    {
        if(!IsHost) return;
        Game_RematchClientRpc();
    }

    [ClientRpc]
    private void Game_RematchClientRpc()
    {
        DeckersNetworkManager.Instance.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }



    public void Game_Quit()
    {
        if(!IsHost) return;
        Game_QuitClientRpc();
    }

    [ClientRpc]
    private void Game_QuitClientRpc()
    {
        Application.Quit();
    }

}