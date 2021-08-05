using System;
using MLAPI.Serialization;
using UnityEngine;

public class Player : MonoBehaviour, INetworkSerializable
{
    private string PlayerName;
    private ulong ClientId;
    public int CurrentPosition;

    public GameObject[] models;

    private void Start()
    {
        var randomModel = UnityEngine.Random.Range(0, models.Length);
        for (int i = 0; i < models.Length; i++)
        {
            if(i == randomModel)
                models[i].SetActive(true);
            else
            {
                models[i].SetActive(false);
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
