using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class AISlenderManBase : MonoBehaviour
{
    public float teleportMinDistance = 2f; // 순간이동 후 플레이어와 최소 거리
    public float teleportMaxDistance = 8f; // 순간이동 후 플레이어와 최대 거리
    public float attackMinDistance = 1f; // 노이즈와 데미지 발생 최소 거리
    public float damagePerSecond = 5f; // 초당 플레이어에게 주는 데미지
    public float teleportInterval = 2f; // 2초마다 순간이동

    private Transform player; // 추적할 플레이어
    private Transform playerCam; // 플레이어의 카메라
    private float damageTimer; // 데미지 타이머
    private float lookAwayTimer; // 플레이어가 쳐다보지 않은 시간 카운터
    private float teleportTimer; // 순간이동 타이머

    private SkinnedMeshRenderer slenderRenderer; // 슬렌더 맨 모델의 SkinnedMeshRenderer

    private PostProcessVolume postProcessVolume; // 포스트 프로세스 볼륨
    private Grain grain; // 노이즈 효과 (Grain)

    private float targetGrainIntensity = 0f; // 노이즈의 목표 강도
    private float grainLerpSpeed = 1f; // 노이즈 변화 속도
    
    public event System.Action OnSlenderManDeath;

    void Start()
    {
        FindNearestPlayer();
        slenderRenderer = transform.Find("Slender").GetComponent<SkinnedMeshRenderer>();

        // PlayerCam의 Post-Processing Volume 가져오기
        postProcessVolume = playerCam.GetComponent<PostProcessVolume>();

        // 노이즈 효과 초기화
        if (postProcessVolume.profile.TryGetSettings(out grain))
        {
            grain.active = true; // 노이즈 효과 활성화
            grain.intensity.value = 0f; // 노이즈 초기화
        }
        else
        {
            Debug.LogError("Grain settings not found in PostProcessProfile!");
        }
    }

    void Update()
    {
        if (player == null) return;

        AttackPlayer();

        // 노이즈 효과 조절
        grain.intensity.value = Mathf.Lerp(grain.intensity.value, targetGrainIntensity, Time.deltaTime * grainLerpSpeed);
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;

        foreach (GameObject p in players)
        {
            float distance = Vector3.Distance(transform.position, p.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                player = p.transform;
                playerCam = p.transform.Find("PlayerCam");
            }
        }
    }

    void AttackPlayer()
    {
        Vector3 viewportPosition = playerCam.GetComponent<Camera>().WorldToViewportPoint(transform.position);
        bool isInViewport = viewportPosition.z > 0 && viewportPosition.x > 0 && viewportPosition.x < 1 && viewportPosition.y > 0 && viewportPosition.y < 1;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 뷰포트 내에 있고, 거리가 attackMinDistance 이상, teleportMaxDistance 이하일 때만 효과 발생
        if (isInViewport && distanceToPlayer >= attackMinDistance && distanceToPlayer <= teleportMaxDistance)
        {
            // 플레이어가 바라보고 있다면 데미지 주기
            damageTimer += Time.deltaTime;

            if (damageTimer >= 1f)
            {
                DealDamageToPlayer();
                damageTimer = 0f; // 타이머 초기화
            }

            targetGrainIntensity = 1f; // 노이즈 증가
            lookAwayTimer = 0f; // 시선 돌린 시간 초기화

            // 플레이어를 계속 바라보도록 회전
            FacePlayerInstantly();
        }
        else
        {
            // 플레이어가 바라보지 않거나, 거리가 teleportMaxDistance보다 클 때
            lookAwayTimer += Time.deltaTime;
            targetGrainIntensity = 0f; // 노이즈 감소

            // teleportInterval 간격마다 순간이동
            teleportTimer += Time.deltaTime;
            if (teleportTimer >= teleportInterval)
            {
                TeleportToPlayerViewport();
                teleportTimer = 0f; // 순간이동 타이머 초기화
            }

            // 10초 이상 바라보지 않으면 슬렌더맨 사망
            if (lookAwayTimer >= 10f)
            {
                Die();
            }
        }
    }

    void TeleportToPlayerViewport()
    {
        // 순간이동 위치 찾기
        Vector3 teleportPosition = FindTeleportPositionInViewport();
        transform.position = teleportPosition;

        // 슬렌더맨이 순간적으로 플레이어를 바라보도록 즉시 회전
        FacePlayerInstantly();
    }

    void FacePlayerInstantly()
    {
        // 플레이어를 즉시 바라보도록 회전
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = lookRotation;
    }

    Vector3 FindTeleportPositionInViewport()
    {
        Vector3 teleportPosition;
        int safetyCounter = 0;

        do
        {
            // 플레이어의 카메라를 기준으로 순간이동할 랜덤한 위치 찾기
            Vector3 randomViewportPoint = new Vector3(
                Random.Range(0.2f, 0.8f), // X 범위 (중앙을 피해서 스폰)
                Random.Range(0.2f, 0.8f), // Y 범위 (중앙을 피해서 스폰)
                Random.Range(teleportMinDistance, teleportMaxDistance)
            );

            teleportPosition = playerCam.GetComponent<Camera>().ViewportToWorldPoint(randomViewportPoint);

            // 안전하게 순간이동할 위치 찾기 위한 최대 시도 횟수
            safetyCounter++;
        } while (Vector3.Distance(teleportPosition, player.position) < teleportMinDistance 
                 || Vector3.Distance(teleportPosition, player.position) > teleportMaxDistance
                 && safetyCounter < 10); // 10번 시도 후에 탈출

        return teleportPosition;
    }

    void SetTransparency(float alpha)
    {
        // 슬렌더 맨의 메인 `Material`의 투명도 변경
        Color color = slenderRenderer.material.color;
        color.a = alpha;
        slenderRenderer.material.color = color;
    }

    void DealDamageToPlayer()
    {
        // PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        
        // if (playerHealth != null)
        // {
        //     playerHealth.TakeDamage(Mathf.RoundToInt(damagePerSecond));
        // }
    }

    void Die()
    {
        Debug.Log("Slender Man has died.");
        
        // 사망 시 이벤트 호출
        OnSlenderManDeath?.Invoke();

        // 슬렌더맨 오브젝트 파괴
        Destroy(gameObject);
    }
}
