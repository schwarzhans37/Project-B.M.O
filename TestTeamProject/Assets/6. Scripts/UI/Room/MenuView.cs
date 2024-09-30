using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{
    public Button playButton;
    public Button hostButton;
    public Button settingButton;
    public Button exitButton;
    
    public event System.Action OnPlayAttempt;
    public event System.Action OnHostAttempt;
    public event System.Action OnSettingAttempt;
    public event System.Action OnExitAttempt;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            playButton.enabled = false;
        }

        playButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            OnPlayAttempt?.Invoke();
        });
        hostButton.onClick.AddListener(() =>
        {
            OnHostAttempt?.Invoke();
        });
        settingButton.onClick.AddListener(() =>
        {
            OnSettingAttempt?.Invoke();
        });
        exitButton.onClick.AddListener(() =>
        {
            OnExitAttempt?.Invoke();
        });
    }

    public void ShowMessage(string message)
    {
        Debug.Log($"message: {message}");
    }
}
