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

    public event System.Action<string, string> OnCreateHostAttempt;
    public event System.Action<int> OnSelectedMaxPlayerCountAttempt;
    
    void Start()
    {
        for (int i = 0; i < maxPlayerButtons.Count; i++)
        {
            int index = i;
            maxPlayerButtons[i].onClick.AddListener(() => 
            {
                OnSelectedMaxPlayerCountAttempt?.Invoke(index + 1);
            });
        }
        createHostButton.onClick.AddListener(() =>
        {
            OnCreateHostAttempt?.Invoke(hostNameInput.text, hostPasswordInput.text);
        });
        cancelButton.onClick.AddListener(OnCancelButton);
        gameObject.GetComponent<Button>().onClick.AddListener(OnCancelButton);
    }

    void OnDisable()
    {
        hostNameInput.text = "";
        hostPasswordInput.text = "";
    }

    void OnCancelButton()
    {
        gameObject.SetActive(false);
    }
}
