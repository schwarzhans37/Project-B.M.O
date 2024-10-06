using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public class MinotaurAI : EnemyObject
{

    public float dashDistance = 10f; // 대쉬 거리
    public float dashSpeed = 10f; // 대쉬 속도
    public float dashCooldown = 10f; // 대쉬 쿨타임
    private float lastDashTime; // 마지막 대쉬 시간
    private bool isDashing = false; // 대쉬 중인지 여부

    protected override void OnValidate()
    {
        base.OnValidate();

        stateInterval = 0.1f; // 상태 전환 주기
        detectionInterval = 0.1f; // 감지 주기

        patrolSpeed = 2f; // 배회 속도
        chaseSpeed = 4f; // 추적 속도

        attackAngle = 90f; // 공격각(0 ~ 360도)
        attackRange = 2f; // 공격 범위
        attackDamage = 500; // 공격 데미지
        attackCooldown = 1f; // 공격 쿨타임

        viewAngle = 120f; // 시야각(0 ~ 360도)
        patrolRange = 15f ; // 배회 범위
        detectionRange = 15f; // 감지 범위
        soundDetectionRange = 20f; // 소리 감지 범위
        patrolWaitTime = 2f; // 배회 대기 시간
        detectionLossTime = 5f; // 추적 범위에서 벗어나 배회로 돌아가는 시간
    }

    public override void UpdateState()
    {
        if (isDashing)
            return;

        base.UpdateState();
    }

    public override void OnPlayerDetected(Collider target)
    {
        base.OnPlayerDetected(target);

        if (!isDashing && Time.time > lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
            lastDashTime = Time.time;
        }
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dash!");
        isDashing = true;
        agent.isStopped = true; // NavMeshAgent 일시 중지

        // 돌진 방향 계산
        Vector3 dashDirection = (targetTransform.position - transform.position).normalized;

        // 충돌 체크 및 이동 처리
        float distanceTravelled = 0f;

        while (distanceTravelled < dashDistance)
        {
            // 이동할 거리 계산
            float moveDistance = dashSpeed * Time.deltaTime;

            // 다음 위치 계산
            Vector3 nextPosition = transform.position + dashDirection * moveDistance;

            // 충돌 감지 (돌진 방향으로 레이캐스트)
            if (Physics.Raycast(transform.position, dashDirection, moveDistance, obstacleMask) || 
                Physics.Raycast(transform.position, dashDirection, moveDistance, playerMask))
            {
                Debug.Log("Dash hit obstacle or player.");
                break;
            }

            // 위치 업데이트
            transform.position = nextPosition;

            // 이동한 거리 업데이트
            distanceTravelled += moveDistance;

            yield return null; // 다음 프레임 대기
        }

        // 돌진 종료 후 NavMeshAgent 재활성화
        isDashing = false;
        agent.isStopped = false;
    }
}
