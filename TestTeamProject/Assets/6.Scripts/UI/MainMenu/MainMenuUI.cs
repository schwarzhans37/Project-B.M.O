using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button quitButton;
    public Button webLinkButton;

    void Start()
    {
        quitButton.onClick.AddListener(OnQuitButtonClick);
        webLinkButton.onClick.AddListener(OpenLink);
    }

    public void OnQuitButtonClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OpenLink()
    {
        Application.OpenURL("http://localhost");
    }
}
