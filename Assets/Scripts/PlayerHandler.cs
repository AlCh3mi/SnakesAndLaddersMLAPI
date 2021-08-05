using DG.Tweening;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHandler : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Board board;

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

    private void OnDestroy()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
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
    
    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log("Client Connected: " +clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void MovePieceServerRpc(ulong clientId)
    {
        var playerPiece = Players[clientId];
        
        var nextPosition = board.NextPosition(playerPiece.CurrentPosition);
        if (playerPiece.CurrentPosition < 100)
            playerPiece.CurrentPosition++;
         
        playerPiece.transform.DOMove(nextPosition, 1f);
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            SceneManager.LoadScene("Scenes/Menu");
        }
        Debug.Log("Client Disconnected: " +clientId);
        Players.Remove(clientId);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Move"))
            MovePieceServerRpc(NetworkManager.Singleton.LocalClientId);
    }
}
