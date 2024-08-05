using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class MakingHostUI : MonoBehaviour
{
    public TMP_InputField hostNameInput;
    public TMP_InputField hostPasswordInput;
    [SerializeField]
    public List<Button> maxPlayerCountButtons;
    public Button createHostButton;
    private HostController hostController;
    private int maxPlayerCount = 1;

    void Start()
    {
        hostController = GetComponent<HostController>();
        createHostButton.onClick.AddListener(OnCreateHostButtonClick);
        for (int i = 0; i < maxPlayerCountButtons.Count; i++)
        {
            int index = i;
            maxPlayerCountButtons[i].onClick.AddListener(() => UpdateMaxPlayerCount(index + 1));
        }
    }

    public void UpdateMaxPlayerCount(int count)
    {
        maxPlayerCount = count;

        for (int i = 0; i < maxPlayerCountButtons.Count; i++)
        {
            if (i == count - 1)
            {
                maxPlayerCountButtons[i].image.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                maxPlayerCountButtons[i].image.color = new Color(1f, 1f, 1f, 0f);
            }
        }
    }

    void OnCreateHostButtonClick()
    {
        string name = hostNameInput.text;
        string password = hostPasswordInput.text;

        hostController.CreateHost(name, password, maxPlayerCount);
    }
}