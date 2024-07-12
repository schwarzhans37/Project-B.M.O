using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakingHostUI : MonoBehaviour
{
    public TMP_InputField hostNameInput;
    public TMP_InputField hostPasswordInput;
    public Button createHostButton;
    private MakingHostController makingHostController;

    void Start()
    {
        makingHostController = GetComponent<MakingHostController>();
        createHostButton.onClick.AddListener(OnCreateHostButtonClick);
    }

    void OnCreateHostButtonClick()
    {
        string name = hostNameInput.text;
        string password = hostPasswordInput.text;

        makingHostController.CreateHost(name, password);
    }
}
