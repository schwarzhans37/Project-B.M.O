using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class HostController : MonoBehaviour
{
    private BMORoomManager manager;

    private void Start()
    {
        manager = (BMORoomManager) BMORoomManager.singleton;
    }

    public async void CreateHost(string name, string password, int maxPlayerCount)
    {
        // HostData 객체 생성 및 값 설정
        HostData hostData = new HostData();
        hostData.name = name;
        hostData.password = password;
        hostData.ipAddress = new WebClient().DownloadString("http://ipinfo.io/ip").Trim();
        hostData.currentPlayers = 1;
        hostData.maxPlayers = maxPlayerCount;

        // 요청 생성 및 보내기
        ApiResponse response = await RequestManager.Instance.Post<HostData, ApiResponse>("/host/create", hostData);

        if (response.status == 200)
        {
            Debug.Log($"Success: {response.message}");

            // 최대 플레이어 수 설정
            manager.maxPlayers = maxPlayerCount;
            manager.maxConnections = manager.maxPlayers;

            // 호스트 서버 생성
            manager.StartHost();
        }
        else
        {
            Debug.LogError($"Error: {response.message}");
        }
    }
}

[System.Serializable]
public class HostData
{
    public string id;
    public string name;
    public string nickname;
    public string password;
    public string ipAddress;
    public int currentPlayers;
    public int maxPlayers;
}