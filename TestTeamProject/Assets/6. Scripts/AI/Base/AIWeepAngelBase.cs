using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AIWeepAngelBase : MonoBehaviour
{
    public enum AngelState { Patrolling, Chasing, Attacking }
    public AngelState currentState = AngelState.Patrolling;

    public float detectionRange = 15f; // 플레이어를 감지할 수 있는 범위
    public float patrolSpeed = 1.5f; // 배회 속도
    public float chaseSpeed = 3f; // 추적 속도
    public float attackRange = 1f; // 공격 범위
    public int health = 100; // 우는 천사의 체력
    public int attackDamage = 9999; // 즉사 데미지
    public float sightAngle = 30f; // 플레이어를 감지하는 각도
    public float detectionLossTime = 5f; // 추적 범위에서 벗어나 배회로 돌아가는 시간

    private Transform player; // 플레이어의 Transform
    private Transform playerCam; // 플레이어의 카메라 Transform
    private NavMeshAgent agent; // NavMeshAgent 컴포넌트
    private Vector3 patrolTarget; // 배회할 목표 지점
    private bool isPlayerLooking = false; // 플레이어가 우는 천사를 바라보는지 여부
    private float timeSincePlayerOutOfRange; // 플레이어가 감지 범위에서 벗어난 시간
    
    void Start()
    {
        // 에이전트 및 플레이어 참조 설정
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 플레이어의 카메라 찾기
        GameObject playerCamObject = GameObject.FindGameObjectWithTag("PlayerCam");
        if (playerCamObject != null)
        {
            playerCam = playerCamObject.transform;
        }
        else
        {
            Debug.LogError("PlayerCam with the tag 'PlayerCam' not found!");
        }
        

        // 초기 배회 타겟 설정
        SetRandomPatrolTarget();
    }

    void Update()
    {
        // 플레이어 감지 및 시야 확인
        CheckForPlayerDetection();
        CheckIfPlayerIsLooking();

        // 플레이어가 우는 천사를 바라보고 있으면 멈춤
        if (isPlayerLooking)
        {
            agent.isStopped = true; // 멈춤
            return;
        }
        else
        {
            agent.isStopped = false; // 움직임 재개
        }

        // 현재 상태에 따른 AI 동작
        switch (currentState)
        {
            case AngelState.Patrolling:
                Patrol();
                break;
            case AngelState.Chasing:
                ChasePlayer();
                break;
            case AngelState.Attacking:
                AttackPlayer();
                break;
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        // 배회할 위치로 이동
        if (Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            SetRandomPatrolTarget();
        }
        else
        {
            agent.SetDestination(patrolTarget);
        }
    }

    void SetRandomPatrolTarget()
    {
        float patrolRadius = 10f; // 배회 반경
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = navHit.position;
        }
    }

    void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        // 추적 범위 벗어남을 확인하고 배회로 전환
        if (Vector3.Distance(transform.position, player.position) > detectionRange)
        {
            timeSincePlayerOutOfRange += Time.deltaTime;

            // 추적 범위에서 5초 이상 벗어나면 배회 상태로 전환
            if (timeSincePlayerOutOfRange >= detectionLossTime)
            {
                currentState = AngelState.Patrolling;
            }
        }
        else
        {
            timeSincePlayerOutOfRange = 0f; // 감지 범위 안에 있으면 시간 초기화
        }

        // 공격 범위에 들어오면 공격 상태로 전환
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = AngelState.Attacking;
        }
    }

    void AttackPlayer()
    {
        // 플레이어와의 접촉 시 즉사 공격
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Weeping Angel attacks player!");
            // player.GetComponent<PlayerHealth>().TakeDamage(attackDamage); // 플레이어에게 데미지 입히기
        }

        // 공격 후 추적 상태로 복귀
        currentState = AngelState.Chasing;
    }

    void CheckIfPlayerIsLooking()
    {
        if (playerCam == null) return;

        // 현재 카메라의 위치와 우는 천사 사이의 방향을 계산
        Vector3 directionToAngel = (transform.position - playerCam.position).normalized;
        float angleToAngel = Vector3.Angle(playerCam.forward, directionToAngel);

        // 시야 각도를 고려하여 우는 천사를 바라보는지 확인 (장애물 인식 없이)
        isPlayerLooking = angleToAngel < sightAngle;
    }

    void CheckForPlayerDetection()
    {
        // 우는 천사와 플레이어 사이의 거리 확인
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 감지 범위 안에 있을 때 추적 시작
        if (distanceToPlayer <= detectionRange && currentState == AngelState.Patrolling)
        {
            currentState = AngelState.Chasing;
        }
    }
}
