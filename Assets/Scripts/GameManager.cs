using System.Collections.Generic;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Color[] playerColours;
    public int maxPlayers = 4;
    private List<Color> usedColours = new List<Color>();

    public NetworkDictionary<ulong, GameObject> Players = new NetworkDictionary<ulong, GameObject>(new NetworkVariableSettings
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    public Queue<Player> playerTurn = new Queue<Player>();
    
    public override void NetworkStart()
    {
        if(IsHost)
        {
            NetworkManager.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.OnClientDisconnectCallback += HandleClientDisconnected;
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (Players.Count >= maxPlayers)
        {
            Debug.Log("Maximum clients reached, disconnecting new client: " +clientId);
            NetworkManager.Singleton.DisconnectClient(clientId);
            return;
        }
        
        Debug.Log("Client Connected: " +clientId);
        
        var instance = Instantiate(playerPrefab);
        instance.name = "Player " +clientId;
        var playerColour = playerColours[Random.Range(0, playerColours.Length)];
        
        while (usedColours.Count > 0 && usedColours.Contains(playerColour))
            playerColour = playerColours[Random.Range(0, playerColours.Length)];    
        
        usedColours.Add(playerColour);
        instance.GetComponent<Player>().Setup(playerColour);
        instance.GetComponent<NetworkObject>().Spawn();
        Players.Add(clientId, instance);
    }
    
    private void HandleClientDisconnected(ulong clientId)
    {
        Debug.Log("Client Disconnected: " +clientId);
        usedColours.Remove(Players[clientId].GetComponent<Player>().Colour.Value);
        Players.Remove(clientId);
    }
}
