using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideDoorController : InteractableObject
{
    protected override void OnValidate()
    {
        base.OnValidate();
        
        objectName = "OutsideDoor";
        guideText = "들어가기 : [V]";
        holdTime = 1.0f;
    }

    public override void InteractWithObject(GameObject player)
    {
        Debug.Log("OutsideDoorController.InteractWithObject()");
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().RemoveEnemies();
        AudioSource.PlayClipAtPoint(soundEffect, transform.position);
    }
}
