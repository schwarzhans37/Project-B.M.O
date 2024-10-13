using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerDoorController : InteractableObject
{
    int level = 0;
    protected override void OnValidate()
    {
        base.OnValidate();
        
        objectName = "InnerDoor";
        guideText = "문 열기 : [F]";
        holdTime = 0.5f;
    }

    public override void InteractWithObject(GameObject player)
    {
        Debug.Log("InnerDoorController.InteractWithObject()");
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().SpawnEnemies(level);
        level++;
    }
}
