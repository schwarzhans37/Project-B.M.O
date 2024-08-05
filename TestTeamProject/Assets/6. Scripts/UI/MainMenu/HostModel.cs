using System.Threading.Tasks;
using UnityEngine;

public class HostModel : MonoBehaviour
{
    public static HostModel Instance { get; private set; }
    public string Id { get; private set; }
    public string IpAddress { get; private set; }

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

    public async Task<ApiResponse> CreateHost(string name, string password, string ipAddress, int maxPlayers)
    {
        HostData hostData = new()
        {
            name = name,
            password = password,
            ipAddress = ipAddress,
            currentPlayers = 1,
            maxPlayers = maxPlayers
        };

        ApiResponse response = await RequestManager.Instance.Post<HostData, ApiResponse>("/host/create", hostData);
        return response;
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