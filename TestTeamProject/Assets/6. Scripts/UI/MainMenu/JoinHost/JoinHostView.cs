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

    private DiscoveryResponse selectedHost;

    private Dictionary<Button, float> buttonLastClickThreshold = new Dictionary<Button, float>();
    private const float doubleClickThreshold = 0.3f; // 더블 클릭으로 인식할 시간 간격 (초)

    public event System.Action<string> OnFindHostAttempt;
    public event System.Action<DiscoveryResponse> OnJoinHostAttempt;

    void Start()
    {
        findHostInput.onValueChanged.AddListener((string text) =>
        {
            OnFindHostAttempt?.Invoke(text);
        });
        joinHostButton.onClick.AddListener(() =>
        {
            OnJoinHostAttempt?.Invoke(selectedHost);
        });
        cancelButton.onClick.AddListener(OnCancelButton);
        confirmPassword.GetComponent<Button>().onClick.AddListener(OnCancelButton);
        gameObject.GetComponent<Button>().onClick.AddListener(OnMainBackgroundButton);
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

    public void AddHost(DiscoveryResponse info)
    {
        GameObject hostItem = Instantiate(hostItemPrefab, contentTransform);
        hostItem.transform.Find("Title").GetComponent<TMP_Text>().text = info.EndPoint.Address.ToString();
        hostItem.transform.Find("Nickname").GetComponent<TMP_Text>().text = info.serverId.ToString();

        if (hostItem.TryGetComponent<Button>(out var button))
        {
            button.onClick.RemoveAllListeners(); // 중복 방지
            button.onClick.AddListener(() => SelectHost(button, info));
        }
    }

    public void ClearHosts()
    {
        buttonLastClickThreshold.Clear();
        foreach (Transform item in contentTransform)
        {
            Destroy(item.gameObject);
        }
    }

    void SelectHost(Button button, DiscoveryResponse info)
    {
        selectedHost = info;

        if (buttonLastClickThreshold.ContainsKey(button))
        {
            if (Time.time - buttonLastClickThreshold[button] < doubleClickThreshold)
            {
                confirmPassword.SetActive(true);
            }
        }

        buttonLastClickThreshold[button] = Time.time;
    }

    void OnEnable()
    {
        findHostInput.text = "ALL";
        findHostInput.text = "";
    }

    void OnDisable()
    {
        findHostInput.text = "";
        ClearHosts();
    }
}
