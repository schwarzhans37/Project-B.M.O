using UnityEngine;

[RequireComponent(typeof(CreateHostView))]
public class CreateHostController : MonoBehaviour
{
    private int maxPlayerCount = 1;

    public CreateHostView createHostView;
    public CustomNetworkDiscovery networkDiscovery;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (createHostView == null)
                createHostView = GetComponent<CreateHostView>();
        }
#endif

    void Start()
    {
        createHostView.OnCreateHostAttempt += HandleCreateHostAttempt;
        createHostView.OnSelectedMaxPlayerCountAttempt += HandleSelectedMaxPlayerCountAttempt;
       
    }

    void HandleCreateHostAttempt(string name, string password)
    {
        HostModel.Instance.SetHost(name, password);
        CustomNetworkRoomManager.singleton.maxConnections = maxPlayerCount;
        CustomNetworkRoomManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    void HandleSelectedMaxPlayerCountAttempt(int count)
    {
        maxPlayerCount = count;
    }
}
