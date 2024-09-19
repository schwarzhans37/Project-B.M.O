using Mirror;
using UnityEngine;

public class CustomNetworkRoomManager : NetworkRoomManager
{
    public override void Start()
    {
        base.Start();

        showRoomGUI = false; // 기본 UI 비활성화
    }

    // 호스트에서 Start()가 호출될 때 실행
    public override void OnRoomStartHost()
    {
        base.OnRoomStartHost();
        
    }

    // 호스트가 중지될 때 호출
    public override void OnRoomStopHost()
    {
        base.OnRoomStopHost();
        
    }

    // 서버에서 Start()가 호출될 때 실행
    public override void OnRoomStartServer()
    {
        base.OnRoomStartServer();
        
    }

    // 서버가 중지될 때 호출
    public override void OnRoomStopServer()
    {
        base.OnRoomStopServer();
        
    }

    // 클라이언트가 서버에 연결될 때 호출
    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);
        
    }

    // 클라이언트가 서버에서 연결 해제될 때 호출
    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerDisconnect(conn);
        
    }

    // 서버의 장면이 변경될 때 호출
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);
        
    }

    // 새로운 룸 플레이어가 생성될 때 서버에서 호출
    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = base.OnRoomServerCreateRoomPlayer(conn);
        
        return player;
    }

    // 새로운 게임 플레이어가 생성될 때 서버에서 호출
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        GameObject gamePlayer = base.OnRoomServerCreateGamePlayer(conn, roomPlayer);
        
        return gamePlayer;
    }

    // 플레이어가 추가될 때 서버에서 호출
    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnRoomServerAddPlayer(conn);
        
    }

    // 플레이어가 장면에 로드될 때 서버에서 호출
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        bool result = base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
        
        return result;
    }

    // 모든 플레이어가 준비되었을 때 서버에서 호출
    public override void OnRoomServerPlayersReady()
    {
                
    }

    // 플레이어가 준비되지 않았을 때 서버에서 호출
    public override void OnRoomServerPlayersNotReady()
    {
        base.OnRoomServerPlayersNotReady();
        
    }

    // 플레이어의 준비 상태가 변경될 때 호출
    public override void ReadyStatusChanged()
    {
        base.ReadyStatusChanged();
        
    }

    // 클라이언트가 룸에 입장할 때 호출
    public override void OnRoomClientEnter()
    {
        base.OnRoomClientEnter();
        
    }

    // 클라이언트가 룸에서 나갈 때 호출
    public override void OnRoomClientExit()
    {
        base.OnRoomClientExit();
        
    }

    // 클라이언트가 서버에 연결될 때 호출
    public override void OnRoomClientConnect()
    {
        base.OnRoomClientConnect();
        
    }

    // 클라이언트가 서버에서 연결 해제될 때 호출
    public override void OnRoomClientDisconnect()
    {
        base.OnRoomClientDisconnect();
        
    }

    // 클라이언트에서 Start()가 호출될 때 실행
    public override void OnRoomStartClient()
    {
        base.OnRoomStartClient();
        
    }

    // 클라이언트가 중지될 때 호출
    public override void OnRoomStopClient()
    {
        base.OnRoomStopClient();
        
    }

    // 클라이언트의 장면이 변경될 때 호출
    public override void OnRoomClientSceneChanged()
    {
        base.OnRoomClientSceneChanged();
        
    }
}