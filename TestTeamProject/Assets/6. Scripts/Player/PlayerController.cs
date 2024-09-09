using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Mirror.Examples.Basic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
//------------------------- 캐릭터 -------------------------
    //캐릭터 컨트롤러 컴포넌트
    private CharacterController cc;

    //캐릭터 걷기 이동속도            
    [SerializeField] float WalkSpeed = 2f;
    [SerializeField] float CrouchSpeed = 1.5f;
    [SerializeField] float RunSpeed = 4f; 
    [SerializeField] float JumpForce = 5f; 

    //캐릭터 애니메이션 컴포넌트
    public Animator animator;
    public bool isCrouch = false;
    public bool isLand = true;
    public bool isFall = false;
    public bool isJump = false;

    //------------------------- 마우스 -------------------------
    //마우스 상하좌우
    private float mouseX;       
    private float mouseY;

     //마우스 민감도
    [SerializeField] float mouseSpeed = 3f;   

//------------------------- 방향과 중력 -------------------------
    //중력
    private float gravity;  

    //방향    
    private Vector3 mov;       

//------------------------- 오브젝트 -------------------------
    //플레이어 모델 프리팹
    public GameObject PlayerModel;

    //플레이어 모델 프리팹 내부 카메라 오브젝트
    public GameObject PlayerCamera;

//------------------------- 스크립트 -------------------------
    // Start is called before the first frame update
    void Start()
    {
        //캐릭터 컨트롤러 컴포넌트 불러오기
        cc = GetComponent<CharacterController>();
        //캐릭터 애니메이터 컴포넌트 불러오기
        animator = GetComponent<Animator>();
        //벡터 초기화
        mov = Vector3.zero;
        //중력강도
        gravity = 9.8f;    
    }

    // Update is called once per frame
    void Update()
    {
        //Pause메뉴 호출 시 마우스 상태와 플레이어 조작가능 유무
        if(PauseMenu.GameIsPaused == true) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            MouseMove();
            Moving();
        }
        AnimationUpdate();
    }

    void MouseMove()
    {
        //마우스 상하좌우 반응
        mouseX += Input.GetAxis("Mouse X") * mouseSpeed;
        mouseY += Input.GetAxis("Mouse Y") * mouseSpeed;

        //마우스 위아래 최대각도 적용
        mouseY = Mathf.Clamp(mouseY, -80f, 80f);

        //마우스 상하좌우 각도값 변경
        PlayerModel.transform.localEulerAngles = new Vector3 (0, mouseX, 0);
        PlayerCamera.transform.localEulerAngles = new Vector3 (-mouseY, 0, 0);
    }

    void Moving()
    {
        float speed = 0f;

        //달리기 조작 여부
        if (Input.GetKey(KeyCode.LeftShift) && isCrouch == false && isLand == true)
        {
            speed = RunSpeed;
        }
        else
        {
            speed = WalkSpeed;
        }
        // 앉기 조작 여부
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouch = true;
            animator.SetBool("isCrouch", true);
            speed = CrouchSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouch = false;
            animator.SetBool("isCrouch", false);
        }
        //중력 및 키보드 조작 반응
        if (cc.isGrounded)
        {

            isLand = true;
            isFall = false;
            isJump = false;
            mov = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            mov = cc.transform.TransformDirection(mov);
            mov *= speed;

            //Space바 입력시 점프
            if (Input.GetKeyDown(KeyCode.Space))
            {
                mov.y += JumpForce;
                isJump = true;
                isLand = false;
            }
            //체공 시
            if (mov.y == 0 && isLand == false)
            {
                isFall = true;
            }
                
        }
        else
        {
            mov.y -= gravity * Time.deltaTime;
        }
        cc.Move(mov * Time.deltaTime);
        
    }
    void AnimationUpdate()
    {
        
        animator.SetBool("isRun", false);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("isRun", true);
        }
        animator.SetFloat("speedX", Input.GetAxis("Horizontal"));
        animator.SetFloat("speedY", Input.GetAxis("Vertical"));
        if (isLand == true)
        {
            animator.SetTrigger("isLand");
        }
        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetTrigger("Jump");   
        }
        if (isJump == true)
        {
            animator.SetBool("isJump", true);
        }
        if (isFall == true)
        {
            animator.SetBool("isFall", true);
        }
        //피격시
        /* if (피격 조건){
         * animator.SetTrigger("Hitted");
         * }
         */
        //사망시
        /* if (사망 조건){
         * animator.SetBool("isDie",true);
         * }
         */
    }
}
