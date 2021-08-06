using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class GameSequenceHandler : NetworkBehaviour
{
    [SerializeField] private GameObject onScreenMessagePrefab;
    [SerializeField] private Canvas GUI;
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource loseSound;
    [SerializeField] private ParticleSystem winParticles;

    private Queue<ulong> playerTurn = new Queue<ulong>();
    public NetworkVariableBool gameOver = new NetworkVariableBool();
    public NetworkVariableInt currentPlayersTurn = new NetworkVariableInt();

    public override void NetworkStart()
    {
        gameOver.Value = false;
        
        foreach (var networkClient in NetworkManager.Singleton.ConnectedClientsList)
        {
            playerTurn.Enqueue(networkClient.ClientId);
        }
    }

    public void NextPlayersTurn()
    {
        playerTurn.Enqueue(playerTurn.Dequeue());
        var nextPlayer = playerTurn.Peek();
        
        if(!gameOver.Value)
        {
            ItsYourTurnClientRpc(nextPlayer);
            currentPlayersTurn.Value = (int) nextPlayer;
        }
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
    
    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    public void WinAchievedClientRpc(ulong clientId)
    {
        winParticles.gameObject.SetActive(true);
        
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            winSound.Play();
            var winMessage = Instantiate(onScreenMessagePrefab, GUI.transform);
            winMessage.GetComponent<OnScreenMessage>()
                .Display("YOU WIN!!!", "Well done!", float.MaxValue, false);
        }
        else
        {
            loseSound.Play();
            var loseMessage = Instantiate(onScreenMessagePrefab, GUI.transform);
            loseMessage.GetComponent<OnScreenMessage>()
                .Display("YOU LOSE!!!", "better luck next time.", float.MaxValue, false);
        }
    }
}
