using Mirror;

public class InteractionObject : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnObjectNameChanged))]
    public string objectName; // 상호작용 오브젝트의 이름
    
    [SyncVar(hook = nameof(OnGuideTextChanged))]
    public string guideText; // 상호작용 UI에 표시할 텍스트
    
    [SyncVar(hook = nameof(OnHoldTimeChanged))]
    public float holdTime; // 누르고 있어야 하는 시간


    [Command]
    public void CmdSetObjectName(string name)
    {
        objectName = name;
    }

    [Command]
    public void CmdSetGuideText(string text)
    {
        guideText = text;
    }

    [Command]
    public void CmdSetHoldTime(float time)
    {
        holdTime = time;
    }

    public virtual void InteractWithObject() {}
    public virtual void OnObjectNameChanged(string oldName, string newName) {}
    public virtual void OnGuideTextChanged(string oldText, string newText) {}
    public virtual void OnHoldTimeChanged(float oldTime, float newTime) {}
}
