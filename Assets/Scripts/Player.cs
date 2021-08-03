using System;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [HideInInspector] public NetworkVariableColor Colour = new NetworkVariableColor(new NetworkVariableSettings());
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private void Start()
    {
        Colour.OnValueChanged += HandleColourChanged;
    }

    private void HandleColourChanged(Color previousvalue, Color newvalue)
    {
        _spriteRenderer.color = newvalue;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton)
        {
            Colour.OnValueChanged -= HandleColourChanged;
        }
    }

    public void Setup(Color colour)
    {
        Colour.Value = colour;
    }
}
