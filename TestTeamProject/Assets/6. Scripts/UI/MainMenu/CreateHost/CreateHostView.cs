using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CreateHostView : MonoBehaviour
{   
    public TMP_InputField hostNameInput;
    public TMP_InputField hostPasswordInput; 
    public List<Button> maxPlayerButtons;
    public Button createHostButton;
    public Button cancelButton;

    private int maxPlayerCount = 1;

    public event System.Action<string, string, int> OnCreateHostAttempt;
    
    void Start()
    {
        for (int i = 0; i < maxPlayerButtons.Count; i++)
        {
            int index = i;
            maxPlayerButtons[i].onClick.AddListener(() => 
            {
                maxPlayerCount = index + 1;
            });
        }
        createHostButton.onClick.AddListener(() =>
        {
            OnCreateHostAttempt?.Invoke(hostNameInput.text, hostPasswordInput.text, maxPlayerCount);
        });
        cancelButton.onClick.AddListener(OnCancelButton);
        gameObject.GetComponent<Button>().onClick.AddListener(OnCancelButton);
    }

    void OnCancelButton()
    {
        hostNameInput.text = "";
        hostPasswordInput.text = "";

        gameObject.SetActive(false);
    }
}
