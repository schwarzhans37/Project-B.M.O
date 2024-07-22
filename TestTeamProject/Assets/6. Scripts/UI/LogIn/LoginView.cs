using kcp2k;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button webLinkButton;

    public event System.Action<string, string> OnLoginAttempt;

    void Start()
    {
        loginButton.onClick.AddListener(() =>
        {
            OnLoginAttempt?.Invoke(emailInput.text, passwordInput.text);
        });
        webLinkButton.onClick.AddListener(() =>
        {
            Application.OpenURL("http://localhost");
        });
    }

    public void ShowMessage(string message)
    {
        Debug.Log($"message: {message}");
    }

}
