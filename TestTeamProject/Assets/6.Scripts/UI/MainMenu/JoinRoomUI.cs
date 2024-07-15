using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomUI : MonoBehaviour
{
    public Button joinRoomButton;
    public Button confirmPasswordButton;
    public TMP_InputField hostPasswordInput;
    private HostController hostController;
    public GameObject roomItemPrefab;
    public Transform contentTransform;
    private List<HostData> hostDatas;
    private HostData targetHost;

    private async void Start()
    {
        hostController = GetComponent<HostController>();
        confirmPasswordButton.onClick.AddListener(OnConfirmPasswordButtonClick);
        await UpdateHosts();
        DisplayRoomItems();
    }

    private void DisplayRoomItems()
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

    private void UpdateRoomItem(GameObject roomItem, HostData hostData)
    {
        targetHost = hostData;
        foreach (Transform item in contentTransform)
        {
            if (item.gameObject == roomItem)
            {
                item.GetComponent<Image>().color = new Color(0f, 0f, 1f, 1f);
            }
            else
            {
                item.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
        joinRoomButton.interactable = true;
    }
    
    private void OnConfirmPasswordButtonClick()
    {
        string password = hostPasswordInput.text;
        targetHost.password = password;
        hostController.JoinHost(targetHost);
    }

    private async Task UpdateHosts()
    {
        hostDatas = await hostController.GetHostList();
    }
}