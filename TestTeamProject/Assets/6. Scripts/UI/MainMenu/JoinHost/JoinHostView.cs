using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinHostView : MonoBehaviour
{
    public GameObject hostItemPrefab;
    public TMP_InputField findHostInput;
    public GameObject nicknamePanel;
    public TMP_InputField nicknameInput;
    public Transform contentTransform; 
    public Button joinHostButton;
    public Button cancelButton;

    public event System.Action<string> OnFindHostAttempt;
    public event System.Action<DiscoveryResponse> OnSelectedHostAttempt;
    public event System.Action<string> OnJoinHostAttempt;

    void Start()
    {
        findHostInput.onValueChanged.AddListener((string text) =>
        {
            OnFindHostAttempt?.Invoke(text);
        });
        joinHostButton.onClick.AddListener(() =>
        {
            OnJoinHostAttempt?.Invoke(nicknameInput.text);
        });
        cancelButton.onClick.AddListener(OnCancelButton);
        nicknamePanel.GetComponent<Button>().onClick.AddListener(OnCancelButton);
        gameObject.GetComponent<Button>().onClick.AddListener(OnMainBackgroundButton);
    }

    void OnDisable()
    {
        findHostInput.text = "";
        ClearHosts();
    }

    void OnMainBackgroundButton()
    {
        gameObject.SetActive(false);
    }

    void OnCancelButton()
    {
        nicknameInput.text = "";
        nicknamePanel.SetActive(false);
    }

    public void ShowNicknamePanel()
    {
        nicknamePanel.SetActive(true);
    }

    public void AddHost(DiscoveryResponse info)
    {
        GameObject hostItem = Instantiate(hostItemPrefab, contentTransform);


        hostItem.transform.Find("Title").GetComponent<TMP_Text>().text =
            info.roomTitle != null && info.roomTitle != "" ? info.roomTitle : "No Title";

        hostItem.transform.Find("Nickname").GetComponent<TMP_Text>().text = 
            info.nickname != null && info.nickname != "" ? info.nickname : "No Nickname";
            
        hostItem.transform.Find("CurrentPlayers").GetComponent<TMP_Text>().text = $"{info.currentPlayerCount}  /  {info.maxPlayerCount}";

        if (hostItem.TryGetComponent<Button>(out var button))
        {
            button.onClick.RemoveAllListeners(); // 중복 방지
            button.onClick.AddListener(() =>
            {
                OnSelectedHostAttempt?.Invoke(info);
            });
        }
    }

    public void ClearHosts()
    {
        foreach (Transform item in contentTransform)
        {
            Destroy(item.gameObject);
        }
    }
}
