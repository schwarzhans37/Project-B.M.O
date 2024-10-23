using UnityEngine;
using System.Collections;

public class MinotaurAI : EnemyObject
{

    public float dashDistance; // 대쉬 거리
    public float dashSpeed; // 대쉬 속도
    public float dashCooldown; // 대쉬 쿨타임
    private float lastDashTime; // 마지막 대쉬 시간
    private bool isDashing = false; // 대쉬 중인지 여부

    public AudioClip DashSound; // 대쉬 사운드

    protected override void OnValidate()
    {
        base.OnValidate();

        dashDistance = 20f; // 대쉬 거리
        dashSpeed = 8f; // 대쉬 속도
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

        attackAngle = 90f; // 공격각(0 ~ 360도)
        attackRange = 2f; // 공격 범위
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

    public override void OnPlayerDetected(Collider target)
    {
        base.OnPlayerDetected(target);

        if (!isDashing && Time.time - lastDashTime > dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash()
    {
        Debug.Log("Dash!");
        isDashing = true;
        StopMoving();
        PlayDashSound();
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
            if (Physics.Raycast(transform.position, dashDirection, moveDistance, obstacleMask))
            {
                lastDashTime = Time.time;
                isDashing = false;
                yield return new WaitForSeconds(3f);
                break;
            }
            if (Physics.Raycast(transform.position, dashDirection, moveDistance, playerMask))
            {
                MeleeAttack();
                break;
            }

            // 위치 업데이트
            transform.position = nextPosition;

            // 이동한 거리 업데이트
            distanceTravelled += moveDistance;

            yield return null; // 다음 프레임 대기
        }

        // 돌진 종료 후 NavMeshAgent 재활성화
        lastDashTime = Time.time;
        isDashing = false;
        ResumeMoving();
    }
    public override IEnumerator Attack()
    {
        networkAnimator.animator.SetTrigger("Attack");
        yield return base.Attack();
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
