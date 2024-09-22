using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomNetworkRoomPlayer : NetworkRoomPlayer
{
    public GameObject RoomPlayerStatusPrefab;
    public Transform contentTransform;

    // 필수적으로 유지할 컴포넌트들을 리스트로 저장
    private List<System.Type> essentialComponents = new List<System.Type>
    {
        typeof(CustomNetworkRoomPlayer),    // CustomNetworkRoomPlayer 컴포넌트
        typeof(NetworkIdentity),            // Mirror 필수 컴포넌트
        typeof(NetworkTransformReliable),   // (선택 사항) 네트워크 동기화를 위한 컴포넌트
        typeof(Transform)                   // Transform 컴포넌트는 Unity의 모든 오브젝트에 필수
    };

    [SyncVar(hook = nameof(OnNicknameChanged))]
    public string nickname;

    // UI 요소를 찾고 상태를 그리는 코루틴
    IEnumerator WaitForPanelUI()
    {
        while (contentTransform == null)
        {
            GameObject content = GameObject.FindWithTag("Transform");
            if (content != null)
            {
                contentTransform = content.GetComponent<RectTransform>();
            }
            yield return null; // 다음 프레임 대기
        }
        DrawPlayerStatus();
    }

    // 일정 시간을 기다렸다가 실행하는 코루틴
    IEnumerator WaitAndExecute(float waitTime, System.Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    public override void Start()
    {
        showRoomGUI = false; // 기본 UI 비활성화

        if (isLocalPlayer)
        {
            string playerNickname = "Player " + index; // 기본 닉네임 설정
            CmdSetNickname(playerNickname); // 서버에 닉네임 전송
        }

        base.Start();
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

    // 클라이언트가 룸에 입장할 때 호출
    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        StartCoroutine(WaitForPanelUI());
    }

    // 클라이언트가 룸에서 나갈 때 호출
    public override void OnClientExitRoom()
    {
        base.OnClientExitRoom();
        StartCoroutine(WaitAndExecute(0.1f, () =>
        {
            DrawPlayerStatus();
        }));

        DisableUnnecessaryComponents();
        DisableChildObjects();
    }

    private void DisableUnnecessaryComponents()
    {
        // 현재 오브젝트의 모든 컴포넌트를 가져옴
        Component[] components = GetComponents<Component>();

        foreach (Component component in components)
        {
            if (!essentialComponents.Contains(component.GetType()))
            {
                // Behaviour 컴포넌트 비활성화
                if (component is Behaviour behaviour)
                {
                    behaviour.enabled = false;
                }
                // Collider 및 CharacterController 비활성화
                else if (component is Collider collider)
                {
                    collider.enabled = false;
                }
                else if (component is CharacterController characterController)
                {
                    characterController.enabled = false;
                }
            }
        }
    }

    private void DisableChildObjects()
    {
        // 모든 자식 오브젝트들을 비활성화
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}