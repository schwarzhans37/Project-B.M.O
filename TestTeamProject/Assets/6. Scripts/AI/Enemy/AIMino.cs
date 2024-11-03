using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MinotaurAI : EnemyObject
{

    public float dashDistance; // 대쉬 거리
    public float dashSpeedMultiple; // 대쉬 속도 배수
    public int dashDamage; // 대쉬 데미지
    public float dashCooldown; // 대쉬 쿨타임
    private float lastDashTime; // 마지막 대쉬 시간
    private bool isDashing = false; // 대쉬 중인지 여부

    public AudioClip DashSound; // 대쉬 사운드

    protected override void OnValidate()
    {
        base.OnValidate();

        dashDistance = 30f; // 대쉬 거리
        dashSpeedMultiple = 5f; // 대쉬 속도 배수
        dashDamage = 900; // 대쉬 데미지
        dashCooldown = 20f; // 대쉬 쿨타임
    }

    public override void Setting()
    {
        base.Setting();

        lastDashTime = -dashCooldown;

        stateInterval = 0.1f; // 상태 전환 주기
        detectionInterval = 0.1f; // 감지 주기

        patrolSpeed = 2f; // 배회 속도
        chaseSpeed = 4f; // 추적 속도

        patrolRange = 15f ; // 배회 범위
        patrolWaitTime = 0f; // 배회 대기 시간

        attackAngle = 120f; // 공격각(0 ~ 360도)
        attackRange = 2.5f; // 공격 범위
        attackDamage = 490; // 공격 데미지
        attackCooldown = 3f; // 공격 쿨타임

        viewAngle = 120f; // 시야각(0 ~ 360도)
        detectionRange = 15f; // 감지 범위
        soundDetectionRange = 20f; // 소리 감지 범위
        timeToChaseLostTarget = 5f; // 추적 범위에서 벗어나 배회로 돌아가는 시간
    }

    public override void UpdateState()
    {
        if (isDashing)
            return;

        base.UpdateState();
    }

    public override void Detect()
    {
        if (isDashing)
            return;

        base.Detect();
    }

    public override void OnPlayerDetected(Collider target)
    {
        base.OnPlayerDetected(target);

        if (!isDashing
            && Time.time - lastDashTime > dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash()
    {
        Vector3 dashTarget = (targetTransform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, dashTarget, detectionRange, obstacleMask))
            yield break;

        if (Physics.Raycast(transform.position, dashTarget, out RaycastHit hit, dashDistance, obstacleMask))
            dashTarget = hit.transform.position;
        else
            dashTarget = transform.position + dashTarget * dashDistance;
        
        isDashing = true;
        agent.SetDestination(dashTarget);
        PlayDashSound();

        while (Vector3.Distance(transform.position, dashTarget) > 1f)
        {
            agent.speed += dashSpeedMultiple * Time.deltaTime;

            // 대쉬 도중 플레이어 충돌 확인
            Collider[] players = Physics.OverlapSphere(transform.position, 1f, playerMask);
            foreach (Collider target in players)
            {
                if (target.GetComponent<PlayerDataController>().isDead)
                    continue;
                    
                // 타겟이 시야각 내에 있는지 확인
                Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) <= attackAngle / 2)
                {
                    // 2. 타겟이 정면 각도 내에 있으면 공격
                    target.GetComponent<PlayerDataController>().ChangeHp(-dashDamage); // 타겟의 체력 감소
                }
            }

            if (players.Length > 0)
                break;

            yield return null; // 다음 프레임 대기
        }

        StopMoving();
        yield return new WaitForSeconds(1f);
        ResumeMoving();

        // 돌진 종료 후 NavMeshAgent 재활성화
        agent.SetDestination(targetTransform.position);
        lastDashTime = Time.time;
        isDashing = false;
    }

    public override IEnumerator Attack()
    {
        networkAnimator.SetTrigger("Attack");
        yield return base.Attack();
    }

    public override void FootStep()
    {
        AudioSource.PlayClipAtPoint(footstep, transform.position, 0.3f);
    }
    public void PlayDashSound()
    {
        AudioSource.PlayClipAtPoint(DashSound, transform.position);
    }

    public override void AnimationUpdate()
    {
        base.AnimationUpdate();
        networkAnimator.animator.SetBool("isCharge", isDashing);
    }
}
