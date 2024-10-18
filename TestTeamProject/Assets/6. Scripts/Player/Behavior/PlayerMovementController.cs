using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : NetworkBehaviour
{
    public CharacterController characterController;
    public NetworkAnimator networkAnimator;

    [Header("Movement")]
    [Range(1f, 10f)]
    public float walkSpeed; // 걷기 속도
    [Range(1f, 10f)]
    public float crouchSpeed; // 앉기 속도
    [Range(1f, 10f)]
    public float runSpeedMultiplier; // 뛰기 속도 배율
    private float moveSpeedMultiplier; // 이동 속도 배율
    private float horizontal;
    private float vertical;

    [Header("Jumping")]
    [Range(-10f, 10f)]
    public float jumpForce; // 점프 힘
    public float jumpSpeed;

    //애니메이터 플래그
    public bool isTorch = false;

    //사운드 테스트
    public AudioClip footstep;
    public AudioClip landing;
    public AudioClip jumping;
    public AudioClip disarmTorch;
    // Torch 테스트
    public GameObject Torch;

    Vector3 direction;
    Vector3Int velocity;

    public GameObject soundEmitterPrefab;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (networkAnimator == null)
            networkAnimator = GetComponent<NetworkAnimator>();

        walkSpeed = 2.5f;
        crouchSpeed = 1f;
        runSpeedMultiplier = 2f;
        jumpForce = 6f;

        characterController.skinWidth = 0.02f;
        characterController.minMoveDistance = 0f;

        GetComponent<Collider>().enabled = true;
        characterController.enabled = false;
        this.enabled = false;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // 로컬 플레이어인 경우 캐릭터 컨트롤러 활성화
        GetComponent<Collider>().enabled = false;
        characterController.enabled = true;
        this.enabled = true;
    }

    void Update()
    {
        // 로컬 플레이어 인지 확인
        if (!isLocalPlayer
            || GetComponent<PlayerDataController>().isDead)
            return;

        // 일시정지 상태가 아니면 이동, 점프, 업데이트
        if (!GameUIController.IsPaused)
        {
            HandleMove();
            HandleJumping();
            HandlerItem();
        }
        else
        {
            // 일시정지 상태이면 속도를 0으로 설정
            direction = Vector3.zero;
        }

        // 중력 적용
        jumpSpeed += Physics.gravity.y * Time.deltaTime;

        // 방향에 y축 속도를 추가
        direction.y = Mathf.Clamp(jumpSpeed, Physics.gravity.y, jumpForce);
        
        // 애니메이션 업데이트
        AnimationUpdate();

        // 캐릭터를 이동
        characterController.Move(direction * Time.deltaTime);

        // 진단을 위한 속도... FloorToInt를 사용하여 표시 목적
        velocity = Vector3Int.FloorToInt(characterController.velocity);
    }

    void HandleMove()
    {
        // 입력 캡처
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // 점프 속도(y축) 없이 초기 방향 벡터 생성
        direction = new Vector3(horizontal, 0f, vertical);

        // 대각선 방향으로 이동할 때 속도 이점을 막기 위해 클램프 처리
        direction = Vector3.ClampMagnitude(direction, 1f);

        // 방향을 로컬 공간에서 월드 공간으로 변환
        direction = transform.TransformDirection(direction);

        // 이동 속도 설정
        moveSpeedMultiplier = Input.GetKey(KeyCode.LeftControl) ? crouchSpeed : walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift)
            && gameObject.GetComponent<PlayerDataController>().stamina > 0
            && (horizontal != 0 || vertical != 0))
        {
            gameObject.GetComponent<PlayerDataController>().AdjustStaminaOverTime(-7);
            moveSpeedMultiplier *= runSpeedMultiplier;
        }
        else
        {
            gameObject.GetComponent<PlayerDataController>().AdjustStaminaOverTime(5);
        }

        // 원하는 지상 속도로 곱셈
        direction *= moveSpeedMultiplier;
    }

    void HandleJumping()
    { 
        // 점프기능
        if (Input.GetKeyDown(KeyCode.Space)
            && characterController.isGrounded)
        {
            jumpSpeed = jumpForce;
            networkAnimator.SetTrigger("Jump");
        }
        
    }

    void HandlerItem()
    {
        Torch = FindChildWithName(transform,"Torch").gameObject;

        if (Input.GetKeyDown(KeyCode.F))
        {
            isTorch = !isTorch;
            Torch.transform.GetChild(0).gameObject.SetActive(isTorch);          
        }
    }

    void AnimationUpdate()
    {
        if (!GameUIController.IsPaused)
        {
            networkAnimator.animator.SetBool("isRun", Input.GetKey(KeyCode.LeftShift));
            networkAnimator.animator.SetBool("isCrouch", Input.GetKey(KeyCode.LeftControl));
            networkAnimator.animator.SetFloat("speedX", Input.GetAxis("Horizontal"));
            networkAnimator.animator.SetFloat("speedY", Input.GetAxis("Vertical"));
        }
        else
        {
            networkAnimator.animator.SetBool("isRun", false);
            networkAnimator.animator.SetBool("isCrouch", false);
            networkAnimator.animator.SetFloat("speedX", 0);
            networkAnimator.animator.SetFloat("speedY", 0);
        }
        
        networkAnimator.animator.SetBool("isLand", characterController.isGrounded);
        networkAnimator.animator.SetBool("isJump", !characterController.isGrounded);
        networkAnimator.animator.SetBool("isFall", !characterController.isGrounded && jumpSpeed < 0);
        networkAnimator.animator.SetBool("isTorch", isTorch);
    }

    public void FootStep()
    {
        AudioSource.PlayClipAtPoint(footstep, transform.position);
        CreateSoundEmitter(footstep);
    }

    public void SEJump()
    {
        Debug.Log("Jump sound played");
        AudioSource.PlayClipAtPoint(jumping, transform.position);
        CreateSoundEmitter(jumping);
    }

    public void SETorch()
    {
        Debug.Log("Torch equipped sound played");
    }

    // SoundEmitter 객체 생성 함수
    void CreateSoundEmitter(AudioClip audioClip)
    {
        // SoundEmitter 프리팹을 현재 위치에 생성
        GameObject soundEmitter = Instantiate(soundEmitterPrefab, transform.position, Quaternion.identity);

        // SoundEmitter의 설정 (감지 범위 및 지속 시간)
        SoundEmitter emitter = soundEmitter.GetComponent<SoundEmitter>();
        emitter.duration = audioClip.length * 3;
    }
    Transform FindChildWithName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child; // 찾은 자식 반환
            }

            // 자식의 자식에서 재귀적으로 탐색
            Transform found = FindChildWithName(child, name);
            if (found != null)
            {
                return found; // 재귀 호출에서 찾은 경우 반환
            }
        }
        return null; // 찾지 못했을 경우 null 반환
    }
    public void LightOffTorch()
    {
        AudioSource.PlayClipAtPoint(disarmTorch, transform.position);
    }
}
