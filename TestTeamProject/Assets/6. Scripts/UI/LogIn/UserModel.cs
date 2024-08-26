using System.Threading.Tasks;
using UnityEngine;

public class UserModel: MonoBehaviour
{
    public static UserModel Instance { get; private set; }
    public string Nickname { get; private set; }

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

    public async Task<ApiResponse> Login(string email, string password)
    {
        LoginData loginData = new()
        {
            email = email,
            password = password
        };

        ApiResponse response = await RequestManager.Instance.Post<LoginData, ApiResponse>("/login", loginData);
        return response;
    }
}

[System.Serializable]
public class LoginData
{
    public string email;
    public string password;
}