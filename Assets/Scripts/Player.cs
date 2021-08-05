using MLAPI.Serialization;
using UnityEngine;

public class Player : MonoBehaviour, INetworkSerializable
{
    private string PlayerName;
    private ulong ClientId;
    public int CurrentPosition;
    
    public void Setup(string playerName, ulong clientId)
    {
        PlayerName = playerName;
        ClientId = clientId;
        CurrentPosition = 0;
    }

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref PlayerName);
        serializer.Serialize(ref ClientId);
        serializer.Serialize(ref CurrentPosition);
    }
}
