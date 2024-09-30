using UnityEngine;

[RequireComponent(typeof(AudioSource))] // AudioSource ������Ʈ �߰�
public class GameUIController : MonoBehaviour
{
    public static bool IsPaused { get; set; } = false;

    public GameObject menuUI;
    public AudioClip click;
    private AudioSource audioSource; // AudioSource ���� �߰�

    void Start()
    {
        if (menuUI == null)
        {
            menuUI = GameObject.Find("MenuUI");
        }

        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        audioSource = GetComponent<AudioSource>(); // AudioSource ������Ʈ �ʱ�ȭ
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
            menuUI.SetActive(!menuUI.activeSelf);

            audioSource.PlayOneShot(click); // Ŭ�� �Ҹ� ���
        }
    }
}
