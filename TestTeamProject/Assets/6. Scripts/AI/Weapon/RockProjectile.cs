using System;
using System.Collections;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
public class RockProjectile : NetworkBehaviour
{
    public int damage; // 공격 데미지
    public float shockwaveRadius; // 충격파 범위
    public int shockwaveDamage; // 충격파 데미지

    private bool isTriggered = false;

    public LayerMask playerMask; // 플레이어 레이어

    // 초기화 메서드 (발사 시 호출)
    public void Initialize(int damage, int shockwaveDamage, float shockwaveRadius)
    {
        playerMask = LayerMask.GetMask("Player");

        this.damage = damage;
        this.shockwaveDamage = shockwaveDamage;
        this.shockwaveRadius = shockwaveRadius;
    }

    // 충돌 판정
    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;
        if (isTriggered)
            return;

        // 피격 대상이 존재하는지 확인
        if (other.gameObject.CompareTag("Player"))
        {
            // 피격 대상에게 데미지를 입힘
            other.GetComponent<PlayerDataController>().ChangeHp(-damage);
        }

        // 충격파 생성
        Collider[] targets = Physics.OverlapSphere(transform.position, shockwaveRadius, playerMask);

        foreach (Collider target in targets)
        {
            target.GetComponent<PlayerDataController>().ChangeHp(-shockwaveDamage);
        }

        isTriggered = true;

        // 파괴
        StartCoroutine(DestroyRock());
    }

    IEnumerator DestroyRock()
    {
        yield return new WaitForSeconds(1f);
        NetworkServer.Destroy(gameObject);
    }
}
