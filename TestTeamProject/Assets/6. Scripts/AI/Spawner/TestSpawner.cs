using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TestSpawner : NetworkBehaviour
{
    public GameObject enemyPrefab; // 적 프리팹

    public void SpawnEnemies()
    {
        if (!isServer)
            return;

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        GameObject newEnemy = Instantiate(enemyPrefab, player.position + Vector3.back * 40, Quaternion.identity);

        newEnemy.GetComponent<AISlenderMan>().player = player;

        NetworkServer.Spawn(newEnemy);
    }
}
