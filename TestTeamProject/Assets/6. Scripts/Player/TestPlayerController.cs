using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TestPlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public Animator animator;

    // 카메라 참조를 위한 변수
    public Camera playerCamera;

    // 마우스 감도 설정
    [Header("Sensitivity")]
    [Range(1f, 200f)]
    public float mouseSensitivity = 30f;

    // 수직 회전을 제한하기 위한 변수
    private float xRotation = 0f;

    [Header("Movement")]
    [Range(1, 20)]
    public float moveSpeedMultiplier; // 이동 속도 배율
    [SerializeField]
    float walkSpeed = 2f; // 걷기 속도

    //애니메이터 플래그
    public bool isTorch = false;

    //사운드 테스트
    public AudioClip footstep;
    public AudioClip equiptorch;
    public AudioClip landing;
    public AudioClip jumping;
    public AudioClip burning;
    private float torchSoundTimer = 0f;
    [SerializeField] private float torchSoundInterval = 4f;

    [Header("Jumping")]
    [Range(-10f, 10f)]
    public float jumpForce = 5f;

    [SerializeField, Range(-1f, 1f)]
    float horizontal;
    [SerializeField, Range(-1f, 1f)]
    float vertical;

    [SerializeField, Range(0f, 10f)]
    float jumpSpeed;

    [SerializeField, Range(-1.5f, 1.5f)]
    float animVelocity;

    [SerializeField, Range(-1.5f, 1.5f)]
    float animRotation;

    [SerializeField] Vector3Int velocity;
    [SerializeField] Vector3 direction;

    void OnValidate()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        characterController.skinWidth = 0.02f;
        characterController.minMoveDistance = 0f;
    }

    void Start()
    {
        // 카메라 설정
        playerCamera.orthographic = false;
        playerCamera.transform.SetParent(transform);
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        HandleCamera();
        HandleMove();
        HandleJumping();
        HandlerItem();

        // 중력 적용
        jumpSpeed += Physics.gravity.y * Time.deltaTime;

        // 방향에 y축 속도를 추가
        direction.y = Mathf.Clamp(jumpSpeed, -10f, 10f);
        
        // 애니메이션 업데이트
        AnimationUpdate();
        
        // 사운드 업데이트
        HandlerSound();

        // 캐릭터를 이동
        characterController.Move(direction * Time.deltaTime);

        // 진단을 위한 속도... FloorToInt를 사용하여 표시 목적
        velocity = Vector3Int.FloorToInt(characterController.velocity);
    }

    void HandleCamera()
    {
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

    void HandleMove()
    {
        // 왼쪽 Shift 키가 눌리면 달리기 속도를 사용하고, 그렇지 않으면 걷기 속도를 사용]
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeedMultiplier = walkSpeed * 2;
        }
        else
        {
            moveSpeedMultiplier = walkSpeed;
        }

        moveSpeedMultiplier = Input.GetKey(KeyCode.LeftControl) ? moveSpeedMultiplier / 2 : moveSpeedMultiplier;
    
        // 입력 캡처
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // 점프 속도(y축) 없이 초기 방향 벡터 생성
        direction = new Vector3(horizontal, 0f, vertical);

        // 대각선 방향으로 이동할 때 속도 이점을 막기 위해 클램프 처리
        direction = Vector3.ClampMagnitude(direction, 1f);

        // 방향을 로컬 공간에서 월드 공간으로 변환
        direction = transform.TransformDirection(direction);

        // 원하는 지상 속도로 곱셈
        direction *= moveSpeedMultiplier;
    }

    void HandleJumping()
    {
        // 점프기능
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            jumpSpeed = jumpForce;
            animator.SetTrigger("Jump");
        }
        
    }

    void HandlerItem()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isTorch = !isTorch;
        }
    }

    void AnimationUpdate()
    {

        animator.SetBool("isRun", Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("isCrouch", Input.GetKey(KeyCode.LeftControl));
        animator.SetFloat("speedX", Input.GetAxis("Horizontal"));
        animator.SetFloat("speedY", Input.GetAxis("Vertical"));

        animator.SetBool("isLand", characterController.isGrounded);
        animator.SetBool("isJump", !characterController.isGrounded);
        animator.SetBool("isFall", !characterController.isGrounded && jumpSpeed < 0);
        animator.SetBool("isTorch", isTorch);
    }

    void HandlerSound()
    {
        if (isTorch)
        {
            torchSoundTimer += Time.deltaTime;
            if(torchSoundTimer >= torchSoundInterval)
            {
                torchSoundTimer = 0f;
                Debug.Log("Torch burning sound played");
                AudioSource.PlayClipAtPoint(burning, transform.position);
            }
            
        }
    }

    public void FootStep()
    {
        AudioSource.PlayClipAtPoint(footstep, transform.position);
    }

    public void SETorch()
    {
        Debug.Log("Torch equipped sound played");
        AudioSource.PlayClipAtPoint(equiptorch, transform.position);
    }

    public void SEJump()
    {
        Debug.Log("Jump sound played");
        AudioSource.PlayClipAtPoint(jumping, transform.position);
    }
}
