using UnityEngine;

public class CreateHostController : MonoBehaviour
{
    public CreateHostView createHostView;

    void Start()
    {
        createHostView.OnCreateHostAttempt += HandleCreateHostAttempt;
    }

    void HandleCreateHostAttempt(string name, string password, int maxPlayerCount)
    {
        
    }
}
