using System;
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
    public Animator Animator;

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

    public int heal;
    public float healCooldown;
    private float lastHealTime = 0f;


    //애니메이터 플래그
    public bool isTorch = false;

    //사운드 테스트
    public AudioClip footstep;
    public AudioClip forestFootstep;
    public AudioClip waterFootstep;
    public AudioClip jumping;
    public AudioClip forestJumping;
    public AudioClip waterJumping;
    public AudioClip equipTorch;
    public AudioClip disarmTorch;
    public GameObject Torch;
    private int soundEmitterCount;

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

        if (Animator == null)
            Animator = GetComponent<Animator>();
        
        networkAnimator.animator = Animator;

        walkSpeed = 2.5f;
        crouchSpeed = 1f;
        runSpeedMultiplier = 2f;
        jumpForce = 6f;

        heal = 100;
        healCooldown = 5f;

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
        if (!GameUIController.IsPaused
            && !GameDataController.isMoveLocked)
        {
            HandleMove();
            HandleBehavior();
        }
        else
            direction = Vector3.zero; // 일시정지 상태이면 속도를 0으로 설정
        

        // 힐링
        CheckHeal();

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
            gameObject.GetComponent<PlayerDataController>().AdjustStaminaOverTime(-10);
            moveSpeedMultiplier *= runSpeedMultiplier;
        }
        else
        {
            gameObject.GetComponent<PlayerDataController>().AdjustStaminaOverTime(5);
        }

        // 원하는 지상 속도로 곱셈
        direction *= moveSpeedMultiplier * GetComponent<InventoryController>().weightSlowdownFactor;
    }

    void HandleBehavior()
    { 
        // 점프기능
        if (Input.GetKeyDown(KeyCode.Space)
            && characterController.isGrounded)
        {
            jumpSpeed = jumpForce;
            networkAnimator.SetTrigger("Jump");
        }
        // 토치기능
        if (Input.GetKeyDown(KeyCode.F))
        {
            isTorch = !isTorch;
            CmdTorch(isTorch);
            GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetTorchState(isTorch);
        }
    }

    [Command]
    void CmdTorch(bool isTorch)
    {
        RpcTorch(isTorch);
    }

    [ClientRpc]
    void RpcTorch(bool isTorch)
    {
        Torch.transform.GetChild(0).gameObject.SetActive(isTorch);
    }

    [Command]
    void CheckHeal()
    {
        if (gameObject.GetComponent<PlayerDataController>().stamina >= 1000
            && Time.time - lastHealTime > healCooldown)
        {
            lastHealTime = Time.time;
            gameObject.GetComponent<PlayerDataController>().ChangeHp(heal);
        }
    }

    void AnimationUpdate()
    {
        if (!GameUIController.IsPaused)
        {
            networkAnimator.animator.SetBool("isRun", Input.GetKey(KeyCode.LeftShift)
            && gameObject.GetComponent<PlayerDataController>().stamina > 0);
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
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit);

        if (hit.collider == null)
            return;

        if (hit.collider.CompareTag("Forest"))
            AudioSource.PlayClipAtPoint(forestFootstep, transform.position, 0.3f);
        else if (hit.collider.CompareTag("Water"))
        {
            soundEmitterCount++;
            if (soundEmitterCount % 2 == 0)
                AudioSource.PlayClipAtPoint(waterFootstep, transform.position, 0.2f);
        }
        else
            AudioSource.PlayClipAtPoint(footstep, transform.position, 0.3f);

        if (!networkAnimator.animator.GetBool("isCrouch"))
            CreateSoundEmitter(footstep);
    }

    public void SEJump()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit);

        if (hit.collider == null)
            return;

        if (hit.collider.CompareTag("Forest"))
            AudioSource.PlayClipAtPoint(forestJumping, transform.position, 0.3f);
        else if (hit.collider.CompareTag("Water"))
            AudioSource.PlayClipAtPoint(waterJumping, transform.position, 0.2f);
        else
            AudioSource.PlayClipAtPoint(jumping, transform.position, 0.3f);

        CreateSoundEmitter(jumping);
    }

    public void SETorch()
    {
        AudioSource.PlayClipAtPoint(equipTorch, transform.position, 0.1f);
    }
    public void LightOffTorch()
    {
        AudioSource.PlayClipAtPoint(disarmTorch, transform.position, 0.1f);
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
}
