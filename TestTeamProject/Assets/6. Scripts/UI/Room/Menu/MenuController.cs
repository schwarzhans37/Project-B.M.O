using System.Collections;
using System.Collections.Generic;
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
