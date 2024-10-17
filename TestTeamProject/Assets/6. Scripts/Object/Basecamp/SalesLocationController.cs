using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalesLocationController : InteractableObject
{
    public float radius;

    protected override void OnValidate()
    {
        base.OnValidate();
        
        guideText = "판매하기 : [V]";
        holdTime = 3f;
        isInteractable = true;
    }

    public override void InteractWithObject(GameObject player)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("ItemObject"))
            {
                // 아이템을 판매
                GameObject.Find("GameDataManager").GetComponent<GameDataController>().AddMoney(collider.gameObject.GetComponent<ItemObject>().itemPrice);
                Destroy(collider.gameObject);
                AudioSource.PlayClipAtPoint(soundEffect, transform.position);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
