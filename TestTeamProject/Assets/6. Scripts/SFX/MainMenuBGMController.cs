using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    public AudioClip backgroundMusic; // 메인 메뉴 배경음악 클립
    private AudioSource audioSource; // AudioSource 컴포넌트

    public float delayAfterMusicEnds = 30f; // 음악 끝난 후 다시 재생까지 대기 시간 (초)

    private void Start()
    {
        // AudioSource 초기화
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = false; // 루프 재생 비활성화
        audioSource.playOnAwake = true; // 시작 시 자동 재생
        audioSource.volume = 0.1f; // 볼륨 조절

        // 배경음악 재생 및 코루틴 시작
        audioSource.Play();
        StartCoroutine(LoopWithDelay());
    }

    private IEnumerator LoopWithDelay()
    {
        // AudioSource가 존재하고 클립이 유효할 때 반복
        while (audioSource != null && backgroundMusic != null)
        {
            // 음악이 끝날 때까지 대기
            while (audioSource.isPlaying)
            {
                yield return null;
            }

            // 대기 시간 동안 멈춤
            yield return new WaitForSeconds(delayAfterMusicEnds);

            // 음악 다시 재생
            audioSource.Play();
        }
    }
}