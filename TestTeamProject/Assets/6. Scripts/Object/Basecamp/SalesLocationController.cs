using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalesLocationController : InteractableObject
{
    protected override void OnValidate()
    {
        base.OnValidate();
        
        objectName = "Sales Location";
        guideText = "판매하기 : [F]";
        holdTime = 3f;
    }

    public override void InteractWithObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("ItemObject"))
            {
                Destroy(collider.gameObject);
            }
        }
    }
}
