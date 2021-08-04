using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyCard : MonoBehaviour
{
    public ulong ClientId;

    [SerializeField] private Toggle toggle;
    [SerializeField] private TMP_Text playerName;

    public void UpdateDisplay(LobbyPlayerState playerState)
    {
        ClientId = playerState.ClientId;
        playerName.SetText(playerState.PlayerName);
        toggle.isOn = playerState.IsReady;
    }

    public void DisableDisplay()
    {
        toggle.isOn = false;
        playerName.SetText("Waiting");
    }
}