using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalesLocationController : InteractableObject
{
    public float radius;

    public ParticleSystem particle;
    protected override void OnValidate()
    {
        base.OnValidate();
        
        guideText = "판매하기 : [V]";
        holdTime = 3f;
        isInteractable = true;
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
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
                collider.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                particle.Play();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
