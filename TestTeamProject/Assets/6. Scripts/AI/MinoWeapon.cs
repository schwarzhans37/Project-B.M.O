using UnityEngine;

public class MinoWeapon : MonoBehaviour
{
    public int attackDamage = 50; // 무기의 데미지
    private bool isAttacking; // 현재 공격 중인지 확인

    void Start()
    {
        isAttacking = false;
    }

    // 미노타우로스 AI에서 공격 시작 시 호출
    public void StartAttack()
    {
        isAttacking = true;
    }

    // 미노타우로스 AI에서 공격 종료 시 호출
    public void EndAttack()
    {
        isAttacking = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // 공격 중이고, 충돌한 대상이 플레이어일 경우
        if (isAttacking && other.CompareTag("Player"))
        {
            // 플레이어에게 데미지 적용 (플레이어 스크립트에 따라 수정 필요)
            other.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }
}

