using UnityEngine;
using UnityEngine.Networking;

public class RequestManager : MonoBehaviour
{
    public static RequestManager Instance { get; private set; }
    public string SessionCookie { get; private set; }
    public ApiResponse LastResponse { get; private set; }
    private string ServerUrl { get; set; } = "http://localhost:8080/game";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 요청에 세션 쿠키를 추가하는 메소드
    private void AddSessionCookieToRequest(UnityWebRequest request)
    {
        if (!string.IsNullOrEmpty(SessionCookie))
        {
            request.SetRequestHeader("Cookie", SessionCookie);
        }
    }

    // 응답에서 세션 쿠키를 갱신하는 메소드
    public void UpdateSessionCookieFromResponse(UnityWebRequest request)
    {
        string newSessionCookie = request.GetResponseHeader("Set-Cookie");
        if (!string.IsNullOrEmpty(newSessionCookie))
        {
            SessionCookie = newSessionCookie;
        }
    }

    // UnityWebRequest 객체를 생성하고 설정하는 메소드
    public UnityWebRequest CreateRequest(string endpoint, string method, string jsonData)
    {
        string url = ServerUrl + endpoint;
        UnityWebRequest request = new UnityWebRequest(url, method);

        if (!string.IsNullOrEmpty(jsonData))
        {
            byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(byteData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
        }
        else
        {
            request.downloadHandler = new DownloadHandlerBuffer();
        }
        
        AddSessionCookieToRequest(request);

        return request;
    }

    // JSON 데이터가 없는 요청 생성
    public UnityWebRequest CreateRequest(string endpoint, string method)
    {
        string url = ServerUrl + endpoint;
        UnityWebRequest request = new UnityWebRequest(url, method);

        request.downloadHandler = new DownloadHandlerBuffer();
        
        AddSessionCookieToRequest(request);

        return request;
    }

    // UnityWebRequest를 처리하여 ApiResponse 객체를 반환하는 메소드
    public ApiResponse CreateResponseFromRequest(UnityWebRequest request)
    {
        string responseText = request.downloadHandler.text;
        ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseText);
        LastResponse = response; // 마지막 응답 업데이트
        return response;
    }
}
