using System.Linq;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class SlenderManSpawner : NetworkBehaviour
{
    public GameObject slenderManPrefab; // 슬렌더맨 프리팹
    private GameObject spawnedSlenderMan; // 현재 스폰된 슬렌더맨
    private Collider targetPlayer; // 플레이어 타겟

    public int currentLevel; // 현재 레벨에 따라 변경 (레벨 정보를 가져와서 업데이트)
    public float LevelMultiplier => 1f + (currentLevel * 0.2f); // 레벨 배율
    [Range(0, 1000f)] public float spawnRange; // 스폰 범위
    [Range(0, 1f)] public float spawnProbability; // 스폰 확률
    [Range(0, 100f)] public float spawnDelay; // 스폰 딜레이
    private float spawnTimer; // 스폰 타이머

    public void SpawnSlenderMan()
    {
        // 서버가 아니거나 스폰된 슬렌더맨이 이미 있으면 리턴
        if (!isServer
            || spawnedSlenderMan != null)
            return;

        // 스폰할 플레이어가 없으면 리턴
        Collider[] players = Physics.OverlapSphere(transform.position, spawnRange, LayerMask.GetMask("Player"));
        if (players.Length == 0)
            return;

        // 타겟 플레이어가 없거나 죽었으면 다시 타겟 설정
        if (!players.Contains(targetPlayer)
            || targetPlayer.GetComponent<PlayerDataController>().isDead)
        {
            spawnTimer = 0f;
            targetPlayer = players[Random.Range(0, players.Length)];
            return;
        }

        // 타겟 플레이어 주변에 다른 플레이어가 있으면 다시 타겟 설정
        Collider[] colliders = Physics.OverlapSphere(targetPlayer.transform.position, 40f, LayerMask.GetMask("Player"));
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlayerDataController>().isDead
                || collider == targetPlayer)
                continue;

            spawnTimer = 0f;
            targetPlayer = players[Random.Range(0, players.Length)];
            return;
        }

        // 스폰 타이머가 스폰 딜레이보다 작으면 리턴
        spawnTimer += Time.deltaTime;
        if (spawnTimer < spawnDelay)
            return;

        // 스폰 확률이 낮으면 리턴
        if (Random.Range(0f, 1f) > spawnProbability * LevelMultiplier)
        {
            spawnTimer = 0f;
            return;
        }

        // 스폰
        spawnedSlenderMan = Instantiate(slenderManPrefab, targetPlayer.transform.position + Vector3.back * 40, Quaternion.identity);
        spawnedSlenderMan.GetComponent<AISlenderMan>().player = targetPlayer.transform;
        NetworkServer.Spawn(spawnedSlenderMan);

        // 스폰 후 초기화
        targetPlayer = null;
        spawnTimer = 0f;
    }

    public void RemoveSlenderMan()
    {
        if (!isServer
            || spawnedSlenderMan == null)
            return;

        NetworkServer.Destroy(spawnedSlenderMan);
        spawnedSlenderMan = null;
    }

    // Gizmos를 사용하여 스폰 범위 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}
