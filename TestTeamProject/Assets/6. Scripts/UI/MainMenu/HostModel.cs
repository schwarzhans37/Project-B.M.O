using System;
using System.Threading.Tasks;
using UnityEngine;

public class HostModel : MonoBehaviour
{
    public static HostModel Instance { get; private set; }
    public long ServerId { get; private set; }
    public string RoomName { get; private set; }
    public string Password { get; private set; }
    public Uri Uri { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetHost(string roomName, string password)
    {
        this.RoomName = roomName;
        this.Password = password;
    }

    public async Task<ApiResponse> CreateHost(string name, string password, string uri, int maxPlayers)
    {
        HostData hostData = new()
        {
            name = name,
            password = password,
            ipAddress = uri,
            currentPlayers = 1,
            maxPlayers = maxPlayers
        };

        ApiResponse response = await RequestManager.Instance.Post<HostData, ApiResponse>("/host/create", hostData);
        return response;
    }

    public async Task<ApiResponse> JoinHost(HostData hostData)
    {
        ApiResponse response = await RequestManager.Instance.Post<HostData, ApiResponse>("/host/join", hostData);
        return response;
    }

    public async Task<ApiResponse> GetHostList()
    {
        ApiResponse response = await RequestManager.Instance.Get<ApiResponse>("/host");
        return response;
        // JsonConvert.DeserializeObject<List<HostData>>(response.date.ToString());
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