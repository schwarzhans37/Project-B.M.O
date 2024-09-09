using UnityEngine;
using UnityEngine.UI;

public class GameUIView : MonoBehaviour
{
    public GameObject menuUI;

    public void SetActiveMenuUI()
    {
        menuUI.SetActive(!menuUI.activeSelf);
    }

    public void ShowMessage(string message)
    {
        Debug.Log($"message: {message}");
    }
}
