using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class JoinHostController : MonoBehaviour
{
    public JoinHostView joinHostView;
    public GameObject networkManager;

    private CancellationTokenSource cancellationTokenSource;
    private string findHostInput = "";

    readonly Dictionary<long, DiscoveryResponse> discoveredServers = new Dictionary<long, DiscoveryResponse>();

    void Start()
    {
        joinHostView.OnFindHostAttempt += HandleFindHostAttempt;
        joinHostView.OnJoinHostAttempt += HandleJoinHostAttempt;
    }

    async void HandleFindHostAttempt(string text)
    {
        Debug.Log("FindHostAttempt: " + text);

        // 이전 작업 취소
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(1000, cancellationTokenSource.Token); // 1초 대기
            findHostInput = text;
            joinHostView.ClearHosts();
            discoveredServers.Clear();
        }
        catch (TaskCanceledException)
        {
            // 작업이 취소된 경우 예외 처리
        }
    }

    void HandleJoinHostAttempt(DiscoveryResponse info)
    {
        Debug.Log("JoinHostAttempt: " + info.uri);
        networkManager.GetComponent<CustomNetworkRoomManager>().StartClient(info.uri);
    }

    public void OnDiscoveredServer(DiscoveryResponse info)
    {
        Debug.Log("DiscoveredServer: " + info.uri);

        if (!discoveredServers.ContainsKey(info.serverId))
        {
            discoveredServers[info.serverId] = info;
            if (info.EndPoint.Address.ToString().Contains(findHostInput))
            {
                joinHostView.AddHost(info);
            }
        }
    }

    void OnEnable()
    {
        discoveredServers.Clear();
        networkManager.GetComponent<CustomNetworkDiscovery>().StartDiscovery();
    }

    void OnDisable()
    {
        discoveredServers.Clear();
        networkManager.GetComponent<CustomNetworkDiscovery>().StopDiscovery();
    }
}
