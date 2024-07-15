using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BMORoomManager : NetworkRoomManager
{
    [Header("Custom Settings")]
    public int maxPlayers = 1;

    public override void Start()
    {
        base.Start();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // 서버 시작 시 최대 플레이어 수 설정
        maxConnections = maxPlayers;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        // 서버 종료 시 처리할 내용 작성
    }
}