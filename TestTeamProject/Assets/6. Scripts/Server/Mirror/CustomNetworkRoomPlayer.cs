using System.Collections;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomNetworkRoomPlayer : NetworkRoomPlayer
{
    public GameObject RoomPlayerStatusPrefab;
    public Transform contentTransform;

    [SyncVar(hook = nameof(OnNicknameChanged))]
    public string nickname;

    public override void Start()
    {
        showRoomGUI = false; // 기본 UI 비활성화

        if (isLocalPlayer)
        {
            string playerNickname = "Player " + index; // 기본 닉네임 설정
            CmdSetNickname(playerNickname); // 서버에 닉네임 전송
        }

        if (contentTransform == null)
        {
            contentTransform = GameObject.Find("GridLayout").transform;
        }

        base.Start();
    }

    // 일정 시간을 기다렸다가 실행하는 코루틴
    IEnumerator WaitAndExecute(float waitTime, System.Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    // 서버에 닉네임을 전송하는 커맨드 함수
    [Command]
    public void CmdSetNickname(string playerNickname)
    {
        nickname = playerNickname;
    }

    // 닉네임이 변경되면 호출되는 함수
    private void OnNicknameChanged(string oldNickname, string newNickname)
    {
        StartCoroutine(WaitAndExecute(0.1f, () =>
        {
            DrawPlayerStatus();
        }));
    }

    // 플레이어의 인덱스가 변경될 때 호출
    // oldIndex: 이전 인덱스
    // newIndex: 새로운 인덱스
    public override void IndexChanged(int oldIndex, int newIndex)
    {
        base.IndexChanged(oldIndex, newIndex);
    }

    // 플레이어의 준비 상태가 변경될 때 호출
    // oldReadyState: 이전 준비 상태
    // newReadyState: 새로운 준비 상태
    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        base.ReadyStateChanged(oldReadyState, newReadyState);
        StartCoroutine(WaitAndExecute(0.1f, () =>
        {
            DrawPlayerStatus();
        }));
    }

    // 클라이언트가 룸에 입장할 때 호출
    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        StartCoroutine(WaitAndExecute(0.1f, () =>
        {
            DrawPlayerStatus();
        }));
    }

    // 클라이언트가 룸에서 나갈 때 호출
    public override void OnClientExitRoom()
    {
        base.OnClientExitRoom();
        StartCoroutine(WaitAndExecute(0.1f, () =>
        {
            DrawPlayerStatus();
        }));
    }

    // 방 플레이어 상태를 그리는 함수
    void DrawPlayerStatus()
    {
        if (contentTransform == null)
            return;

        // 기존 상태 UI 제거
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        // 네트워크 룸에서 각 플레이어의 상태 그리기
        if (NetworkManager.singleton is NetworkRoomManager room)
        {
            foreach (CustomNetworkRoomPlayer player in room.roomSlots.Cast<CustomNetworkRoomPlayer>())
            {
                GameObject roomPlayerStatus = Instantiate(RoomPlayerStatusPrefab, contentTransform);
                GameObject remotePlayer = player.gameObject;
                roomPlayerStatus.transform.Find("Nickname").GetComponent<TMP_Text>().text = player.nickname ?? "NO NICKNAME";
                if (player.isLocalPlayer)
                {
                    roomPlayerStatus.transform.Find("Ready").GetComponent<Button>().enabled = true;
                    if (isServer && room.allPlayersReady)
                    {
                        roomPlayerStatus.transform.Find("Ready").GetComponentInChildren<TMP_Text>().text = "Start Game";
                        roomPlayerStatus.transform.Find("Ready").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            room.ServerChangeScene(room.GameplayScene);
                        });
                    }
                    else
                    {
                        roomPlayerStatus.transform.Find("Ready").GetComponentInChildren<TMP_Text>().text = player.readyToBegin ? "Cancel" : "Get Ready";
                        roomPlayerStatus.transform.Find("Ready").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            remotePlayer.GetComponent<CustomNetworkRoomPlayer>().CmdChangeReadyState(!player.readyToBegin);
                        });
                    }
                }
                else
                {
                    roomPlayerStatus.transform.Find("Ready").GetComponent<Button>().enabled = false;
                    roomPlayerStatus.transform.Find("Ready").GetComponentInChildren<TMP_Text>().text = player.readyToBegin ? "Ready" : "Not Ready";
                }
                
                // 서버인 경우 플레이어 제거 버튼 활성화
                if (isServer && player.index > 0)
                {
                    roomPlayerStatus.transform.Find("Remove").gameObject.SetActive(true);
                    roomPlayerStatus.transform.Find("Remove").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        remotePlayer.GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
                    });
                }
            }
        }
    }
}