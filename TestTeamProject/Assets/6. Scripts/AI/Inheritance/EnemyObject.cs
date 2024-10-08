using System.Collections;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrolling, // 배회 중
    Detecting,  // 감지 중
    Chasing,    // 추적 중
    Attacking   // 공격 중
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
public class EnemyObject : NetworkBehaviour
{
    public EnemyState currentState = EnemyState.Patrolling;

    public float stateInterval; // 상태 전환 주기
    public float detectionInterval; // 감지 주기

    public float patrolSpeed; // 배회 속도
    public float chaseSpeed; // 추적 속도

    public float patrolRange; // 배회 범위
    public float patrolWaitTime; // 배회 대기 시간

    [Range(0, 360)] public float attackAngle; // 공격각(0 ~ 360도)
    public float attackRange; // 공격 범위
    public int attackDamage; // 공격 데미지
    public float attackCooldown; // 공격 쿨타임
    protected float lastAttackTime; // 마지막 공격 시간

    [Range(0, 360)] public float viewAngle; // 시야각(0 ~ 360도)
    public float detectionRange; // 감지 범위
    public float soundDetectionRange; // 소리 감지 범위
    public float timeToChaseLostTarget; // 추적 범위에서 벗어나 배회로 돌아가는 시간
    protected float timeSinceTargetLost; // 플레이어가 감지 범위에서 벗어난 시간
    
    protected Transform targetTransform; // 추적할 타겟의 Transform
    protected Vector3 patrolTarget; // 배회할 목표 지점

    public LayerMask playerMask; // 플레이어 레이어
    public LayerMask soundMask; // 소리 레이어
    public LayerMask obstacleMask; // 장애물 레이어

    public AudioClip patrolSound; // 배회 사운드
    public AudioClip chaseSound; // 추적 사운드
    public AudioClip meleeAttackSound; // 근접 공격 사운드
    public AudioClip rangedAttackSound; // 원거리 공격 사운드

    protected NavMeshAgent agent; // NavMeshAgent 컴포넌트
    public Animator animator; // 애니메이터 컴포넌트
    public AudioClip footstep; // 발소리 클립

    protected override void OnValidate()
    {
        base.OnValidate();

        playerMask = LayerMask.GetMask("Player");
        soundMask = LayerMask.GetMask("Sound");
        obstacleMask = LayerMask.GetMask("Obstacle");

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public virtual void Start()
    {
        agent.speed = patrolSpeed; // 초기 배회 속도 설정
        lastAttackTime = -attackCooldown; // 초기화 시 즉시 공격 가능하도록 설정
        patrolTarget = transform.position; // 초기 배회 위치 설정
        
        if (isServer)
        {
            StartCoroutine(nameof(StartAI), stateInterval);
            StartCoroutine(nameof(StartDetection), detectionInterval);
        }
    }

    // AI 시작
    public virtual IEnumerator StartAI(float interval)
    {
        yield return new WaitForSeconds(interval);

        while (true)
        {
            UpdateState();

            yield return null;
        }
    }

    // 상태 업데이트
    public virtual void UpdateState()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
        }
        AnimationUpdate();
    }

    // 감지 시작
    public virtual IEnumerator StartDetection(float interval)
    {
        while (true)
        {
            Detect();

            yield return new WaitForSeconds(interval);
        }
    }

    // 배회 로직
    public virtual void Patrol()
    {
        agent.speed = patrolSpeed;

        // 배회할 위치로 이동
        if (Vector3.Distance(transform.position, patrolTarget) > patrolRange
         || Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            StartCoroutine(SetRandomPatrolTarget());
        }
        else
        {
            agent.SetDestination(patrolTarget);
        }
    }

    // 배회할 목표 지점 설정
    protected IEnumerator SetRandomPatrolTarget()
    {
        currentState = EnemyState.Patrolling;
        yield return new WaitForSeconds(patrolWaitTime);

        Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, patrolRange, NavMesh.AllAreas))
        {
            patrolTarget = navHit.position;
        }
    }

    // 감지 로직
    public virtual void Detect()
    {
        // 플레이어 감지 및 소리 감지 (순서 중요)
        DetectSound();
        DetectPlayer();
    }

    // 플레이어 감지 로직
    public virtual void DetectPlayer()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, detectionRange, playerMask)
            .OrderBy(target => Vector3.Distance(transform.position, target.transform.position)).ToArray();

        foreach (Collider target in targets)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;

            // AI의 정면 방향과 타겟 사이의 각도 계산
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                // 타겟까지 Ray를 쏴서 장애물에 막히지 않았는지 확인
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    OnPlayerDetected(target); // 플레이어 감지 이벤트 호출

                    // 첫 번째 타겟만 추적하기 위해 반복문 종료
                    break;
                }
            }
        }
    }

    // 소리 감지 로직
    public virtual void DetectSound()
    {
        Collider[] sounds = Physics.OverlapSphere(transform.position, soundDetectionRange, soundMask)
            .OrderBy(col => Vector3.Distance(transform.position, col.transform.position)).ToArray();

        foreach (Collider sound in sounds)
        {
            Transform soundTransform = sound.transform;
            Vector3 dirToTarget = (soundTransform.position - transform.position).normalized;

            // 타겟까지의 거리 계산
            float distanceToTarget = Vector3.Distance(transform.position, soundTransform.position);

            // 타겟까지 Ray를 쏴서 장애물에 막히지 않았는지 확인
            if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
            {
                OnSoundDetected(sound); // 소리 감지 이벤트 호출

                // 첫 번째 타겟만 추적하기 위해 반복문 종료
                break;
            }
        }
    }

    // 플레이어 감지 이벤트
    public virtual void OnPlayerDetected(Collider target)
    {
        currentState = EnemyState.Chasing;
        targetTransform = target.transform;
    }

    // 소리 감지 이벤트
    public virtual void OnSoundDetected(Collider target)
    {
        currentState = EnemyState.Chasing;
        targetTransform = target.transform;
    }

    // 추적 로직
    public virtual void Chase()
    {
        if (targetTransform == null)
        {
            currentState = EnemyState.Patrolling;
            return;
        }

        agent.speed = chaseSpeed;
        agent.SetDestination(targetTransform.position);

        // 추적 범위 벗어남을 확인하고 배회로 전환
        if (Vector3.Distance(transform.position, targetTransform.position) > detectionRange)
        {
            timeSinceTargetLost += Time.deltaTime;

            if (timeSinceTargetLost >= timeToChaseLostTarget)
            {
                currentState = EnemyState.Patrolling;
                timeSinceTargetLost = 0f;
            }
        }
        else
        {
            timeSinceTargetLost = 0f;
        }

        if (Vector3.Distance(transform.position, targetTransform.position) <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(nameof(Attack));
        }
    }

    // 공격 로직
    public virtual IEnumerator Attack()
    {
        lastAttackTime = Time.time; // 공격 후 쿨타임 초기화

        agent.isStopped = true; // 이동 멈춤

        // 현재 애니메이션 클립의 길이 가져오기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;  // 현재 애니메이션 길이
        
        // 애니메이션의 길이만큼 대기
        yield return new WaitForSeconds(animationLength);

        agent.isStopped = false; // 이동 재개
        
    }

    // 근접 공격 로직
    public virtual void MeleeAttack()
    {
        // 1. 적의 정면 방향으로 범위 내 타겟을 탐지
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, playerMask);
        
        foreach (Collider target in targets)
        {
            // 타겟이 시야각 내에 있는지 확인
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) <= attackAngle / 2)
            {
                // 2. 타겟이 정면 각도 내에 있으면 공격
                target.GetComponent<PlayerDataController>().CmdChangeHp(-attackDamage); // 타겟의 체력 감소
            }
        }
    }

    // 원거리 공격 로직
    public virtual void RangedAttack() {}

    public virtual void PlayPatrolSound()
    {
        AudioSource.PlayClipAtPoint(patrolSound, transform.position);
    }

    public virtual void PlayChaseSound()
    {
        AudioSource.PlayClipAtPoint(chaseSound, transform.position);
    }

    public virtual void PlayMeleeAttackSound()
    {
        AudioSource.PlayClipAtPoint(meleeAttackSound, transform.position);
    }

    public virtual void PlayRangedAttackSound()
    {
        AudioSource.PlayClipAtPoint(rangedAttackSound, transform.position);
    }
    //애니메이션 업데이트
    public virtual void AnimationUpdate()
    {
        float speed = agent.velocity.magnitude;
        if (speed > 0)
        {
            animator.SetBool("isMove", true);
        }
        else
        {
            animator.SetBool("isMove", false);
        }
        if (currentState == EnemyState.Chasing)
        {
            animator.SetBool("isChase", true);
        }
        else
            animator.SetBool("isChase", false);
    }
    //발소리
    public virtual void FootStep()
    {
        AudioSource.PlayClipAtPoint(footstep, transform.position);
        Debug.Log("footstep sound play");
    }
}
