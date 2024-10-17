using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingPointContoller : InteractableObject
{
    public float radius;

    protected override void OnValidate()
    {
        base.OnValidate();
        
        guideText = "수면하기 : [V]";
        holdTime = 3f;
    }

    public override void InteractWithObject(GameObject player)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
