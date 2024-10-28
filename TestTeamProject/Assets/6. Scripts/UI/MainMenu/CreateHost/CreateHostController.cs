using System.Text.RegularExpressions;
using Mirror;
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
        CustomNetworkRoomManager roomManager = NetworkManager.singleton as CustomNetworkRoomManager;
        if (IsValidIpAddress(title))
        {
            CustomNetworkRoomManager.Nickname = nickname;
            if (maxPlayerCount == 1)
            {
                roomManager.networkAddress = title;
                roomManager.StartClient();
                return;
            }
            
            roomManager.maxConnections = maxPlayerCount;
            roomManager.StartHost();
            return;
        }

        HostModel.Instance.SetHost(title, nickname);
        CustomNetworkRoomManager.RoomTitle = title;
        CustomNetworkRoomManager.Nickname = nickname;
        roomManager.maxConnections = maxPlayerCount;
        roomManager.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    void HandleSelectedMaxPlayerCountAttempt(int count)
    {
        maxPlayerCount = count;
    }

    public bool IsValidIpAddress(string ipAddress)
    {
        // 정규 표현식 패턴
        string pattern = @"^(\d{1,3}\.){3}\d{1,3}$";

        // 정규 표현식을 사용해 검사
        Regex regex = new Regex(pattern);
        if (!regex.IsMatch(ipAddress))
        {
            return false;
        }

        // 각 숫자 범위가 0에서 255 사이인지 확인
        string[] segments = ipAddress.Split('.');
        foreach (string segment in segments)
        {
            if (int.Parse(segment) < 0 || int.Parse(segment) > 255)
            {
                return false;
            }
        }

        return true;
    }
}
