using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class RequestManager : MonoBehaviour
{
    public static RequestManager Instance { get; private set; }
    public string SessionToken { get; private set; }
    public string ServerUrl { get; set; } = "http://localhost:8080/game";

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

    private async Task<string> SendRequest(string url, string method, string jsonData = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(ServerUrl + url, method))
        {
            if (jsonData != null)
            {
                byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(byteData);
                request.SetRequestHeader("Content-Type", "application/json");
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            // request.certificateHandler = new BypassCertificate();  // 인증서 검증 우회

            if (!string.IsNullOrEmpty(SessionToken))
            {
                request.SetRequestHeader("Cookie", SessionToken);
            }

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"Error: {request.error}");
                return null;
            }
            else
            {
                string newSessionToken = request.GetResponseHeader("Set-Cookie");
                if (!string.IsNullOrEmpty(newSessionToken))
                {
                    SessionToken = newSessionToken;
                }
                return request.downloadHandler.text;
            }
        }
    }

    public async Task<TResponse> Post<TRequest, TResponse>(string url, TRequest data)
    {
        string jsonData = JsonUtility.ToJson(data);
        string response = await SendRequest(url, UnityWebRequest.kHttpVerbPOST, jsonData);
        return response != null ? JsonUtility.FromJson<TResponse>(response) : default;
    }

    public async Task<TResponse> Get<TResponse>(string url)
    {
        string response = await SendRequest(url, UnityWebRequest.kHttpVerbGET);
        return response != null ? JsonUtility.FromJson<TResponse>(response) : default;
    }

    public async Task<TResponse> Patch<TRequest, TResponse>(string url, TRequest data)
    {
        string jsonData = JsonUtility.ToJson(data);
        string response = await SendRequest(url, "PATCH", jsonData);
        return response != null ? JsonUtility.FromJson<TResponse>(response) : default;
    }

    public async Task<TResponse> Put<TRequest, TResponse>(string url, TRequest data)
    {
        string jsonData = JsonUtility.ToJson(data);
        string response = await SendRequest(url, UnityWebRequest.kHttpVerbPUT, jsonData);
        return response != null ? JsonUtility.FromJson<TResponse>(response) : default;
    }

    public async Task<TResponse> Delete<TResponse>(string url)
    {
        string response = await SendRequest(url, UnityWebRequest.kHttpVerbDELETE);
        return response != null ? JsonUtility.FromJson<TResponse>(response) : default;
    }

    /*
    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
    */
}