using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogInUI : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    private LoginController loginController;

    void Start()
    {
        loginController = GetComponent<LoginController>();
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    void OnLoginButtonClick()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        StartCoroutine(loginController.Login(email, password));
    }

}