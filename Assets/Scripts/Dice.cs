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
    [SerializeField] private TMP_Text diceText;
    
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

    private void RollDice()
    {
        LastRoll.Value = Random.Range(1, 7);
        LastRoll.SetDirty(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsClient)
        {
            RequestDiceRollServerRpc();
            Debug.Log("Dice Roll Request Sent to Server");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestDiceRollServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //if its the players turn, allow the diceroll, else reject.
        if(serverRpcParams.Receive.SenderClientId == gameSequenceHandler.CurrentPlayerTurn())
        {
            RollDice();
            gameSequenceHandler.NextPlayersTurn();
        }
    }
}
