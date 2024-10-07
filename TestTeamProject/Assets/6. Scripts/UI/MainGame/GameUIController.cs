using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameUIController : MonoBehaviour
{
    public static bool IsPaused { get; set; } = false;

    public GameObject menuUI;
    public AudioClip click;
    private AudioSource audioSource;

    void Start()
    {
        if (menuUI == null)
        {
            menuUI = GameObject.Find("MenuUI");
        }

        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
            menuUI.SetActive(!menuUI.activeSelf);

            audioSource.PlayOneShot(click);
        }
    }
}
