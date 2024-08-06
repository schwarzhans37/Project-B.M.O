using Mirror;
using UnityEngine;

public class CustomGamePlayer : NetworkBehaviour
{
    // 서버와 호스트에서만 호출, Start()와 유사
    public override void OnStartServer()
    {
        base.OnStartServer();
        
    }

    // 서버와 호스트에서만 호출되는 종료 이벤트
    public override void OnStopServer()
    {
        base.OnStopServer();
        
    }

    // 클라이언트와 호스트에서만 호출, Start()와 유사
    public override void OnStartClient()
    {
        base.OnStartClient();
        
    }

    // 클라이언트와 호스트에서만 호출되는 종료 이벤트
    public override void OnStopClient()
    {
        base.OnStopClient();
        
    }

    // 클라이언트와 호스트에서 로컬 플레이어 객체에만 호출, Start()와 유사
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
    }

    // 클라이언트와 호스트에서 로컬 플레이어 객체에만 호출되는 종료 이벤트
    public override void OnStopLocalPlayer()
    {
        base.OnStopLocalPlayer();
        
    }

    // 클라이언트가 권한을 가진 객체에만 호출, Start()와 유사
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        
    }

    // 클라이언트가 권한을 가진 객체에만 호출되는 종료 이벤트
    public override void OnStopAuthority()
    {
        base.OnStopAuthority();
        
    }
}