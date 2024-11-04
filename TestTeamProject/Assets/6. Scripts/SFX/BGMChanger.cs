using UnityEngine;

public class BGMChanger : MonoBehaviour
{
    public AudioSource forestBGM;  // 숲 지역 배경음악
    public AudioSource dungeonBGM; // 던전 지역 배경음악

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 트리거에 진입했는지 확인
        if (other.CompareTag("Player"))
        {
            if (gameObject.name == "ForestArea")
            {
                PlayForestBGM();
            }
            else if (gameObject.name == "DungeonArea")
            {
                PlayDungeonBGM();
            }
        }
    }

    private void PlayForestBGM()
    {
        if (!forestBGM.isPlaying)
        {
            dungeonBGM.Stop();
            forestBGM.Play();
        }
    }

    private void PlayDungeonBGM()
    {
        if (!dungeonBGM.isPlaying)
        {
            forestBGM.Stop();
            dungeonBGM.Play();
        }
    }
}
