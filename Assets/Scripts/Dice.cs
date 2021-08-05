using System;
using System.Collections;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Dice : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameSequenceHandler gameSequenceHandler;
    [SerializeField] private PlayerHandler playerHandler;
    [SerializeField] private Board board;
    [SerializeField] private TMP_Text diceText;


    private NetworkVariableBool isMoving = new NetworkVariableBool(new NetworkVariableSettings
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.Everyone
    });
    
    public NetworkVariableInt LastRoll = new NetworkVariableInt(
        new NetworkVariableSettings { 
            ReadPermission = NetworkVariablePermission.Everyone, 
            WritePermission = NetworkVariablePermission.ServerOnly
        }
    );

    public override void NetworkStart()
    {
        LastRoll.OnValueChanged += HandleDiceBeingRolled;
        
        if(IsClient)
            diceText.text = LastRoll.Value.ToString();
    }

    private void HandleDiceBeingRolled(int previousvalue, int newvalue)
    {
        //todo Show an animation before displaying the value.
        diceText.text = newvalue.ToString();
    }

    private void RollDice(ulong clientId)
    {
        LastRoll.Value = Random.Range(1, 7);
        LastRoll.SetDirty(true);
        StartCoroutine(DelayedMove(clientId, () =>
        {
            gameSequenceHandler.NextPlayersTurn();
            isMoving.Value = false;
            playerHandler.MovePlayerTo(clientId, board.MoveToIfLandedOn(playerHandler.Players[clientId].CurrentPosition));
        }));
    }

    IEnumerator DelayedMove(ulong clientId, Action onComplete = default)
    {
        isMoving.Value = true;
        for (int i = 0; i < LastRoll.Value; i++)
        {
            playerHandler.MovePieceServerRpc(clientId);
            yield return new WaitForSeconds(1f);
        }
        onComplete?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsClient && !isMoving.Value)
        {
            RequestDiceRollServerRpc();
            Debug.Log("Dice Roll Request Sent to Server");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestDiceRollServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if(serverRpcParams.Receive.SenderClientId == (ulong)gameSequenceHandler.currentPlayersTurn.Value)
        {
            RollDice(serverRpcParams.Receive.SenderClientId);
        }
    }
}
