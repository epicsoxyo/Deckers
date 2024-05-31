using System.Collections;
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
            if(!DeckersNetworkManager.isOnline) return Team.TEAM_NULL;
            return DeckersNetworkManager.Instance.IsHost ? Team.TEAM_WHITE : Team.TEAM_RED;
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



    // GAME MANAGER RPCs

    public void StartGame()
    {
        Debug.Log("Starting online game...");
        StartCoroutine("StartGameCoroutine");
    }

    private IEnumerator StartGameCoroutine()
    {

        if(IsHost)
        {
            while(DeckersNetworkManager.Instance.ConnectedClients.Count < 2) yield return null;
        }
        else
        {
            while(!IsClient) yield return null;
        }

        Debug.Log("Connected!");

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
        CheckersGameManager.Instance.LocalMovePiece(pieceId, x, y);
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
        CheckersGameManager.Instance.LocalPromotePiece(pieceId);
    }



    public void Checkers_CapturePiece(int pieceId)
    {
        Checkers_CapturePieceServerRpc(pieceId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Checkers_CapturePieceServerRpc(int pieceId)
    {
        Checkers_CapturePieceClientRpc(pieceId);
    }

    [ClientRpc]
    private void Checkers_CapturePieceClientRpc(int pieceId)
    {
        CheckersGameManager.Instance.LocalCapturePiece(pieceId);
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
        CheckersGameManager.Instance.LocalEndTurn();
    }



    // DECKERS MANAGER RPCs

    public void Deckers_SwapLocalSlots()
    {
        Deckers_SwapLocalSlotsClientRpc();
    }

    [ClientRpc]
    private void Deckers_SwapLocalSlotsClientRpc()
    {
        if(IsHost) return;
        CardsManager.Instance.SwapLocalSlots();
    }



    public void Deckers_DrawCard(Team team, int cardId)
    {
        if(!IsHost) return;
        Deckers_DrawCardServerRpc((byte)team, cardId);
    }

    [ServerRpc]
    private void Deckers_DrawCardServerRpc(byte team, int cardId)
    {
        Deckers_DrawCardClientRpc(team, cardId);
    }

    [ClientRpc]
    private void Deckers_DrawCardClientRpc(byte team, int cardId)
    {
        CardsManager.Instance.LocalDrawCard((Team)team, cardId);
    }



    public void Deckers_SwapSlots(int slot1, int slot2)
    {
        Deckers_SwapSlotsServerRpc(slot1, slot2);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Deckers_SwapSlotsServerRpc(int slot1, int slot2)
    {
        Deckers_SwapSlotsClientRpc(slot1, slot2);
    }

    [ClientRpc]
    private void Deckers_SwapSlotsClientRpc(int slot1, int slot2)
    {
        DropAreaManager.Instance.LocalSwap(slot1, slot2);
    }



    public void Deckers_PlayCard(int cardId)
    {
        Deckers_PlayCardServerRpc(cardId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Deckers_PlayCardServerRpc(int cardId)
    {
        Deckers_PlayCardClientRpc(cardId);
    }

    [ClientRpc]
    private void Deckers_PlayCardClientRpc(int cardId)
    {
        DeckersGameManager.Instance.LocalPlayCard(cardId);
    }



    public void Deckers_EndTurn()
    {
        Deckers_EndTurnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void Deckers_EndTurnServerRpc()
    {
        Deckers_EndTurnClientRpc();
    }

    [ClientRpc]
    private void Deckers_EndTurnClientRpc()
    {
        DeckersGameManager.Instance.LocalEndTurn();
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