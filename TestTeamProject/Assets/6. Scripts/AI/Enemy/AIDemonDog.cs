using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;
using Mirror;

public class AIDemonDog : EnemyObject
{
    public override void Setting()
    {
        base.Setting();

        stateInterval = 0.1f; // 상태 전환 주기
        detectionInterval = 0.1f; // 감지 주기

        patrolSpeed = 2f; // 배회 속도
        chaseSpeed = 4f; // 추적 속도

        patrolRange = 15f ; // 배회 범위
        patrolWaitTime = 0f; // 배회 대기 시간

        attackAngle = 90f; // 공격각(0 ~ 360도)
        attackRange = 1.5f; // 공격 범위
        attackDamage = 200; // 공격 데미지
        attackCooldown = 1f; // 공격 쿨타임

        viewAngle = 360f; // 시야각(0 ~ 360도)
        detectionRange = 5f; // 감지 범위
        soundDetectionRange = 20f; // 소리 감지 범위
        timeToChaseLostTarget = 5f; // 추적 범위에서 벗어나 배회로 돌아가는 시간
    }

    public override void Detect()
    {
        DetectSound();
    }

    public override void OnSoundDetected(Collider target)
    {
        base.OnSoundDetected(target);

        DetectPlayer();
    }

    public override IEnumerator Attack()
    {
        //animator.SetTrigger("Attack");
        Debug.Log("Demon Dog Attack!");

        yield return base.Attack();

        Debug.Log("Demon Dog Attack End!");
    }

}
