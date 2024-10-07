using UnityEngine;
using UnityEngine.AI;

public class AIRockGolem : MonoBehaviour
{
    public float health = 500f; // 골렘의 체력
    public float moveSpeed = 2f; // 배회 시 이동 속도
    public float chaseSpeed = 4f; // 추적 시 이동 속도
    public float meleeAttackRange = 1f; // 근접 공격 범위
    public float meleeAttackDamage = 80f; // 근접 공격 데미지
    public float meleeRange = 3f; // 근접 공격을 선택할 거리
    public float meleeAttackCooldown = 1f; // 근접 공격 쿨타임
    public float rockThrowRange = 10f; // 바위 투척 사거리
    public float rockDirectHitDamage = 100f; // 바위 직접 명중 시 데미지
    public float rockAoEDamage = 40f; // 충격파 데미지
    public float rockAoERadius = 4f; // 바위 충격파 범위
    public GameObject rockProjectilePrefab; // 바위 투척에 사용할 프리팹
    public Transform rockThrowOrigin; // 바위 투척의 발사 지점

    private float rockThrowCooldown = 60f; // 바위 투척 쿨타임 1분
    private float lastRockThrowTime; // 마지막으로 바위를 던진 시간
    private float lastMeleeAttackTime; // 마지막 근접 공격 시간

    private Transform player; // 추적할 플레이어
    private NavMeshAgent agent; // NavMeshAgent 컴포넌트
    private enum GolemState { Patrolling, Chasing }
    private GolemState currentState = GolemState.Patrolling;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed; // 초기 배회 속도 설정
        FindNearestPlayer();
        lastRockThrowTime = -rockThrowCooldown; // 초기화 시 즉시 공격 가능하도록 설정
        lastMeleeAttackTime = -meleeAttackCooldown; // 근접 공격도 초기화
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
            return;
        }

        switch (currentState)
        {
            case GolemState.Patrolling:
                Patrol();
                break;
            case GolemState.Chasing:
                ChasePlayer();
                break;
        }
        AnimationUpdate();
    }

    void FindNearestPlayer()
    {
        // Player 태그를 가진 오브젝트 중 가장 가까운 플레이어를 찾음
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;

        foreach (GameObject p in players)
        {
            float distance = Vector3.Distance(transform.position, p.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                player = p.transform;
            }
        }
    }

    void Patrol()
    {
        // 임시 배회 행동 (랜덤 포인트로 이동하도록 설정)
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10f, 1);
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= rockThrowRange)
        {
            currentState = GolemState.Chasing;
            agent.speed = chaseSpeed;
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 공격 선택
        if (distanceToPlayer <= meleeRange && Time.time >= lastMeleeAttackTime + meleeAttackCooldown)
        {
            MeleeAttack();
        }
        else if (distanceToPlayer <= rockThrowRange && Time.time >= lastRockThrowTime + rockThrowCooldown)
        {
            // 쿨타임이 끝나면 바위 투척
            RockThrowAttack();
        }
        else
        {
            // 바위 투척 쿨타임 중이거나 거리가 멀면 근접 공격 수행
            if (Time.time >= lastMeleeAttackTime + meleeAttackCooldown)
            {
                MeleeAttack();
            }
        }

        // 추적 범위를 벗어나면 다시 배회 상태로
        if (distanceToPlayer > rockThrowRange)
        {
            currentState = GolemState.Patrolling;
            agent.speed = moveSpeed;
        }
    }

    void MeleeAttack()
    {
        // 플레이어와 거리가 1 이하일 때 공격
        if (Vector3.Distance(transform.position, player.position) <= meleeAttackRange)
        {
            // 근접 공격 로직 (한 번 공격하면 다시 추적 상태로)
            Debug.Log("Rock Golem performs a melee attack.");
            animator.SetTrigger("attack");
            // 근접 공격 후 타이머 초기화
            lastMeleeAttackTime = Time.time;
            // 근접 공격 후 다시 추적 상태로
            currentState = GolemState.Chasing;
        }
    }

    void RockThrowAttack()
    {
        // 제자리에서 바위 투척 공격
        Debug.Log("Rock Golem throws a rock.");
        animator.SetTrigger("rock");
        // 바위 투척 프리팹 인스턴스 생성
        GameObject rock = Instantiate(rockProjectilePrefab, rockThrowOrigin.position, Quaternion.identity);
        // 바위 투척 발사 방향 설정
        rock.GetComponent<RockProjectile>().Initialize(player.position, rockDirectHitDamage, rockAoEDamage, rockAoERadius);

        // 마지막 바위 투척 시간 기록
        lastRockThrowTime = Time.time;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Rock Golem has died.");
        animator.SetBool("isDead", true);
        // 사망 시 AI 정지
        agent.isStopped = true;
        // 3600초 후에 오브젝트 비활성화
        Invoke(nameof(DeactivateGolem), 3600f);
    }

    void DeactivateGolem()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌 시 근접 공격 데미지 처리
        if (other.CompareTag("Player") && Time.time >= lastMeleeAttackTime + meleeAttackCooldown)
        {
            Debug.Log("Rock Golem hits the player with melee attack.");
            // 플레이어에게 데미지 적용 (float을 int로 변환하여 전달)
            // other.GetComponent<PlayerHealth>().TakeDamage(Mathf.RoundToInt(meleeAttackDamage));
            // 근접 공격 후 타이머 초기화
            lastMeleeAttackTime = Time.time;
        }
    }
    void AnimationUpdate()
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
    }
}
