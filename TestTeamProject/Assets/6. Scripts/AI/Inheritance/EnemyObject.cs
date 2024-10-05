using System.Collections;
using System.Collections.Generic;
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

    public float patrolSpeed = 2f; // 배회 속도
    public float chaseSpeed = 4f; // 추적 속도

    [Range(0, 360)] public float attackAngle = 45f; // 공격각(0 ~ 360도)
    public float attackRange = 2f; // 공격 범위
    public int attackDamage = 20; // 공격 데미지
    public float attackCooldown = 1f; // 공격 쿨타임
    private float lastAttackTime; // 마지막 공격 시간

    [Range(0, 360)] public float viewAngle = 90f; // 시야각(0 ~ 360도)
    public float patrolRange = 10f; // 배회 범위
    public float detectionRange = 10f; // 감지 범위
    public float soundDetectionRange = 20f; // 소리 감지 범위
    public float detectionLossTime = 5f; // 추적 범위에서 벗어나 배회로 돌아가는 시간
    private float timeSincePlayerOutOfRange; // 플레이어가 감지 범위에서 벗어난 시간
    
    public LayerMask playerMask; // 플레이어 레이어
    public LayerMask obstacleMask; // 장애물 레이어

    private Transform targetTransform; // 플레이어의 Transform
    private NavMeshAgent agent; // NavMeshAgent 컴포넌트
    private Vector3 patrolTarget; // 배회할 목표 지점

    protected override void OnValidate()
    {
        base.OnValidate();

        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void Patrol()
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
        Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
        randomDirection += transform.position;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRange, NavMesh.AllAreas))
        {
            patrolTarget = navHit.position;
        }
    }

    public virtual void DetectPlayer()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, detectionRange, playerMask);

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
                    currentState = EnemyState.Chasing;
                    this.targetTransform = targetTransform;
                    break;
                }
            }
        }
    }

    public virtual void DetectSound()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, soundDetectionRange);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("SoundTarget"))
            {
                currentState = EnemyState.Chasing;
                targetTransform = col.transform;
                break;
            }
        }
    }

    public virtual void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(targetTransform.position);

        // 추적 범위 벗어남을 확인하고 배회로 전환
        if (Vector3.Distance(transform.position, targetTransform.position) > detectionRange)
        {
            timeSincePlayerOutOfRange += Time.deltaTime;

            if (timeSincePlayerOutOfRange >= detectionLossTime)
            {
                currentState = EnemyState.Patrolling;
                timeSincePlayerOutOfRange = 0f;
            }
        }
        else
        {
            timeSincePlayerOutOfRange = 0f;
        }

        if (Vector3.Distance(transform.position, targetTransform.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
    }

    public virtual void Attack()
    {
        if (Vector3.Distance(transform.position, targetTransform.position) <= attackRange)
        {
            if (Time.time < lastAttackTime + attackCooldown)
                return;
            
            // 1. 적의 정면 방향으로 범위 내 타겟을 탐지
            Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, playerMask);
            
            foreach (Collider target in targets)
            {
                // 타겟이 시야각 내에 있는지 확인
                Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) <= attackAngle / 2)
                {
                    // 2. 타겟이 정면 각도 내에 있으면 공격
                    target.GetComponent<PlayerDataController>().CmdChangeHp(attackDamage); // 타겟의 체력 감소
                }
            }
            lastAttackTime = Time.time;
        }
        else
        {
            currentState = EnemyState.Chasing;
        }
    }


    // Gizmo로 시야각을 시각적으로 표시 (디버깅용)
    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * detectionRange);
    }

    // 각도를 방향 벡터로 변환
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}
