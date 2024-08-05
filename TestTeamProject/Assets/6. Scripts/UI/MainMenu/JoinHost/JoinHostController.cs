using UnityEngine;

public class JoinHostController : MonoBehaviour
{
    public JoinHostView joinHostView;
    private CustomNetworkRoomManager manager;

    void Start()
    {
        joinHostView.OnFindHostAttempt += HandleFindHostAttempt;
        joinHostView.OnJoinHostAttempt += HandleJoinHostAttempt;

        manager = (CustomNetworkRoomManager) CustomNetworkRoomManager.singleton;
    }

    void HandleFindHostAttempt(string text)
    {
        
    }

    void HandleJoinHostAttempt(HostData hostData)
    {
        
    }
}
