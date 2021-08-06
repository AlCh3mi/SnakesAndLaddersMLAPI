using System;
using System.Collections;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Dice : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameSequenceHandler gameSequenceHandler;
    [SerializeField] private PlayerHandler playerHandler;
    [SerializeField] private Board board;
    [SerializeField] private Sprite[] diceFaces;
    [SerializeField] private SpriteRenderer spriteRenderer;

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

    public override void NetworkStart() => LastRoll.OnValueChanged += HandleDiceBeingRolled;

    public void OnDestroy() => LastRoll.OnValueChanged -= HandleDiceBeingRolled;

    private void HandleDiceBeingRolled(int previousvalue, int newvalue)
    {
        if(newvalue != 0)
            UpdateDiceFace();
    }

    private void UpdateDiceFace() => spriteRenderer.sprite = diceFaces[LastRoll.Value - 1];
    
    private void RollDice(ulong clientId)
    {
        LastRoll.Value = Random.Range(1, 7);
        LastRoll.SetDirty(true);
        StartCoroutine(DelayedMove(clientId, () =>
        {
            isMoving.Value = false;
            if(!gameSequenceHandler.gameOver.Value)
            {
                gameSequenceHandler.NextPlayersTurn();
                playerHandler.MovePlayerTo(clientId,
                    board.MoveToIfLandedOn(playerHandler.Players[clientId].CurrentPosition));
            }
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
        if(gameSequenceHandler.gameOver.Value) return;
        
        if (IsClient && !isMoving.Value)
        {
            RequestDiceRollServerRpc();
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
