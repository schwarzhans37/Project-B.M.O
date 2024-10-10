using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NetworkIdentity))]
public class EnemySpawner : NetworkBehaviour
{
    public GameObject[] enemyPrefabs; // 스폰할 적 캐릭터
    public float spawnRange; // 스폰 범위
    public float spawnInterval; // 스폰 간격
    public float navMeshSampleDistance; // 네비게이션 메쉬 샘플링 거리

    private List<GameObject> spawnedEnemies = new(); // 현재 스폰된 적 리스트

    void Start()
    {
        if (!isServer)
            return;
        SpawnEnemiesWithSpacing();
    }

    // 적을 간격을 두고 스폰하는 함수
    void SpawnEnemiesWithSpacing()
    {
        int i = 0;
        while (i < 10)
        {
            // 적을 스폰할 유효한 위치 찾기
            Vector3 spawnPosition = FindValidSpawnPosition();

            // 유효한 위치를 찾은 경우 적 스폰
            if (spawnPosition != Vector3.zero)
            {
                // 랜덤한 적 프리팹 선택
                int randomIndex = Random.Range(0, enemyPrefabs.Length);
                GameObject newEnemy = Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);

                NetworkServer.Spawn(newEnemy); // 네트워크 상에서 적 스폰
                Debug.Log("Enemy spawned at " + spawnPosition);

                // 스폰된 적을 리스트에 추가
                spawnedEnemies.Add(newEnemy);
                i++;
            }
        }
    }
    // 유효한 스폰 위치를 찾는 함수
    Vector3 FindValidSpawnPosition()
    {
        // 무작위 위치 생성
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRange;
        randomPoint.y = transform.position.y; // y 값 고정

        // NavMesh에서 해당 위치가 유효한지 검사
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            Vector3 spawnPosition = hit.position;

            // 다른 적들과의 최소 거리 확인
            if (IsValidPosition(spawnPosition))
            {
                return spawnPosition; // 유효한 위치 반환
            }
        }
        return Vector3.zero; // 유효한 위치를 찾지 못한 경우
    }

    // 현재 스폰된 다른 적들과의 간격을 확인하는 함수
    bool IsValidPosition(Vector3 position)
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (Vector3.Distance(position, enemy.transform.position) < spawnInterval)
            {
                return false; // 최소 거리보다 가까운 경우 유효하지 않음
            }
        }
        return true; // 모든 적들과의 간격이 유효한 경우
    }

    // Gizmos를 사용하여 스폰 범위와 스폰된 적들 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange); // 스폰 범위 표시
    }
}
