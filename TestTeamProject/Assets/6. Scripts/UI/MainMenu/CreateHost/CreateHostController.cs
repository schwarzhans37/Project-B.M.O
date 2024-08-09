using UnityEngine;

public class CreateHostController : MonoBehaviour
{
    public CreateHostView createHostView;
    public GameObject networkManager;

    void Start()
    {
        createHostView.OnCreateHostAttempt += HandleCreateHostAttempt;
       
    }

    void HandleCreateHostAttempt(string name, string password, int maxPlayerCount)
    {
        networkManager.GetComponent<CustomNetworkRoomManager>().StartHost();
        networkManager.GetComponent<CustomNetworkDiscovery>().AdvertiseServer();
    }
}
