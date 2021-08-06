using DG.Tweening;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;

public class PlayerHandler : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Board board;
    [SerializeField] private GameSequenceHandler gameSequenceHandler;

    public NetworkDictionary<ulong, Player> Players = new NetworkDictionary<ulong, Player>(new NetworkVariableSettings
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    public override void NetworkStart()
    {
        if(IsHost)
        {
            NetworkManager.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.OnClientDisconnectCallback += HandleClientDisconnected;
        }

        if (IsClient)
        {
            SpawnPlayerServerRpc(PlayerPrefs.GetString("PlayerName"), NetworkManager.Singleton.LocalClientId);
        }
    }
    
    private void HandleClientConnected(ulong clientId) => Debug.Log("Client Connected: " +clientId);
    
    private void HandleClientDisconnected(ulong clientId)
    {
        Debug.Log("Client Disconnected: " +clientId);
        Players.Remove(clientId);
    }
        
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(string playerName, ulong clientId)
    {
        var instance = Instantiate(playerPrefab, board.startPosition.transform.position, Quaternion.identity);
        instance.name = $"{clientId} {playerName}";
        instance.GetComponent<Player>().Setup(playerName, clientId);
        instance.GetComponent<NetworkObject>().Spawn();
        Players.Add(clientId, instance.GetComponent<Player>());
    }
    
    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    public void MovePieceServerRpc(ulong clientId)
    {
        var playerPiece = Players[clientId];
        var nextPosition = board.NextPosition(playerPiece.CurrentPosition);
        
        if (playerPiece.CurrentPosition < 100)
        {
            playerPiece.transform.DOMove(nextPosition, 1f);
            playerPiece.CurrentPosition++;
        }
        if(playerPiece.CurrentPosition >= 100)
        {
            gameSequenceHandler.WinAchievedClientRpc(clientId);
        }
    }
    
    public void MovePlayerTo(ulong clientId, int Block)
    {
        if(Block != -1)
        {
            var playerPiece = Players[clientId];
            playerPiece.transform.DOMove(board.positions[Block].transform.position, 2f);
            playerPiece.CurrentPosition = Block;
            if (playerPiece.CurrentPosition >= 100)
            {
                gameSequenceHandler.WinAchievedClientRpc(clientId);
            }
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }
}
