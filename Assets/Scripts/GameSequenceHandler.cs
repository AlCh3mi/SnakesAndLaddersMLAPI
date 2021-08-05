using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class GameSequenceHandler : NetworkBehaviour
{
    [SerializeField] private GameObject onScreenMessagePrefab;
    [SerializeField] private Canvas GUI;
    [SerializeField] private PlayerHandler playerHandler;
    [SerializeField] private Dice dice;

    private Queue<ulong> playerTurn = new Queue<ulong>();

    public NetworkVariableInt currentPlayersTurn = new NetworkVariableInt();

    public override void NetworkStart()
    {
        foreach (var networkClient in NetworkManager.Singleton.ConnectedClientsList)
        {
            playerTurn.Enqueue(networkClient.ClientId);
        }
        currentPlayersTurn.OnValueChanged += HandlePlayerTurnChanged;
    }

    private void HandlePlayerTurnChanged(int previousvalue, int newvalue)
    {
        //Update UI as to who's turn it currentlyIs 
    }

    public ulong NextPlayersTurn()
    {
        playerTurn.Enqueue(playerTurn.Dequeue());
        var nextPlayer = playerTurn.Peek();
        currentPlayersTurn.Value = (int)nextPlayer;
        ItsYourTurnClientRpc(nextPlayer);
        return nextPlayer;
    }

    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    private void ItsYourTurnClientRpc(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            var message = Instantiate(onScreenMessagePrefab, GUI.transform);
            message.GetComponent<OnScreenMessage>().Display("Its your turn to play!", "Roll the dice", 3f);
        }
    }

    private void OnDestroy()
    {
        currentPlayersTurn.OnValueChanged -= HandlePlayerTurnChanged;
    }
}
