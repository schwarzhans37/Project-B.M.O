using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            // manager.networkAddress = hostData.ipAddress;
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

    public async void JoinHost(HostData hostData)
    {
        // 요청 생성 및 보내기
        ApiResponse response = await RequestManager.Instance.Post<HostData, ApiResponse>("/host/join", hostData);

        if (response.status == 200)
        {
            Debug.Log($"Success: {response.message}");
            manager.networkAddress = (string) response.date;
            manager.StartClient();
        }
        else
        {
            Debug.LogError($"Error: {response.message}");
        }
    }

    public async Task<List<HostData>> GetHostList()
    {
        // 요청 생성 및 보내기
        ApiResponse response = await RequestManager.Instance.Get<ApiResponse>("/host");

        if (response.status == 200)
        {
            Debug.Log($"Success: {response.message}");
            return JsonConvert.DeserializeObject<List<HostData>>(response.date.ToString());
        }
        else
        {
            Debug.LogError($"Error: {response.message}");
            return new List<HostData>();
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