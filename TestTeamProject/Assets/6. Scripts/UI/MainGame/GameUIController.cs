using UnityEngine;

[RequireComponent(typeof(GameUIView))]
[RequireComponent(typeof(AudioSource))] // AudioSource 컴포넌트 추가
public class GameUIController : MonoBehaviour
{
    public static bool IsPaused { get; set; } = false;

    public GameUIView gameUIView;
    public AudioClip click;
    private AudioSource audioSource; // AudioSource 변수 추가

    void OnValidate()
    {
        if (gameUIView == null)
            gameUIView = GetComponent<GameUIView>();
    }

    void Start()
    {
        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 초기화
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
            gameUIView.SetActiveMenuUI();

            audioSource.PlayOneShot(click); // 클릭 소리 재생
        }
    }
}
