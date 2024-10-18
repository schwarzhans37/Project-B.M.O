using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerCamera : NetworkBehaviour
{
    // 마우스 감도 설정
    [Header("Sensitivity")]
    [Range(1f, 200f)]
    public float mouseSensitivity;
    private float xRotation = 0f; // 수직 회전을 제한하기 위한 변수

    public float crouchHeight; // 앉았을 때 카메라가 내려갈 높이
    public float standHeight;  // 서 있을 때 카메라의 기본 높이
    public float crouchSpeed;    // 카메라가 이동하는 속도

    private bool isCrouching = false; // 앉기 상태를 확인하는 변수
    private Vector3 cameraOriginalPosition; // 카메라의 원래 위치

    // 카메라 참조를 위한 변수
    public Camera playerCamera;

    protected override void OnValidate()
    {
        base.OnValidate();

        mouseSensitivity = 50f;
        crouchHeight = 1.25f;
        standHeight = 1.65f;
        crouchSpeed = 10f;
        playerCamera.gameObject.SetActive(false);
        this.enabled = false;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // 로컬 플레이어인 경우 카메라 활성화
        playerCamera.gameObject.SetActive(true);
        this.enabled = true;
    }

    void Start()
    {
        if (playerCamera == null)
            this.enabled = false;

        // 카메라 설정
        playerCamera.orthographic = false;
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = new Vector3(0, 1.65f, 0.2f);
        playerCamera.transform.localRotation = Quaternion.identity;

        cameraOriginalPosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        // 로컬 플레이어 인지 확인
        if (!isLocalPlayer
            || GetComponent<PlayerDataController>().isDead)
            return;
        // 커서가 잠겨있지 않으면 함수 종료
        if (GameUIController.IsPaused)
            return;

        // 마우스 입력 값 받아오기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 카메라의 수직 회전 값 변경
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 60f); // 수직 회전 각도 제한

        // 카메라 회전 적용
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 플레이어의 수평 회전 적용
        transform.Rotate(Vector3.up * mouseX);

        // 앉기 상태일 때 카메라 높이 조절
        isCrouching = (Input.GetKey(KeyCode.LeftControl) && GetComponent<CharacterController>().isGrounded) 
            || (GetComponent<PlayerMovementController>().jumpSpeed < 0 && !GetComponent<CharacterController>().isGrounded) ;

        float targetHeight = isCrouching ? crouchHeight : standHeight;

        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition,
            new Vector3(cameraOriginalPosition.x, targetHeight, cameraOriginalPosition.z), Time.deltaTime * crouchSpeed);
    }
}