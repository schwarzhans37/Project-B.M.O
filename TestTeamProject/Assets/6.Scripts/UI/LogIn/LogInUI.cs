using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogInUI : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button webLinkButton;
    private LoginController loginController;

    void Start()
    {
        loginController = GetComponent<LoginController>();
        loginButton.onClick.AddListener(OnLoginButtonClick);
        webLinkButton.onClick.AddListener(OpenLink);
    }

    void OnLoginButtonClick()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        loginController.Login(email, password);
    }

    public void OpenLink()
    {
        Application.OpenURL("http://localhost");
    }

}