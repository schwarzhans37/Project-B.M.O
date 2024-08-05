using UnityEngine;

public class CreateHostController : MonoBehaviour
{
    public CreateHostView createHostView;
    private CustomNetworkRoomManager manager;

    void Start()
    {
        createHostView.OnCreateHostAttempt += HandleCreateHostAttempt;

        manager = (CustomNetworkRoomManager) CustomNetworkRoomManager.singleton;
    }

    void HandleCreateHostAttempt(string name, string password, int maxPlayerCount)
    {
        manager.StartServer();
    }
}
