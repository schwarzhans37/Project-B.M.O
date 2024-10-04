using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AIDemonDog : MonoBehaviour
{
    public enum DogState { Patrolling, Chasing, Attacking }
    public DogState currentState = DogState.Patrolling;

    public float detectionRange = 10f; // 소리 감지 범위
    public float patrolSpeed = 2f; // 배회 속도
    public float chaseSpeed = 4f; // 추적 속도
    public float attackRange = 1.5f; // 공격 범위
    public int health = 100; // 체력
    public int attackDamage = 20; // 공격 데미지
    public float attackCooldown = 1f; // 공격 쿨타임
    public float detectionLossTime = 5f; // 추적 범위에서 벗어나 배회로 돌아가는 시간

    private Transform player; // 플레이어의 Transform
    private NavMeshAgent agent; // NavMeshAgent 컴포넌트
    private Vector3 patrolTarget; // 배회할 목표 지점
    private float timeSincePlayerOutOfRange; // 플레이어가 감지 범위에서 벗어난 시간
    private float lastAttackTime; // 마지막 공격 시간

    
    private Animator animator;
    private Animator animatorlod;
    void Start()
    {
        // 에이전트 및 플레이어 참조 설정
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject creepMesh = transform.Find("Creep_mesh")?.gameObject;
        GameObject creepMesh_lod = transform.Find("Creep_mesh_lod1")?.gameObject;

        if (creepMesh != null)
        {
            animator = creepMesh.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Creep_mesh not found.");
        }

        if (creepMesh_lod != null)
        {
            animatorlod = creepMesh_lod.GetComponent<Animator>();
            if (animatorlod == null)
            {
                Debug.LogError("Animator component not found in Creep_mesh_lod1.");
            }
        }
        else
        {
            Debug.LogError("Creep_mesh_lod1 not found.");
        }
        if (animator == null)
        {
            Debug.LogError("Animator component not found in Creep_Mesh.");
        }
        // 초기 배회 타겟 설정
        SetRandomPatrolTarget();
    }

    void Update()
    {
        // 소리 감지 확인
        CheckForSoundDetection();

        // 현재 상태에 따른 AI 동작
        switch (currentState)
        {
            case DogState.Patrolling:
                Patrol();
                break;
            case DogState.Chasing:
                ChasePlayer();
                break;
            case DogState.Attacking:
                // 공격 중에는 플레이어 추적을 멈춤
                agent.isStopped = true;
                break;
        }
        AnimationUpdate();
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
        float patrolRadius = 15f; // 배회 반경
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
                currentState = DogState.Patrolling;
                agent.isStopped = false; // 배회 상태에서는 다시 움직임
            }
        }
        else
        {
            timeSincePlayerOutOfRange = 0f; // 감지 범위 안에 있으면 시간 초기화
        }

        // 공격 범위에 들어오면 공격 상태로 전환
        if (Vector3.Distance(transform.position, player.position) <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            currentState = DogState.Attacking;
            animator.SetTrigger("Attack");
            animatorlod.SetTrigger("Attack");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 공격 상태이며, 충돌한 오브젝트가 플레이어일 때 데미지 입히기
        if (currentState == DogState.Attacking && other.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Demon Dog attacks player!");

            // 플레이어에게 데미지 입히기
            other.GetComponent<PlayerHealth>().TakeDamage(attackDamage);

            // 마지막 공격 시간 업데이트
            lastAttackTime = Time.time;

            // 공격 후 추적 상태로 복귀
            currentState = DogState.Chasing;
            agent.isStopped = false;
        }
    }

    void CheckForSoundDetection()
    {
        // "DogHearSound" 태그를 가진 오브젝트를 찾기
        GameObject[] soundSources = GameObject.FindGameObjectsWithTag("DogHearSound");

        // 모든 소리 소스들에 대해 감지 범위 확인
        foreach (GameObject soundSource in soundSources)
        {
            float distanceToSound = Vector3.Distance(transform.position, soundSource.transform.position);

            // 감지 범위 안에 들어오는 소리가 있다면 추적 시작
            if (distanceToSound <= detectionRange)
            {
                currentState = DogState.Chasing;
                return; // 소리가 하나라도 감지되면 추적 시작, 나머지 소리 확인 필요 없음
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // 체력이 0 이하이면 사망 처리
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Demon Dog has died!");
        animator.SetBool("isDead", true);
        animatorlod.SetBool("isDead", true);
        // AI 동작 정지
        agent.isStopped = true;
        enabled = false; // 스크립트 동작 정지

        // 3600초 후 오브젝트 비활성화
        StartCoroutine(DisableAfterTime(3600f));
    }

    IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
    void AnimationUpdate()
    {
        float speed = agent.velocity.magnitude;
        if (speed > 0)
        {
            animator.SetBool("isMove", true);
            animatorlod.SetBool("isMove", true);
        }
        else
        {
            animator.SetBool("isMove", false);
            animatorlod.SetBool("isMove", false);
        }
    }
}
