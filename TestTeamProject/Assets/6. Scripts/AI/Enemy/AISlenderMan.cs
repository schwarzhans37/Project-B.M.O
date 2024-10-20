using System.Collections;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
[RequireComponent(typeof(NetworkAnimator))]
public class AISlenderMan : NetworkBehaviour
{
    [SyncVar]
    public Transform player;
    [SyncVar]
    public Transform playerCamera;
    public NetworkAnimator networkAnimator;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip ScreamSE;

    public SkinnedMeshRenderer body;
    public Transform head;

    public float deathTime = 30f; // 플레이어가 바라보는 시간이 사망 시간에 도달

    public float teleportMinDistance = 21f; // 순간이동 후 플레이어와 최소 거리
    public float teleportMaxDistance = 30f; // 순간이동 후 플레이어와 최대 거리
    public float deathDistanceProportion = 0.16f; // 사망 거리 비율
    public float teleportCooldown = 2f; // 텔레포트 간 최소 시간 간격

    private float lookTime = 0f;
    private float noLookTime = 0f;
    private float lastLookTime = 0f;
    private bool isPlayerLooking = false;
    private bool isStopped = false;

    private float effectCooldown = 6f;
    private float effectTime = 0f;
    private float lastEffectTime = 0f;

    public LayerMask playerMask; // 플레이어 레이어
    public LayerMask obstacleMask; // 장애물 레이어


    Vector3 lookAtTargetPosition, lookAtPosition;
    float lookAtWeight;
    public float blendTime = 0.4f;
    public float towards = 5.0f;
    public float weightMul = 1;
    public float clampWeight = 0.5f;
    public Vector3 weight = new Vector3(0.4f, 0.8f, 0.9f);
    public bool yTargetHeadSynk;

    protected override void OnValidate()
    {
        base.OnValidate();

        playerMask = LayerMask.GetMask("Player");
        obstacleMask = LayerMask.GetMask("Obstacle");
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        networkAnimator = GetComponent<NetworkAnimator>();
        networkAnimator.animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (!isServer)
            return;

        Debug.Log("슬렌더맨이 생성되었습니다!");
    }

    void Update()
    {
        if (!isServer)
            return;

        if (isStopped)
            return;

        if (noLookTime >= deathTime
            || player == null
            || player.GetComponent<PlayerDataController>().isDead)
        {
            isStopped = true;
            StartCoroutine(TriggerSurvive());
            return;
        }
        if (lookTime >= deathTime
            || Vector3.Distance(player.position, transform.position) < teleportMinDistance * deathDistanceProportion)
        {
            isStopped = true;
            StartCoroutine(TriggerDeath());
            return;
        }

        lookAtTargetPosition = player.position + transform.forward;
        PlayEffect(player.GetComponent<NetworkIdentity>().connectionToClient);

        // 플레이어가 슬렌더맨을 보고 있는지 확인
        isPlayerLooking = IsPlayerLooking();

        float proportion = lookTime / deathTime;
        if (isPlayerLooking)
        {
            lookTime += Time.deltaTime;
            
            if (Time.time - lastLookTime > teleportCooldown * 3f)
            {
                lastLookTime = Time.time;

                Teleport(1f - proportion - deathDistanceProportion);
            }
        }
        else
        {
            noLookTime += Time.deltaTime;

            if (Time.time - lastLookTime > teleportCooldown)
            {
                lastLookTime = Time.time;

                Teleport(1f - proportion - deathDistanceProportion);
            }
        }
    }

    [TargetRpc]
    private void PlayEffect(NetworkConnectionToClient target)
    {
        if (playerCamera == null)
            return;
            
        // 여기서 시각적 또는 음향 효과를 추가할 수 있음
        float proportion = Vector3.Distance(player.position, transform.position) / teleportMaxDistance;

        playerCamera.GetComponent<NoiseAndGrain>().softness = Random.Range(1f - proportion / 2, 1f);;
        if (effectTime < Time.time)
        {
            audioSource.volume = 0;
            body.SetBlendShapeWeight(0, 0);

            if (Time.time - lastEffectTime > effectCooldown * proportion)
            {
                effectTime = Time.time + Random.Range(1.0f, 2.0f);
                lastEffectTime = Time.time;
                audioSource.pitch = Random.Range(0.8f, 1.0f);
                audioSource.volume = 1f;
                body.SetBlendShapeWeight(0, 100f);
            }
        }
    }

    private bool IsPlayerLooking()
    {

        Vector3 dirToPlayer = (transform.position - playerCamera.position).normalized;

        // AI와 플레이어 카메라 방향의 내적(Dot Product) 계산
        float dotProduct = Vector3.Dot(playerCamera.forward, dirToPlayer);

        // 내적이 0.5보다 크다면, 카메라가 AI를 바라보고 있는 것
        if (dotProduct > 0.5f)
        {
            float distanceToPlayer = Vector3.Distance(playerCamera.position, transform.position);

            // Raycast를 통해 장애물이 있는지 확인
            if (!Physics.Raycast(playerCamera.position, dirToPlayer, distanceToPlayer, obstacleMask))
            {
                return true; // 첫 번째로 감지된 플레이어만 고려
            }
        }

        return false;
    }

    private void Teleport(float distanceMultiplier)
    {
        Vector3 newSpawnPoint = GetRandomSpawnPoint(distanceMultiplier);

        if (newSpawnPoint == Vector3.zero)
            return;

        transform.position = newSpawnPoint;

        // 플레이어를 즉시 바라보도록 회전
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = lookRotation;
    }

    private Vector3 GetRandomSpawnPoint(float distanceMultiplier)
    {
        Vector3 teleportPosition;

        // 플레이어의 카메라를 기준으로 순간이동할 랜덤한 위치 찾기
        Vector3 randomViewportPoint = new Vector3(
            Random.Range(0.2f, 0.8f), // X 범위
            Random.Range(0.2f, 0.8f), // Y 범위
            Random.Range(teleportMinDistance * distanceMultiplier, teleportMaxDistance * distanceMultiplier)
        );

        teleportPosition = playerCamera.GetComponent<Camera>().ViewportToWorldPoint(randomViewportPoint);

        if (NavMesh.SamplePosition(teleportPosition, out NavMeshHit hit, (teleportMinDistance + teleportMaxDistance) / 2f * distanceMultiplier, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return player.position + deathDistanceProportion * teleportMinDistance * Vector3.forward;
    }

    private IEnumerator TriggerSurvive()
    {
        // 생존 로직 구현
        float proportion = lookTime / deathTime;
        Teleport(1f - proportion - deathDistanceProportion);

        float startTime = Time.time;
        float time = 0f;

        while (Time.time - startTime < 3f)
        {
            time += Time.deltaTime;
            PlayEffect(player.GetComponent<NetworkIdentity>().connectionToClient);

            // 현재 위치에서 뒤쪽으로 이동 (transform.forward의 반대 방향)
            Vector3 targetPosition = transform.position - transform.forward;
            transform.position = Vector3.Lerp(transform.position, targetPosition, time);

            yield return null; // 한 프레임 대기
        }

        playerCamera.GetComponent<NoiseAndGrain>().softness = 0;
        NetworkServer.Destroy(gameObject); // AI 제거
    }

    private IEnumerator TriggerDeath()
    {
        // 사망 로직 구현
        networkAnimator.animator.SetTrigger("Attack");
        
        Teleport(0.08f);

        float startTime = Time.time;
        float time = 1f;
        Vector3 originalPosition = player.position; // 현재 위치 저장

        while (Time.time - startTime < 3f)
        {
            time += Time.deltaTime / 2;
            PlayEffect(player.GetComponent<NetworkIdentity>().connectionToClient);
            player.position = originalPosition; // 플레이어 위치 고정
            playerCamera.LookAt(transform.position + Vector3.up * time); // AI를 바라보도록 카메라 회전
            player.LookAt(transform); // AI를 바라보도록 플레이어 회전

            yield return null; // 한 프레임 대기
        }

        player.GetComponent<PlayerDataController>().ChangeHp(-9999); // 플레이어 사망 처리
        player.rotation = Quaternion.identity; // 플레이어 회전 초기화
        playerCamera.GetComponent<NoiseAndGrain>().softness = 0;
        NetworkServer.Destroy(gameObject); // AI 제거
    }

    void OnAnimatorIK()
    {
        if (yTargetHeadSynk == false) lookAtTargetPosition.y = head.position.y;
        Vector3 curDir = lookAtPosition - head.position;
        curDir = Vector3.RotateTowards(curDir, lookAtTargetPosition - head.position, towards * Time.deltaTime, float.PositiveInfinity);
        lookAtPosition = head.position + curDir;
        lookAtWeight = Mathf.MoveTowards(lookAtWeight, 1, Time.deltaTime / blendTime);
        networkAnimator.animator.SetLookAtWeight(lookAtWeight * weightMul, weight.x, weight.y, weight.z, clampWeight);
        networkAnimator.animator.SetLookAtPosition(lookAtPosition);
    }

    public void Scream()
    {
        AudioSource.PlayClipAtPoint(ScreamSE, transform.position, 10.0f);
    }
}
