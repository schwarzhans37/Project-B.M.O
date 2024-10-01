using UnityEngine;

[RequireComponent(typeof(CreateHostView))]
public class CreateHostController : MonoBehaviour
{
    private int maxPlayerCount = 1;

    public CreateHostView createHostView;
    public CustomNetworkDiscovery networkDiscovery;

    void OnValidate()
    {
        createHostView = GetComponent<CreateHostView>();
    }

    void Start()
    {
        createHostView.OnCreateHostAttempt += HandleCreateHostAttempt;
        createHostView.OnSelectedMaxPlayerCountAttempt += HandleSelectedMaxPlayerCountAttempt;
       
    }

    void HandleCreateHostAttempt(string title, string nickname)
    {
        HostModel.Instance.SetHost(title, nickname);
        CustomNetworkRoomManager.RoomTitle = title;
        CustomNetworkRoomManager.Nickname = nickname;
        CustomNetworkRoomManager.singleton.maxConnections = maxPlayerCount;
        CustomNetworkRoomManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    void HandleSelectedMaxPlayerCountAttempt(int count)
    {
        maxPlayerCount = count;
    }
}
