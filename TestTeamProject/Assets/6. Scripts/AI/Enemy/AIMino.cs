using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MinotaurAI : MonoBehaviour
{
    // MonsterState 열거형 정의
    public enum MonsterState
    {
        Patrolling, // 배회 중
        Detecting,  // 소리 감지 중
        Chasing,    // 플레이어 추적 중
        Attacking   // 공격 중
    }

    public MonsterState currentState = MonsterState.Patrolling;

    public float patrolSpeed = 2f; // 배회 속도
    public float chaseSpeed = 4f; // 추적 속도
    public float visualDetectionRange = 15f; // 시각적 감지 거리
    public float soundDetectionRange = 20f; // 소리 감지 거리
    public float attackRange = 2f; // 공격 범위
    public int health = 200; // 체력
    public int attackDamage = 50; // 공격력
    public float attackCooldown = 1f; // 공격 쿨타임
    public float waitTime = 2f; // 배회 중 대기 시간
    public float chargeDuration = 3f; // 돌진 지속 시간
    public float chargeSpeedMultiplier = 2.5f; // 돌진 시 속도 배수

    public float detectionLossTime = 5f; // 탐지 범위에서 멀어진 후 배회로 돌아가는 시간
    private float timeSincePlayerOutOfRange; // 플레이어가 탐지 범위에서 벗어난 시간

    private Transform player;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private Vector3 patrolTarget;
    private bool isCharging;
    private bool isAttacking; // 공격 중인지 확인
    private Vector3 lastHeardSoundPosition; // 마지막으로 소리가 들린 위치

    private MinoWeapon minoWeapon; // MinoWeapon 스크립트 참조

    Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // MinoWeapon 스크립트 참조 설정
        minoWeapon = GetComponentInChildren<MinoWeapon>();

        SetRandomPatrolTarget();
    }

    void Update()
    {
        // 공격 중일 때는 이동하지 않음
        if (isAttacking) return;

        switch (currentState)
        {
            case MonsterState.Patrolling:
                Patrol();
                break;
            case MonsterState.Detecting:
                MoveToSoundLocation();
                break;
            case MonsterState.Chasing:
                ChasePlayer();
                break;
            case MonsterState.Attacking:
                StartCoroutine(PerformAttack());
                break;
        }
        
        // 감지 체크 (시각 및 소리 감지)
        CheckForVisualDetection();
        CheckForSoundDetection();
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        // 목적지에 도달하면 대기 및 다음 목적지 설정
        if (Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            StartCoroutine(WaitAndSetNewPatrolTarget());
        }
        else
        {
            agent.SetDestination(patrolTarget);
        }
    }

    IEnumerator WaitAndSetNewPatrolTarget()
    {
        currentState = MonsterState.Patrolling;
        yield return new WaitForSeconds(waitTime);
        SetRandomPatrolTarget();
    }

    void SetRandomPatrolTarget()
    {
        // 현재 위치를 기준으로 일정 거리 안에서 랜덤 위치를 찾습니다
        float patrolRadius = 10f; // 배회 반경 (원하는 대로 설정 가능)
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        
        // 랜덤한 지점에 NavMesh 위의 위치를 찾습니다
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = navHit.position;
            agent.SetDestination(patrolTarget);
        }
    }


    void MoveToSoundLocation()
    {
        // 소리가 들렸던 위치로 이동
        agent.speed = patrolSpeed;
        agent.SetDestination(lastHeardSoundPosition);

        // 해당 위치에 도달하면 다시 배회 상태로 전환
        if (Vector3.Distance(transform.position, lastHeardSoundPosition) < 1f)
        {
            currentState = MonsterState.Patrolling;
        }
    }

    void ChasePlayer()
    {
        // 추적 단계에서 점진적으로 속도 상승
        if (!isCharging)
        {
            StartCoroutine(Charge());
        }
        
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        // 플레이어와의 거리를 확인하여 공격 상태로 전환
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = MonsterState.Attacking;
        }
        else
        {
            // 플레이어가 탐지 범위를 벗어났는지 확인
            if (Vector3.Distance(transform.position, player.position) > visualDetectionRange)
            {
                // 탐지 범위 밖에 있다면 시간 누적
                timeSincePlayerOutOfRange += Time.deltaTime;

                // 플레이어가 5초 이상 탐지 범위 밖에 있다면 배회 상태로 전환
                if (timeSincePlayerOutOfRange >= detectionLossTime)
                {
                    currentState = MonsterState.Patrolling;
                }
            }
            else
            {
                // 탐지 범위 내에 있으면 시간 초기화
                timeSincePlayerOutOfRange = 0f;
            }
        }
    }

    IEnumerator Charge()
    {
        isCharging = true;
        float originalSpeed = chaseSpeed;

        // 점진적으로 가속
        agent.speed = chaseSpeed * chargeSpeedMultiplier;
        yield return new WaitForSeconds(chargeDuration);

        // 돌진 지속 시간이 끝나면 원래 속도로 복귀
        agent.speed = originalSpeed;
        isCharging = false;
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        // 이동 멈추기
        agent.isStopped = true;

        // 공격 애니메이션 시작
        animator.SetTrigger("Attack");

        // 무기에 공격 상태 알림
        minoWeapon.StartAttack();

        // 잠시 대기 (공격하는 시간)
        yield return new WaitForSeconds(1f); // 공격 애니메이션의 길이에 따라 대기 시간 조정

        // 공격 종료
        minoWeapon.EndAttack();

        // 공격 후 이동 재개
        agent.isStopped = false;
        isAttacking = false;

        // 공격 후 상태 업데이트
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = MonsterState.Attacking; // 플레이어가 계속 공격 범위에 있다면 다시 공격
        }
        else if (Vector3.Distance(transform.position, player.position) <= visualDetectionRange)
        {
            currentState = MonsterState.Chasing; // 플레이어가 공격 범위 밖에 있지만 시야 내에 있을 경우 추적
        }
        else
        {
            currentState = MonsterState.Patrolling; // 시야에서 벗어난 경우 배회 상태로 전환
        }
    }

    void CheckForVisualDetection()
    {
        // 시각적 감지 체크
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 시각적 감지 범위 안에 있고, 시야 내에 플레이어가 있을 경우 추적 시작
        if (distanceToPlayer <= visualDetectionRange && CanSeePlayer())
        {
            currentState = MonsterState.Chasing;
            timeSincePlayerOutOfRange = 0f; // 추적 시작 시 시간 초기화
        }
    }

    bool CanSeePlayer()
    {
        // 시야 각도와 시야 거리 설정
        float viewAngle = 90f; // 미노타우로스의 시야 각도 (앞쪽 90도 설정 예시)
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        // 전방 방향과 플레이어 방향의 각도를 계산
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // 시야 각도 이내에 있을 때만 감지 (전방 각도 고려)
        if (angleToPlayer < viewAngle / 2)
        {
            RaycastHit hit;
            
            // Raycast로 장애물 확인
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, visualDetectionRange))
            {
                // 플레이어와 직접적으로 마주할 때만 감지
                return hit.transform.CompareTag("Player");
            }
        }
        
        return false;
    }


    void CheckForSoundDetection()
    {
        // 소리 감지 체크
        float distanceToSound = Vector3.Distance(transform.position, lastHeardSoundPosition);

        // 소리 감지 범위 안에 소리가 들리면 해당 위치로 이동 시작
        if (distanceToSound <= soundDetectionRange && currentState != MonsterState.Chasing)
        {
            currentState = MonsterState.Detecting;
        }
    }

    public void HearSound(Vector3 soundPosition)
    {
        // 플레이어로부터 소리를 들었을 때 호출
        lastHeardSoundPosition = soundPosition;

        // 현재 상태가 추적 상태가 아닐 경우 감지 상태로 전환
        if (currentState != MonsterState.Chasing)
        {
            currentState = MonsterState.Detecting;
        }
    }

    public void TakeDamage(int damage)
    {
        // 체력 감소 처리
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 몬스터 사망 로직 (예: 애니메이션 재생 및 비활성화)
        Debug.Log("Minotaur has died!");
        
        // AI 동작 정지
        agent.isStopped = true;
        enabled = false;
        
        // 죽는 애니메이션 및 3600초 후 비활성화 처리
        StartCoroutine(DisableAfterTime(3600f));
    }

    IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
