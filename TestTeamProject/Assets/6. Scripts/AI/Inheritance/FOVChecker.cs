using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVChecker : MonoBehaviour
{
    public float viewRadius = 10f; // 탐색 반경
    [Range(0, 360)] public float viewAngle = 90f; // 시야각(0 ~ 360도)

    public LayerMask targetMask; // 탐색할 타겟 (예: 플레이어의 레이어)
    public LayerMask obstacleMask; // 장애물 레이어 (예: 벽)

    public List<Transform> visibleTargets = new List<Transform>(); // 감지된 객체 목록

    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0.2f); // 0.2초마다 타겟을 감지하도록 설정
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (Collider target in targetsInViewRadius)
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
                    visibleTargets.Add(targetTransform); // 타겟이 시야 내에 있고 장애물에 막히지 않았다면 추가
                    Debug.Log("Detected Target: " + targetTransform.name);
                }
            }
        }
    }

    // Gizmo로 시야각을 시각적으로 표시 (디버깅용)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
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
