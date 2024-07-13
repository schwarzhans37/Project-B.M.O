using UnityEngine;

public class LoginController : MonoBehaviour
{
    public async void Login(string email, string password)
    {
        // LoginData 객체 생성 및 값 설정
        LoginData loginData = new LoginData();
        loginData.email = email;
        loginData.password = password;

        // 요청 생성 및 보내기
        ApiResponse response = await RequestManager.Instance.Post<LoginData, ApiResponse>("/login", loginData);

        if (response.status == 200)
        {
            Debug.Log($"success: {response.message}");

            // 로그인 성공 시 메인 메뉴 씬으로 이동
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            Debug.Log($"error: {response.message}");
        }
    }
}

[System.Serializable]
public class LoginData
{
    public string email;
    public string password;
}