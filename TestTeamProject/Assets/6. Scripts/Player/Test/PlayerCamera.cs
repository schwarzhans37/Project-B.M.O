using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCamera : NetworkBehaviour
{
    // 마우스 감도 설정
    [Header("Sensitivity")]
    [Range(1f, 200f)]
    public float mouseSensitivity = 30f;

    // 수직 회전을 제한하기 위한 변수
    private float xRotation = 0f;

    // 카메라 참조를 위한 변수
    public Camera playerCamera;

    void Start()
    {
        if (playerCamera == null)
            Debug.LogError("PlayerCamera: playerCamera not assigned!");

        // 로컬 플레이어인지 확인
        if (isLocalPlayer)
        {
            // 로컬 플레이어의 카메라는 활성화
            gameObject.GetComponent<PlayerCamera>().enabled = true;
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            // 로컬 플레이어가 아닌 경우 카메라 비활성화
            gameObject.GetComponent<PlayerCamera>().enabled = false;
            playerCamera.gameObject.SetActive(false);
        }

        // 카메라 설정
        playerCamera.orthographic = false;
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = new Vector3(0f, 1.65f, 0.2f);
        playerCamera.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
    }

    void Update()
    {
        // 로컬 플레이어 인지 확인
        if (!isLocalPlayer)
            return;
        // 커서가 잠겨있지 않으면 함수 종료
        if (GameUIController.IsPaused)
            return;

        // 마우스 입력 값 받아오기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 카메라의 수직 회전 값 변경
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f); // 수직 회전 각도 제한

        // 카메라 회전 적용
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 플레이어의 수평 회전 적용
        transform.Rotate(Vector3.up * mouseX);
    }
}