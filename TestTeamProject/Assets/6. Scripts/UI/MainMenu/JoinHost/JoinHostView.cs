using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinHostView : MonoBehaviour
{
    public GameObject hostItemPrefab;
    public TMP_InputField findHostInput;
    public GameObject confirmPassword;
    public TMP_InputField hostPasswordInput; 
    public Button joinHostButton;
    public Button cancelButton;

    private HostData selectedHost;

    public event System.Action<string> OnFindHostAttempt;
    public event System.Action<HostData> OnJoinHostAttempt;

    void Start()
    {
        findHostInput.onValueChanged.AddListener((string text) =>
        {
            OnFindHostAttempt?.Invoke(text);
        });
        joinHostButton.onClick.AddListener(() =>
        {
            selectedHost.password = hostPasswordInput.text;
            OnJoinHostAttempt?.Invoke(selectedHost);
        });
        cancelButton.onClick.AddListener(OnCancelButton);
        confirmPassword.GetComponent<Button>().onClick.AddListener(OnCancelButton);
        gameObject.GetComponent<Button>().onClick.AddListener(OnMainBackgroundButton);
    }

    void OnMainBackgroundButton()
    {
        findHostInput.text = "";

        gameObject.SetActive(false);
    }

    void OnCancelButton()
    {
        hostPasswordInput.text = "";

        confirmPassword.SetActive(false);
    }

    /*
    void DisplayRoomItems()
    {
        foreach (Transform item in contentTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (HostData item in hostDatas)
        {
            GameObject roomItem = Instantiate(roomItemPrefab, contentTransform);
            HostData hostData = item;
            roomItem.transform.Find("Field/Title").GetComponent<TMP_Text>().text = hostData.name;
            roomItem.transform.Find("Field/Nickname").GetComponent<TMP_Text>().text = hostData.nickname;
            roomItem.transform.Find("Field/Players/CurrentPlayers").GetComponent<TMP_Text>().text = hostData.currentPlayers.ToString();
            roomItem.transform.Find("Field/Players/MaxPlayers").GetComponent<TMP_Text>().text = hostData.maxPlayers.ToString();

            // 버튼 클릭 이벤트 등록
            Button button = roomItem.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // 중복 방지
                button.onClick.AddListener(() => UpdateRoomItem(roomItem, hostData));
            }
        }
    }
    */
}
