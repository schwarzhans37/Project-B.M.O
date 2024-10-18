using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public AudioClip burning; // 불붙는 소리 클립
    private AudioSource audioSource; // 오디오 소스 컴포넌트

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = burning; // AudioClip 할당
        audioSource.loop = true; // 반복 재생 활성화
        audioSource.volume = 0.01f;
        audioSource.Play(); // 시작할 때 소리 재생
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive); // 횃불의 활성화 상태 설정

        if (!isActive)
        {
            audioSource.Stop(); // 비활성화될 때 소리 멈추기
        }
    }

    void Update()
    {
    }
}