using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerDoorController : InteractableObject
{
    protected override void OnValidate()
    {
        base.OnValidate();
        
        objectName = "InnerDoor";
        guideText = "문 열기 : [F]";
        holdTime = 0.5f;
    }

    public override void InteractWithObject()
    {
        Debug.Log("InnerDoorController.InteractWithObject()");
    }
}
