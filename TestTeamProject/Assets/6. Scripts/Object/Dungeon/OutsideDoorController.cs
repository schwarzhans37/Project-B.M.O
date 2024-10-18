using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideDoorController : InteractableObject
{
    protected override void OnValidate()
    {
        base.OnValidate();
        
        guideText = "들어가기 : [V]";
        holdTime = 1.0f;
        isInteractable = true;
    }

    public override void InteractWithObject(GameObject player)
    {
        Debug.Log("OutsideDoorController.InteractWithObject()");
        AudioSource.PlayClipAtPoint(soundEffect, transform.position);
    }
}
