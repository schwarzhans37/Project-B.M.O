using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class FieldPlayerKiller : NetworkBehaviour
{
    [Range(0, 1000f)] public float killRange; // 플레이어를 죽일 범위

    public void KillPlayer()
    {
        // 서버가 아니거나 플레이어가 없으면 리턴
        if (!isServer)
            return;

        Collider[] players = Physics.OverlapSphere(transform.position, killRange, LayerMask.GetMask("Player"));
        if (players.Length == 0)
            return;

        foreach (Collider player in players)
        {
            player.GetComponent<PlayerDataController>().isDead = true;
        }
    }

    // Gizmos를 사용하여 스폰 범위 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRange);
    }
}
