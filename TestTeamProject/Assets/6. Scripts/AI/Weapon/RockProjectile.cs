using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    public float speed = 10f; // 바위 투척 속도
    public float explosionRadius = 4f; // 충격파 범위
    public float directHitDamage; // 직접 맞았을 때의 데미지
    public float aoeDamage; // 충격파 데미지
    public GameObject explosionEffectPrefab; // 충격파 효과 프리팹

    private Vector3 targetPosition; // 바위가 향할 목표 위치

    // 초기화 메서드 (발사 시 호출)
    public void Initialize(Vector3 target, float directDamage, float aoeDmg, float aoeRadius)
    {
        targetPosition = target;
        directHitDamage = directDamage;
        aoeDamage = aoeDmg;
        explosionRadius = aoeRadius;
    }

    void Update()
    {
        // 바위가 목표 위치로 이동
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // 목표 위치에 도달했거나, 플레이어와 충돌했을 경우 폭발
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Explode();
        }
    }

    // 충돌 판정
    void OnTriggerEnter(Collider other)
    {
        // 만약 충돌한 오브젝트가 플레이어라면 즉시 폭발
        if (other.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        // 충격파 효과 생성 (선택 사항)
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 범위 내 모든 오브젝트 가져오기
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            // 충돌한 오브젝트가 플레이어일 경우
            if (hitCollider.CompareTag("Player"))
            {
                // 직접 충돌 시 데미지 적용
                float distanceToHit = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distanceToHit < 1f) // 충돌이 거의 정확할 때
                {
                    // 데미지를 int로 변환하여 적용
                    // hitCollider.GetComponent<PlayerHealth>().TakeDamage(Mathf.RoundToInt(directHitDamage));
                }
                else // 범위 충격파 데미지
                {
                    // 데미지를 int로 변환하여 적용
                    // hitCollider.GetComponent<PlayerHealth>().TakeDamage(Mathf.RoundToInt(aoeDamage));
                }
            }
        }

        // 바위 투척 오브젝트 삭제
        Destroy(gameObject);
    }
}
