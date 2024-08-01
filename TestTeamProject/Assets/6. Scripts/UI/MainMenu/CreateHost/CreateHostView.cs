using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CreateHostView : MonoBehaviour
{    public List<Button> maxPlayerButtons;
    public Button createHostButton;
    public Button cancelButton;
    public Button mainButton;
    public TMP_InputField hostNameInput;
    public TMP_InputField hostPasswordInput;

    private int maxPlayerCount = 1;

    public event System.Action<string, string, int> OnCreateHostAttempt;
    void Start()
    {
        for (int i = 0; i < maxPlayerButtons.Count; i++)
        {
            int index = i;
            maxPlayerButtons[i].onClick.AddListener(() => UpdateMaxPlayerCount(index + 1));
        }
        createHostButton.onClick.AddListener(() =>
        {
            OnCreateHostAttempt?.Invoke(hostNameInput.text, hostPasswordInput.text, maxPlayerCount);
        });
        cancelButton.onClick.AddListener(() =>
        {
            hostNameInput.text = "";
            hostPasswordInput.text = "";

            gameObject.SetActive(false);
        });
        mainButton.onClick.AddListener(() =>
        {
            hostNameInput.text = "";
            hostPasswordInput.text = "";

            gameObject.SetActive(false);
        });
    }

    void UpdateMaxPlayerCount(int count)
    {
        maxPlayerCount = count;

        for (int i = 0; i < maxPlayerButtons.Count; i++)
        {
            if (i == count - 1)
            {
                maxPlayerButtons[i].image.enabled = true;
            }
            else
            {
                maxPlayerButtons[i].image.enabled = false;
            }
        }
    }
}
