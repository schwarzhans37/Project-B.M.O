using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideDoorController : InteractableObject
{
    protected override void OnValidate()
    {
        base.OnValidate();
        
        objectName = "OutsideDoor";
        guideText = "들어가기 : [F]";
        holdTime = 1.0f;
    }

    public override void InteractWithObject()
    {
        Debug.Log("OutsideDoorController.InteractWithObject()");
    }
}
