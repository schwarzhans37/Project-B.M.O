using UnityEngine;

public class LoginController : MonoBehaviour
{
    public LoginView loginView;

    void Start()
    {
        loginView.OnLoginAttempt += HandleLoginAttempt;
        loginView.OnWebLinkAttempt += HandleWebLinkAttempt;
    }

    async void HandleLoginAttempt(string email, string password)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");

        /*
        ApiResponse response = await UserModel.Instance.Login(email, password);

        if (response.status == 200)
        {
            loginView.ShowMessage(response.message);

            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            loginView.ShowMessage(response.message);
        }
        */
    }

    void HandleWebLinkAttempt()
    {
        Application.OpenURL("http://localhost");
    }

}
