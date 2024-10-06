using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public class AIDemonDog : EnemyObject
{
    protected override void OnValidate()
    {
        base.OnValidate();

        patrolSpeed = 2f; // 배회 속도
        chaseSpeed = 4f; // 추적 속도

        attackAngle = 90f; // 공격각(0 ~ 360도)
        attackRange = 1.5f; // 공격 범위
        attackDamage = 200; // 공격 데미지
        attackCooldown = 1f; // 공격 쿨타임

        viewAngle = 360f; // 시야각(0 ~ 360도)
        patrolRange = 15f ; // 배회 범위
        detectionRange = 5f; // 감지 범위
        soundDetectionRange = 20f; // 소리 감지 범위
        detectionLossTime = 5f; // 추적 범위에서 벗어나 배회로 돌아가는 시간
    }

    public override IEnumerator StartAI(float interval)
    { 
        while (true)
        {
            switch (currentState)
            {
                case EnemyState.Patrolling:
                    Patrol();
                    break;
                case EnemyState.Chasing:
                    Chase();
                    break;
            }

            yield return new WaitForSeconds(interval);
        }
    }

    public override IEnumerator StartDetection(float interval)
    {
        while (true)
        {
            DetectSound();

            yield return new WaitForSeconds(interval);
        }
    }

    public override void DetectSound()
    {
        Debug.Log("DetectSound");
        Collider[] sounds = Physics.OverlapSphere(transform.position, soundDetectionRange, soundMask)
            .OrderBy(col => Vector3.Distance(transform.position, col.transform.position)).ToArray();

        foreach (Collider sound in sounds)
        {
            Debug.Log("DetectSound2");
            Transform soundTransform = sound.transform;
            Vector3 dirToTarget = (soundTransform.position - transform.position).normalized;

            // 타겟까지의 거리 계산
            float distanceToTarget = Vector3.Distance(transform.position, soundTransform.position);

            // 타겟까지 Ray를 쏴서 장애물에 막히지 않았는지 확인
            if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
            {
                Debug.Log("DetectSound3");
                currentState = EnemyState.Chasing;
                this.targetTransform = soundTransform;

                DetectPlayer(); // 플레이어 감지

                // 첫 번째 타겟만 추적하기 위해 반복문 종료
                break;
            }
        }
    }

    public override IEnumerator PerformAttack()
    {
        // animator.SetTrigger("Attack");
        Debug.Log("Demon Dog Attack!");

        yield return base.PerformAttack();

        Debug.Log("Demon Dog Attack End!");
    }

}
