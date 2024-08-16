using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinHostView : MonoBehaviour
{
    public GameObject hostItemPrefab;
    public TMP_InputField findHostInput;
    public GameObject confirmPassword;
    public TMP_InputField hostPasswordInput;
    public Transform contentTransform; 
    public Button joinHostButton;
    public Button cancelButton;

    public event System.Action<string> OnFindHostAttempt;
    public event System.Action<DiscoveryResponse> OnSelectedHostAttempt;
    public event System.Action OnJoinHostAttempt;

    void Start()
    {
        findHostInput.onValueChanged.AddListener((string text) =>
        {
            OnFindHostAttempt?.Invoke(text);
        });
        joinHostButton.onClick.AddListener(() =>
        {
            OnJoinHostAttempt?.Invoke();
        });
        cancelButton.onClick.AddListener(OnCancelButton);
        confirmPassword.GetComponent<Button>().onClick.AddListener(OnCancelButton);
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
        hostPasswordInput.text = "";
        confirmPassword.SetActive(false);
    }

    public void ShowConfirmPassword()
    {
        confirmPassword.SetActive(true);
    }

    public void AddHost(DiscoveryResponse info)
    {
        GameObject hostItem = Instantiate(hostItemPrefab, contentTransform);

        if (info.roomName != null && info.roomName != "")
        {
            hostItem.transform.Find("Title").GetComponent<TMP_Text>().text = info.roomName;
        }
        if (info.nickname != null && info.nickname != "")
        {
            hostItem.transform.Find("Nickname").GetComponent<TMP_Text>().text = info.nickname;
        }
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
