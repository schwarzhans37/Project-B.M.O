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
    private List<HostData> hostDatas = new List<HostData>{
        new HostData{ name = "Room 1", nickname = "1", currentPlayers = 0, maxPlayers = 0},
        new HostData{ name = "Room 2", nickname = "2", currentPlayers = 0, maxPlayers = 0},
        new HostData{ name = "Room 3", nickname = "3", currentPlayers = 0, maxPlayers = 0},
        new HostData{ name = "Room 4", nickname = "4", currentPlayers = 0, maxPlayers = 0},
        new HostData{ name = "Room 5", nickname = "5", currentPlayers = 0, maxPlayers = 0},
        new HostData{ name = "Room 6", nickname = "6", currentPlayers = 0, maxPlayers = 0},
        new HostData{ name = "Room 7", nickname = "7", currentPlayers = 0, maxPlayers = 0},
        new HostData{ name = "Room 8", nickname = "8", currentPlayers = 0, maxPlayers = 0},
        new HostData{ name = "Room 9", nickname = "9", currentPlayers = 0, maxPlayers = 0},
        new HostData{ name = "Room 10", nickname = "10", currentPlayers = 0, maxPlayers = 0},
    };
    private HostData targetHost;

    private async void Start()
    {
        hostController = GetComponent<HostController>();
        confirmPasswordButton.onClick.AddListener(OnConfirmPasswordButtonClick);
        // await UpdateHosts();
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