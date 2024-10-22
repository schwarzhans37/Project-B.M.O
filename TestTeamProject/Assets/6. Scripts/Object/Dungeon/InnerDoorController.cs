using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerDoorController : InteractableObject
{
    protected override void OnValidate()
    {
        base.OnValidate();
        
        guideText = "문 열기 : [E]";
        holdTime = 0.5f;
        isInteractable = true;
    }

    public override IEnumerator InteractWithObject(GameObject player)
    {
        Debug.Log("InnerDoorController.InteractWithObject()");
        AudioSource.PlayClipAtPoint(soundEffect, transform.position);
        yield return null;
    }
}
