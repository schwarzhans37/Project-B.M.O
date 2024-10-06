using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public class AIWeepAngel : EnemyObject
{
    private bool isPlayerLooking = false; // 플레이어가 우는 천사를 바라보는지 여부

    protected override void OnValidate()
    {
        base.OnValidate();

        patrolSpeed = 1.5f; // 배회 속도
        chaseSpeed = 10f; // 추적 속도

        attackAngle = 90f; // 공격각(0 ~ 360도)
        attackRange = 1f; // 공격 범위
        attackDamage = 2000; // 공격 데미지
        attackCooldown = 1f; // 공격 쿨타임

        viewAngle = 180f; // 시야각(0 ~ 360도)
        patrolRange = 15f ; // 배회 범위
        detectionRange = 40f; // 감지 범위
        soundDetectionRange = 0f; // 소리 감지 범위
        detectionLossTime = 5f; // 추적 범위에서 벗어나 배회로 돌아가는 시간
    }

    public override void Start()
    {
        base.Start();

        StartCoroutine(StartAI(0.1f));
        StartCoroutine(StartDetection(0.1f));
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
            DetectPlayer();
            IsPlayerLookingAtMe();

            yield return new WaitForSeconds(interval);
        }
    }

    public override void Chase()
    {
        if (isPlayerLooking)
            return;

        base.Chase();
    }

    public override IEnumerator PerformAttack()
    {
        lastAttackTime = Time.time; // 공격 후 쿨타임 초기화
        agent.isStopped = true; // 이동 멈춤

        yield return new WaitForSeconds(2f);
    }

    public void IsPlayerLookingAtMe()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRange, playerMask);

        foreach (Collider player in players)
        {
            // 플레이어의 카메라를 가져오기
            Transform playerCam = player.GetComponentInChildren<Camera>(true).transform;
            Vector3 dirToPlayer = (transform.position - playerCam.position).normalized;

            // AI와 플레이어 카메라 방향의 내적(Dot Product) 계산
            float dotProduct = Vector3.Dot(playerCam.forward, dirToPlayer);

            // 내적이 0보다 크다면, 카메라가 AI를 바라보고 있는 것
            if (dotProduct > 0.5f)
            {
                float distanceToPlayer = Vector3.Distance(playerCam.position, transform.position);

                // Raycast를 통해 장애물이 있는지 확인
                if (!Physics.Raycast(playerCam.position, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    Debug.Log("Player is looking at me!");
                    isPlayerLooking = true;
                    agent.isStopped = true; // AI 이동 멈춤
                    break; // 첫 번째로 감지된 플레이어만 고려
                }
            }
        }

        // 플레이어가 감지되지 않으면 이동 재개
        if (!isPlayerLooking)
        {
            agent.isStopped = false;
        }
        isPlayerLooking = false;
    }
}
