using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BMORoomManager : NetworkRoomManager
{
    [Header("Custom Settings")]
    public int maxPlayers = 1;

    public override void Awake()
    {
        base.Awake();
        maxConnections = maxPlayers; // 최대 플레이어 수 설정
    }

    // 서버가 시작될 때 호출되는 메서드
    public override void OnStartHost()
    {
        Debug.Log("Server started on");
        base.OnStartHost();
    }
}