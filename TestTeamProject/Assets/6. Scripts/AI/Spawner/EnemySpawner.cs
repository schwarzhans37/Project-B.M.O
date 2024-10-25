using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NetworkIdentity))]
public class EnemySpawner : NetworkBehaviour
{
    [System.Serializable]
    public class SpawnConfiguration
    {
        public GameObject enemyPrefab; // 적 프리팹
        [Range(0, 1f)] public float minSpawnRatio; // 최소 스폰 비율
        [Range(0, 1f)] public float maxSpawnRatio; // 최대 스폰 비율
    }

    public SpawnConfiguration[] spawnConfigurations; // 스폰 설정 배열
    [Range(0, 1000f)] public float spawnRange; // 스폰 범위
    [Range(0, 100f)] public float spawnInterval; // 적 스폰 간의 최소 거리
    [Range(0, 10f)] public float navMeshSampleDistance; // 네비게이션 메쉬 샘플링 거리
    public int baseEnemyCount; // 기본 스폰할 적 수

    public int currentLevel; // 현재 레벨에 따라 변경 (레벨 정보를 가져와서 업데이트)
    float LevelMultiplier => 1f + (currentLevel * 0.25f); // 레벨 배율

    private readonly List<GameObject> spawnedEnemies = new(); // 현재 스폰된 적 리스트

    public void SpawnEnemies(int level)
    {
        if (!isServer)
            return;

        currentLevel = level;

        int totalEnemiesToSpawn = Mathf.CeilToInt(baseEnemyCount * LevelMultiplier);

        // 각 적 유형의 스폰할 수 있는 수 계산
        Dictionary<GameObject, int> spawnLimits = CalculateSpawnLimits(totalEnemiesToSpawn);

        int spawnedCount = 0;
        while (spawnedCount < totalEnemiesToSpawn)
        {
            Vector3 spawnPosition = FindValidSpawnPosition();

            if (spawnPosition != Vector3.zero)
            {
                GameObject selectedEnemy = SelectEnemyToSpawn(spawnLimits);
                if (selectedEnemy != null)
                {
                    GameObject newEnemy = Instantiate(selectedEnemy, spawnPosition, Quaternion.identity);
                    NetworkServer.Spawn(newEnemy);

                    spawnedEnemies.Add(newEnemy);
                    spawnedCount++;
                }
            }
        }
    }

    public void RemoveEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            NetworkServer.Destroy(enemy);
        }
        spawnedEnemies.Clear();
    }

    // 난이도에 따른 적 비율 계산
    Dictionary<GameObject, int> CalculateSpawnLimits(int totalEnemiesToSpawn)
    {
        Dictionary<GameObject, int> spawnLimits = new();

        foreach (var config in spawnConfigurations)
        {
            spawnLimits[config.enemyPrefab] = Mathf.CeilToInt(Random.Range(config.minSpawnRatio, config.maxSpawnRatio) * totalEnemiesToSpawn);
        }
        return spawnLimits;
    }

    // 유효한 스폰 위치를 찾는 함수
    Vector3 FindValidSpawnPosition()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRange;
        randomPoint.y = transform.position.y;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            Vector3 spawnPosition = hit.position;
            if (IsValidPosition(spawnPosition))
            {
                return spawnPosition;
            }
        }
        return Vector3.zero;
    }

    // 현재 스폰된 다른 적들과의 간격을 확인
    bool IsValidPosition(Vector3 position)
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (Vector3.Distance(position, enemy.transform.position) < spawnInterval)
            {
                return false;
            }
        }
        return true;
    }

    // 스폰할 적 유형 선택
    GameObject SelectEnemyToSpawn(Dictionary<GameObject, int> spawnLimits)
    {
        // 적 유형별 스폰 한도에 따라 누적된 가중치를 계산
        int totalWeight = 0;
        foreach (var pair in spawnLimits)
        {
            if (pair.Value > 0)
            {
                totalWeight += pair.Value;
            }
        }

        if (totalWeight <= 0)
        {
            return null; // 스폰 가능한 적이 없을 경우
        }

        // 가중치에 따라 적을 선택
        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var pair in spawnLimits)
        {
            if (pair.Value > 0)
            {
                currentWeight += pair.Value;

                // 누적된 가중치가 랜덤 값 이상일 경우 선택
                if (randomWeight < currentWeight)
                {
                    // 선택된 적의 스폰 한도 감소
                    spawnLimits[pair.Key]--;
                    return pair.Key;
                }
            }
        }

        return null; // 예상치 못한 경우 null 반환
    }

    // Gizmos를 사용하여 스폰 범위 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}