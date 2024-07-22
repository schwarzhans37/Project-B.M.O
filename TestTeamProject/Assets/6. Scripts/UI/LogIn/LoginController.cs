using UnityEngine;

public class LoginController : MonoBehaviour
{
    public LoginView loginView;

    void Start()
    {
        loginView.OnLoginAttempt += HandleLoginAttempt;
    }

    async void HandleLoginAttempt(string email, string password)
    {
        UserModel userModel = new(email, password);
        ApiResponse response = await userModel.Login();

        if (response.status == 200)
        {
            loginView.ShowMessage(response.message);

            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            loginView.ShowMessage(response.message);
        }
    }

}
