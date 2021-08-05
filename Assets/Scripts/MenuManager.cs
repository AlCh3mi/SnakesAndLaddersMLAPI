using System;
using System.Collections;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Transports.UNET;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject joinGameMenu;
    [SerializeField] private GameObject lobby;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField playerNameInputField;

    private bool isConnecting = false;
    
    private MenuState menuState = MenuState.MainMenu;
    void Start()
    {
        SwitchMenu(menuState);
        playerNameInputField.text = PlayerPrefs.GetString("PlayerName");
    }

    private void SwitchMenu(MenuState newState)
    {
        switch (newState)
        {
            case MenuState.MainMenu:
                mainMenu.SetActive(true);
                joinGameMenu.SetActive(false);
                lobby.SetActive(false);
                break;
            case MenuState.JoinGame:
                mainMenu.SetActive(false);
                joinGameMenu.SetActive(true);
                lobby.SetActive(false);
                break;
            case MenuState.Lobby:
                mainMenu.SetActive(false);
                joinGameMenu.SetActive(false);
                lobby.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, "Invalid Menu State.");
        }
    }

    public void BackToMainMenu()
    {
        if(NetworkManager.Singleton.IsHost)
            NetworkManager.Singleton.StopHost();
        else
            NetworkManager.Singleton.StopClient();
        
        SwitchMenu(MenuState.MainMenu);
    }

    public void LeaveButton()
    {
        if (NetworkManager.Singleton.IsHost)
            NetworkManager.Singleton.StopHost();
        else if (NetworkManager.Singleton.IsClient)
            NetworkManager.Singleton.StopClient();
        
        SwitchMenu(MenuState.MainMenu);
    }

    public void HostGame()
    {
        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);
        if(NetworkManager.Singleton.StartHost().Success)
            SwitchMenu(MenuState.Lobby);
    }

    public void StartGame()
    {
        NetworkSceneManager.SwitchScene("Game");
    }

    public void JoinButtonMainMenu()
    {
        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);
        SwitchMenu(MenuState.JoinGame);
    }

    public void JoinGameAsClient()
    {
        if(ipInputField.text == "")
            NetworkManager.Singleton.StartClient();
        else
        {
            NetworkManager.Singleton.gameObject.GetComponent<UNetTransport>().ConnectAddress = ipInputField.text;
            NetworkManager.Singleton.StartClient();
        }
        if(!isConnecting)
            StartCoroutine(WaitingToConnect());
    }

    IEnumerator WaitingToConnect()
    {
        isConnecting = true;
        var timeOutTimer = 10f;
        while (!NetworkManager.Singleton.IsConnectedClient)
        {
            timeOutTimer -= Time.deltaTime;
            if (timeOutTimer <= 0)
                break;
            yield return null;
        }
        
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            SwitchMenu(MenuState.Lobby);
        }
        else
        {
            Debug.Log("Unable To Connect.");
            NetworkManager.Singleton.StopClient();
        }
        isConnecting = false;
    }
}
