using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LoginController : MonoBehaviour
{
    public IEnumerator Login(string email, string password)
    {
        // LoginData 객체 생성 및 값 설정
        LoginData loginData = new LoginData();
        loginData.email = email;
        loginData.password = password;

        // JSON으로 직렬화
        string jsonData = JsonUtility.ToJson(loginData);

        // 요청 생성 및 설정
        UnityWebRequest request = RequestManager.Instance.CreateRequest("/login", "POST", jsonData);
        // request.certificateHandler = new BypassCertificate();

        // 요청 보내기
        yield return request.SendWebRequest();

        ApiResponse response = RequestManager.Instance.CreateResponseFromRequest(request);

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("success: " + response.message);

            // 응답에서 세션 쿠키를 갱신
            RequestManager.Instance.UpdateSessionCookieFromResponse(request);

            // 로그인 성공 시 메인 메뉴 씬으로 이동
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        }
        else
        {
            Debug.Log("error: " + response.message);
        }
    }

    /**
    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // 무조건 true 반환
            return true;
        }
    }
    **/
}

[System.Serializable]
public class LoginData
{
    public string email;
    public string password;
}