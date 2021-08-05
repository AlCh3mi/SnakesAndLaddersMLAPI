using System;
using System.Collections;
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

    public ulong CurrentPlayerTurn()
    {
        return playerTurn.Peek();
    }

    public ulong NextPlayersTurn()
    {
        playerTurn.Enqueue(playerTurn.Dequeue());
        ItsYourTurnClientRpc(playerTurn.Peek());
        return playerTurn.Peek();
    }

    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    private void ItsYourTurnClientRpc(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            StartCoroutine(WaitingForPlayerAction());
        }
    }

    IEnumerator WaitingForPlayerAction()
    {
        var turnDuration = 30f;
        
        var message = Instantiate(onScreenMessagePrefab, GUI.transform);
        message.GetComponent<OnScreenMessage>().Display("Its your turn to play!", "Roll the dice", 3f);

        while (turnDuration >= 0f)
        {
            turnDuration -= Time.deltaTime;
            yield return null;
        }
    }

    private void OnDestroy()
    {
        currentPlayersTurn.OnValueChanged -= HandlePlayerTurnChanged;
    }
}
