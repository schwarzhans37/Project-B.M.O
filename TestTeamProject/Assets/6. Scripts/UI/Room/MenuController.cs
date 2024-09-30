using UnityEngine;

[RequireComponent(typeof(MenuView))]
public class MenuController : MonoBehaviour
{
    public MenuView menuView;

    void OnValidate()
    {
        if (menuView == null)
            menuView = GetComponent<MenuView>();
    }

    void Start()
    {
        menuView.OnExitAttempt += HandleExitAttempt;
        menuView.OnPlayAttempt += HandlePlayAttempt;
    }

    void HandlePlayAttempt()
    {
        // 게임 일시 정지 해제
        GameUIController.IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void HandleExitAttempt()
    {
        if (CustomNetworkRoomManager.singleton.mode == Mirror.NetworkManagerMode.Host)
        {
            CustomNetworkRoomManager.singleton.StopHost();
        }
        else
        {
            CustomNetworkRoomManager.singleton.StopClient();
        }
    }

}
