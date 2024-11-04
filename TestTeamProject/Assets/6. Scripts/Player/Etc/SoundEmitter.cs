using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public float duration;  // 사운드 지속 시간
    private SphereCollider soundCollider;

    void Start()
    {
        // SphereCollider를 추가하여 감지 영역을 설정
        soundCollider = gameObject.AddComponent<SphereCollider>();
        // 소리 감지용 레이어 할당
        gameObject.layer = LayerMask.NameToLayer("Sound");  // "Sound" 레이어 할당
        // 일정 시간 후 자동으로 파괴
        Destroy(gameObject, duration);
    }
}
