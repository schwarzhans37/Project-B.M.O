using System.Collections;
using Mirror;
using UnityEngine;

public class AIGolem : EnemyObject
{
    public float meleeRange; // 근접 공격을 선택할 거리
    public float meleeAttackCooldown; // 근접 공격 쿨타임
    private float lastMeleeAttackTime; // 마지막 근접 공격 시간

    public float rangedAttackRange; // 원거리 공격을 선택할 거리
    public float rangedAttackForce; // 원거리 공격 힘
    public int rangedAttackDamage; // 원거리 공격 데미지
    public float shockwaveRadius; // 충격파 범위
    public int shockwaveDamage; // 충격파 데미지
    public float rangedAttackCooldown;// 원거리 공격 쿨타임
    private float lastRangedAttackTime; // 마지막 원거리 공격 시간
    private Vector3 targetPosition; // 투척할 대상 위치

    public GameObject rockProjectilePrefab; // 바위 투척에 사용할 프리팹

    protected override void OnValidate()
    {
        base.OnValidate();

        meleeRange = 3f; // 근접 공격을 선택할 거리
        meleeAttackCooldown = 5f; // 근접 공격 쿨타임
        rangedAttackRange = 40f; // 원거리 공격을 선택할 거리
        rangedAttackForce = 40f; // 원거리 공격 힘
        rangedAttackDamage = 1000; // 원거리 공격 데미지
        shockwaveRadius = 4f; // 충격파 범위
        shockwaveDamage = 500; // 충격파 데미지
        rangedAttackCooldown = 15f; // 원거리 공격 쿨타임
    }

    public override void Setting()
    {
        base.Setting();

        lastMeleeAttackTime = -meleeAttackCooldown;
        lastRangedAttackTime = -rangedAttackCooldown;

        stateInterval = 0.5f; // 상태 전환 주기
        detectionInterval = 0.1f; // 감지 주기

        patrolSpeed = 1f; // 배회 속도
        chaseSpeed = 3f; // 추적 속도

        patrolRange = 15f ; // 배회 범위
        patrolWaitTime = 3f; // 배회 대기 시간

        attackAngle = 180f; // 공격각(0 ~ 360도)
        attackRange = rangedAttackRange; // 공격 범위
        attackDamage = 900; // 공격 데미지
        attackCooldown = 0f; // 공격 쿨타임

        viewAngle = 90f; // 시야각(0 ~ 360도)
        detectionRange = 30f; // 감지 범위
        soundDetectionRange = 0f; // 소리 감지 범위
        timeToChaseLostTarget = 3f; // 추적 범위에서 벗어나 배회로 돌아가는 시간
    }

    public override void Detect()
    {
        DetectPlayer();
    }

    public override IEnumerator Attack()
    {
        if (Vector3.Distance(transform.position, targetTransform.position) < meleeRange
            && Time.time - lastMeleeAttackTime > meleeAttackCooldown)
        {
            lastMeleeAttackTime = Time.time;

            networkAnimator.animator.SetTrigger("Attack");

            yield return base.Attack();

        }
        else if (Vector3.Distance(transform.position, targetTransform.position) < rangedAttackRange
            && Time.time - lastRangedAttackTime > rangedAttackCooldown)
        {
            lastRangedAttackTime = Time.time;
            targetPosition = targetTransform.position + Vector3.up * 2f;

            
            networkAnimator.animator.SetTrigger("rock");
            yield return base.Attack();
        }
    }

    public override void RangedAttack()
    {
        if (!isServer)
            return;

        Vector3 spawnPosition = transform.position + transform.up * 5f + transform.forward * 3f;

        GameObject rockProjectile = Instantiate(rockProjectilePrefab, spawnPosition, Quaternion.identity);
        rockProjectile.GetComponent<RockProjectile>().Initialize(rangedAttackDamage, shockwaveDamage, shockwaveRadius);
        
        Vector3 direction = (targetPosition - spawnPosition).normalized;
        NetworkServer.Spawn(rockProjectile);
        rockProjectile.GetComponent<Rigidbody>().AddForce(direction * rangedAttackForce, ForceMode.Impulse);
    }

    public override void FootStep()
    {
        AudioSource.PlayClipAtPoint(footstep, transform.position,0.1f);
    }
}
