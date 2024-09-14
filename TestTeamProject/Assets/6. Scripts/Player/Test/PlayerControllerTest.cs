using Mirror;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransformReliable))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerTest : NetworkBehaviour
{
    [Header("Avatar Components")]
    public CharacterController characterController;
    public NetworkAnimator networkAnimator;
    public Animator animator;

    [Header("Movement")]
    [Range(1, 20)]
    public float moveSpeedMultiplier;
    [SerializeField]
    float walkSpeed = 2f;
    [SerializeField]
    float runSpeed = 4f; 

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

    protected override void OnValidate()
    {
        base.OnValidate();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        characterController.enabled = false;
        characterController.skinWidth = 0.02f;
        characterController.minMoveDistance = 0f;

        GetComponent<Rigidbody>().isKinematic = true;

        this.enabled = false;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        characterController.enabled = true;
        this.enabled = true;
    }

    public override void OnStopAuthority()
    {
        base.OnStopAuthority();
        this.enabled = false;
        characterController.enabled = false;
    }

    void Start()
    {
        // 로컬 플레이어인지 확인
        if (isLocalPlayer)
        {
            gameObject.GetComponent<PlayerControllerTest>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<PlayerControllerTest>().enabled = false;
        }

        if (networkAnimator == null)
            networkAnimator = GetComponent<NetworkAnimator>();
    }

    void Update()
    {
        // 로컬 플레이어 인지 확인
        if (!isLocalPlayer)
            return;
        // CharacterController가 활성화 되어있지 않으면 리턴
        if (!characterController.enabled)
            return;
        
        // 일시정지 상태가 아니면 이동, 점프, 애니메이션 업데이트
        if (!GameUIController.IsPaused)
        {
            HandleMove();
            HandleJumping();
            AnimationUpdate();
        }

        // 중력 적용
        jumpSpeed += Physics.gravity.y * Time.deltaTime;

        // 방향에 점프 속도를 추가
        direction.y = Mathf.Clamp(jumpSpeed, -10f, 10f);
        
        // 캐릭터를 이동
        characterController.Move(direction * Time.deltaTime);

        // 진단을 위한 속도... FloorToInt를 사용하여 표시 목적
        velocity = Vector3Int.FloorToInt(characterController.velocity);
    }

    void HandleJumping()
    {
        if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpSpeed = jumpForce;
            networkAnimator.animator.SetTrigger("jump");
        }
    }

    void HandleMove()
    {
        // 왼쪽 Shift 키가 눌리면 달리기 속도를 사용하고, 그렇지 않으면 걷기 속도를 사용
        moveSpeedMultiplier = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

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

    void AnimationUpdate()
    {
        // 애니메이터 설정: 달리기 여부
        networkAnimator.animator.SetBool("isRun", Input.GetKey(KeyCode.LeftShift));
        // 애니메이터 설정: 수평 속도
        networkAnimator.animator.SetFloat("speedX", Input.GetAxis("Horizontal"));
        // 애니메이터 설정: 수직 속도
        networkAnimator.animator.SetFloat("speedY", Input.GetAxis("Vertical"));
    }
}