using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Org.BouncyCastle.Security;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEditor.UI;

public class CreateRoom : MonoBehaviour
{
    public void CreateRoomManage()
    {
        var manager = BMORoomManager.singleton;
        manager.StartHost();
    }
}
public class CreateGameRoomData
{
    public int PlayerCount;
    public int MaxPlayerCount;
}