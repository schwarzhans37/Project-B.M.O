using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    public GameObject createHostUI;
    public GameObject joinHostUI;
    public GameObject settingUI;
    public Button createHostButton;
    public Button joinHostButton;
    public Button settingButton;
    public Button exitButton;
    public Button webLinkButton;

    public event System.Action OnCreateHostAttempt;
    public event System.Action OnJoinHostAttempt;
    public event System.Action OnSettingAttempt;
    public event System.Action OnExitAttempt;
    public event System.Action OnWebLinkAttempt;

    void Start()
    {
        createHostButton.onClick.AddListener(() =>
        {
            OnCreateHostAttempt?.Invoke();
            if (createHostUI != null)
            {
                createHostUI.SetActive(true);
            }
        });
        joinHostButton.onClick.AddListener(() =>
        {
            OnJoinHostAttempt?.Invoke();
            if (joinHostUI != null)
            {
                joinHostUI.SetActive(true);
            }
        });
        settingButton.onClick.AddListener(() =>
        {
            OnSettingAttempt?.Invoke();
            if (settingUI != null)
            {
                settingUI.SetActive(true);
            }
        });
        exitButton.onClick.AddListener(() =>
        {
            OnExitAttempt?.Invoke();
        });
        webLinkButton.onClick.AddListener(() =>
        {
            OnWebLinkAttempt?.Invoke();
        });
    }

    public void ShowMessage(string message)
    {
        Debug.Log($"message: {message}");
    }

}
