using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerDoorController : InteractionObject
{
    private void OnValidate()
    {
        objectName = "InnerDoor";
        guideText = "들어가기 [F]";
        holdTime = 0.5f;
    }

    public override void InteractWithObject()
    {
        Debug.Log("InnerDoorController.InteractWithObject()");
    }
}
