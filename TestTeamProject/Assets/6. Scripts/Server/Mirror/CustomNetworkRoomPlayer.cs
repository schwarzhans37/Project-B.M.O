using Mirror;
using UnityEngine;

public class CustomNetworkRoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public string nickname;

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
        
    }

    // 클라이언트가 룸에 입장할 때 호출
    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        
    }

    // 클라이언트가 룸에서 나갈 때 호출
    public override void OnClientExitRoom()
    {
        base.OnClientExitRoom();

    }
}