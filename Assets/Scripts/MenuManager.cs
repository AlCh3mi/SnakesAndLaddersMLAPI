using MLAPI;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject multiplayerMenu;
    
    void Start()
    {
        multiplayerMenu.SetActive(true);
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsHost || !NetworkManager.Singleton.IsClient) return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }
    
    private void ToggleMenu() => multiplayerMenu.SetActive(!multiplayerMenu.gameObject.activeSelf);
    private void ToggleMenu(bool active) => multiplayerMenu.SetActive(active);

    public void HostGame()
    {
        NetworkManager.Singleton.StartHost();
        ToggleMenu(false);
    }

    public void JoinAsClient()
    {
        NetworkManager.Singleton.StartClient();
        ToggleMenu(false);
    }
}
