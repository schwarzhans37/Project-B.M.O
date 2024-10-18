using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class InteractableObject : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnGuideTextChanged))]
    public string guideText; // 상호작용 UI에 표시할 텍스트
    
    [SyncVar(hook = nameof(OnHoldTimeChanged))]
    public float holdTime; // 누르고 있어야 하는 시간

    [SyncVar(hook = nameof(OnInteractableChanged))]
    public bool isInteractable; // 상호작용 가능 여부

    public AudioClip soundEffect;

    public virtual void InteractWithObject(GameObject player) {}
    public virtual void OnGuideTextChanged(string oldText, string newText) {}
    public virtual void OnHoldTimeChanged(float oldTime, float newTime) {}
    public virtual void OnInteractableChanged(bool oldInteractable, bool newInteractable) {}
}
