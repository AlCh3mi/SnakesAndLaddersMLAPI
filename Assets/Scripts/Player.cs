using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Serialization;
using UnityEngine;

public class Player : NetworkBehaviour, INetworkSerializable
{
    private string PlayerName;
    private ulong ClientId;
    public int CurrentPosition;

    public GameObject[] models;

    private NetworkVariableInt model = new NetworkVariableInt();

    public override void NetworkStart()
    {
        if (IsHost)
        {
            model.Value = Random.Range(0, models.Length);
        }

        if (IsClient)
        {
            for (int i = 0; i < models.Length; i++)
            {
                if(i == model.Value)
                    models[i].SetActive(true);
                else
                {
                    models[i].SetActive(false);
                }
            }
        }
    }

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
