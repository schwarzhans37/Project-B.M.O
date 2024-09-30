using UnityEngine;

public class SlenderManSpawner : MonoBehaviour
{
    public GameObject slenderManPrefab; // 슬렌더맨 프리팹
    public float spawnProbability = 0.5f; // 스폰 확률 (50%)
    public float spawnCheckInterval = 5f; // 스폰 체크 주기

    private Transform[] players; // 플레이어들
    private GameObject currentSlenderMan; // 현재 맵에 존재하는 슬렌더맨
    private bool canSpawn = true; // 슬렌더맨 스폰 가능 여부
    private float deathTime = -1f; // 슬렌더맨 사망 시각 (-1은 아직 사망하지 않은 상태)

    void Start()
    {
        // 플레이어 찾기 (Player 태그로 찾음)
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playerObjects.Length];

        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }

        // 일정 시간 간격으로 스폰 시도
        InvokeRepeating("TrySpawnSlenderMan", spawnCheckInterval, spawnCheckInterval);
    }

    void Update()
    {
        // 슬렌더맨이 죽었고, 사망 후 1분이 지났다면 스폰 가능
        if (!canSpawn && currentSlenderMan == null && deathTime != -1f)
        {
            // 슬렌더맨 사망 후 1분 경과 시 스폰 가능하도록 설정
            if (Time.time - deathTime >= 60f)
            {
                canSpawn = true;
                deathTime = -1f; // 초기화
            }
        }
    }

    void TrySpawnSlenderMan()
    {
        // 슬렌더맨이 없고, 스폰 가능 상태일 때만 스폰 시도
        if (canSpawn && currentSlenderMan == null)
        {
            foreach (Transform player in players)
            {
                if (Random.value < spawnProbability) // 50% 확률로 스폰 시도
                {
                    // 플레이어의 카메라 찾기
                    Transform playerCam = player.Find("PlayerCam");

                    if (playerCam != null)
                    {
                        // 플레이어 뷰포인트 내에서 임의의 위치 찾기
                        Vector3 spawnPosition = FindSpawnPositionInViewport(playerCam);
                        // 슬렌더맨 스폰
                        currentSlenderMan = Instantiate(slenderManPrefab, spawnPosition, Quaternion.identity);
                        
                        // 슬렌더맨이 스폰되었으므로 스폰 금지
                        canSpawn = false;

                        // 슬렌더맨이 사망할 때 호출할 메서드를 연결
                        AISlenderMan slenderManScript = currentSlenderMan.GetComponent<AISlenderMan>();
                        slenderManScript.OnSlenderManDeath += OnSlenderManDeath;
                    }
                }
            }
        }
    }

    Vector3 FindSpawnPositionInViewport(Transform playerCam)
    {
        // 카메라의 뷰포인트 내에서 임의의 위치 찾기
        Camera cam = playerCam.GetComponent<Camera>();

        Vector3 randomViewportPoint = new Vector3(
            Random.Range(0.2f, 0.8f), // X 범위 (중앙을 피해서 스폰)
            Random.Range(0.2f, 0.8f), // Y 범위 (중앙을 피해서 스폰)
            Random.Range(3f, 10f)     // Z 범위 (거리 3~10 사이)
        );

        return cam.ViewportToWorldPoint(randomViewportPoint);
    }

    void OnSlenderManDeath()
    {
        // 슬렌더맨이 사망하면 사망 시각을 기록하고 스폰 가능 여부를 설정
        deathTime = Time.time;
        currentSlenderMan = null;
    }
}
