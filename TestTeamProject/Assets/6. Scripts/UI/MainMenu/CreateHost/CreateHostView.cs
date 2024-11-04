using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CreateHostView : MonoBehaviour
{   
    public TMP_InputField hostTitleInput;
    public TMP_InputField nicknameInput; 
    public List<Button> maxPlayerButtons;
    public Button createHostButton;
    public Button cancelButton;

    public Sprite normalSprite;
    public Sprite seletedSprite;

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
                OnMaxPlayerButton(index);
            });
        }
        createHostButton.onClick.AddListener(() =>
        {
            OnCreateHostAttempt?.Invoke(hostTitleInput.text, nicknameInput.text);
        });
        cancelButton.onClick.AddListener(OnCancelButton);
        gameObject.GetComponent<Button>().onClick.AddListener(OnCancelButton);
    }

    void OnDisable()
    {
        hostTitleInput.text = "";
        nicknameInput.text = "";
    }

    void OnMaxPlayerButton(int index)
    {
        for (int i = 0; i < maxPlayerButtons.Count; i++)
        {
            maxPlayerButtons[i].image.sprite = normalSprite;
        }
        maxPlayerButtons[index].image.sprite = seletedSprite;
    }

    void OnCancelButton()
    {
        gameObject.SetActive(false);
    }
}
