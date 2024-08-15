using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

[RequireComponent(typeof(JoinHostView))]
public class JoinHostController : MonoBehaviour
{
    readonly Dictionary<long, DiscoveryResponse> discoveredServers = new Dictionary<long, DiscoveryResponse>();

    private CancellationTokenSource cancellationTokenSource;
    private string findHostInput = "";
    private float doubleClickTime = 0.25f; // 두 번 클릭 사이의 최대 시간
    private float lastClickTime = 0f;
    private DiscoveryResponse selectedHostInfo;

    public JoinHostView joinHostView;
    public CustomNetworkDiscovery networkDiscovery;

    void OnValidate()
    {
        if (joinHostView == null)
            joinHostView = GetComponent<JoinHostView>();
    }

    void Start()
    {
        joinHostView.OnFindHostAttempt += HandleFindHostAttempt;
        joinHostView.OnSelectedHostAttempt += HandleSelectedHostAttempt;
        joinHostView.OnJoinHostAttempt += HandleJoinHostAttempt;
    }

    void OnEnable()
    {
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
    }

    void OnDisable()
    {
        discoveredServers.Clear();
        networkDiscovery.StopDiscovery();
    }

    async void HandleFindHostAttempt(string text)
    {
        // 이전 작업 취소
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(500, cancellationTokenSource.Token);
            findHostInput = text;
            Debug.Log("FindHostAttempt " + text);
            joinHostView.ClearHosts();
            discoveredServers.Clear();
        }
        catch (TaskCanceledException)
        {
            // 작업이 취소된 경우
        }
    }

    void HandleSelectedHostAttempt(DiscoveryResponse info)
    {
        if (Time.time - lastClickTime < doubleClickTime)
        {
            if (selectedHostInfo == info)
            {
                joinHostView.ShowConfirmPassword();
                return;
            }
        }

        selectedHostInfo = info;
        lastClickTime = Time.time;
        Debug.Log("Selected Host" + info.uri);
    }

    void HandleJoinHostAttempt()
    {
        CustomNetworkRoomManager.singleton.StartClient(selectedHostInfo.uri);
        networkDiscovery.StopDiscovery();
    }

    public void OnDiscoveredServer(DiscoveryResponse info)
    {
        if (discoveredServers.ContainsKey(info.serverId))
        {
            return;
        }

        discoveredServers[info.serverId] = info;
        Debug.Log("Discovered server " + info.uri);
        if (info.roomName.Contains(findHostInput))
        {
            joinHostView.AddHost(info);
        }
    }
}
