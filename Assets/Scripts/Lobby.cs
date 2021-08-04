using System;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : NetworkBehaviour
{
    [SerializeField] private PlayerLobbyCard[] playerLobbyCards;
    [SerializeField] private Button startButton;

    private NetworkList<LobbyPlayerState> lobbyPlayerStates = new NetworkList<LobbyPlayerState>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    public override void NetworkStart()
    {
        Debug.Log("Network Start Initiated on Lobby");
        if(IsHost)
        {
            startButton.gameObject.SetActive(true);
            
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (var networkClient in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(networkClient.ClientId);
            }
        }
        
        lobbyPlayerStates.OnListChanged += HandleLobbyPlayerStateChanged;
    }

    private void OnDestroy()
    {
        if(NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
        lobbyPlayerStates.OnListChanged -= HandleLobbyPlayerStateChanged;
    }

    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log("Player connected : assigned clientId "+clientId);
        
        lobbyPlayerStates.Add(new LobbyPlayerState(
            clientId,
            "Player " +clientId,
            false
        ));
        
    }
    
    private void HandleClientDisconnected(ulong clientId)
    {
        Debug.Log("Player Disconnected : " +clientId);
        for (int i = 0; i < lobbyPlayerStates.Count; i++)
        {
            if (lobbyPlayerStates[i].ClientId == clientId)
            {
                lobbyPlayerStates.RemoveAt(i);
                break;
            }
        }
    }
    
    private void HandleLobbyPlayerStateChanged(NetworkListEvent<LobbyPlayerState> lobbyState)
    {
        Debug.Log($"Lobby Player States Count : {lobbyPlayerStates.Count}");
        
        for (int i = 0; i < playerLobbyCards.Length; i++)
        {
            if (lobbyPlayerStates.Count > i)
            {
                Debug.Log($"Activating PlayerCard {i}");
                playerLobbyCards[i].UpdateDisplay(lobbyPlayerStates[i]);
            }
            else
            {
                playerLobbyCards[i].DisableDisplay();
                Debug.Log($"Disabling PlayerCard {i}");
            }
        }

        if (IsHost)
        {
            startButton.interactable = IsEveryoneReady();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"RPC Called from {serverRpcParams.Receive.SenderClientId} : {playerName}");
        
        for (int i = 0; i < lobbyPlayerStates.Count; i++)
        {
            if (lobbyPlayerStates[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                lobbyPlayerStates[i] = new LobbyPlayerState(
                    lobbyPlayerStates[i].ClientId,
                    playerName,
                    !lobbyPlayerStates[i].IsReady
                );
            }
        }
    }
    
    private bool IsEveryoneReady()
    {
        foreach (var player in lobbyPlayerStates)
        {
            if (!player.IsReady)
                return false;
        }
        return true;
    }

    public void OnReadyClicked() =>
        ToggleReadyServerRpc(PlayerPrefs.GetString("PlayerName"));
}
