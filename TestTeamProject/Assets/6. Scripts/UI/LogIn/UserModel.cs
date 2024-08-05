using System.Threading.Tasks;
using UnityEngine;

public class UserModel
{
    public string Email { get; private set; }
    public string Password { get; private set; }

    public UserModel(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public async Task<ApiResponse> Login()
    {
        LoginData loginData = new()
        {
            email = Email,
            password = Password
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