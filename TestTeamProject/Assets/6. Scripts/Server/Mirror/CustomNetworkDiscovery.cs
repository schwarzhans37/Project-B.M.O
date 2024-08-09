using Mirror;
using Mirror.Discovery;
using System;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    // 클라이언트로부터의 요청을 처리
    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        try
        {
            // 응답 메시지
            return new DiscoveryResponse
            {
                serverId = ServerId,
                uri = transport.ServerUri()
            };
        }
        catch (NotImplementedException)
        {
            Debug.LogError($"Transport {transport}는 네트워크 디스커버리를 지원하지 않습니다");
            throw;
        }
    }

    // 서버를 발견하기 위해 네트워크에 브로드캐스트될 메시지를 생성
    // 브로드캐스트할 데이터가 포함된 DiscoveryRequest 인스턴스
    protected override DiscoveryRequest GetRequest() => new DiscoveryRequest();


    /// 서버로부터의 응답을 처리
    /// 클라이언트가 서버로부터 응답을 받으면, 이 메서드는 응답을 처리하고 이벤트를 발생
    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        // 원격 엔드포인트로부터 메시지를 받음
        response.EndPoint = endpoint;

        // 유효한 URL을 받았지만, 제공된 호스트를 해석할 수 없는 경우가 있음
        // 그러나 방금 패킷을 받았으므로 서버의 실제 IP 주소를 알고 있음
        // 따라서 이를 호스트로 사용
        UriBuilder realUri = new UriBuilder(response.uri)
        {
            Host = response.EndPoint.Address.ToString()
        };
        response.uri = realUri.Uri;

        OnServerFound.Invoke(response);
    }
}


[Serializable]
public class DiscoveryRequest : NetworkMessage { }

[Serializable]
public class DiscoveryResponse : NetworkMessage
{
    // 이 서버를 보낸 서버
    // 이 속성은 직렬화되지 않지만, 
    // 클라이언트가 수신 후 이를 채움
    public IPEndPoint EndPoint { get; set; }

    public Uri uri;

    // LAN에서 여러 NIC를 통해 연결할 수 있을 때 중복 서버가 나타나는 것을 방지
    public long serverId;
}