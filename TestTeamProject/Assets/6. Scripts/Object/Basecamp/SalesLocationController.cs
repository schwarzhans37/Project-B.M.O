using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius)
            .Where(x => x.CompareTag("ItemObject"))
            .ToArray();

        if (colliders.Length == 0)
            return;

        particle.Play();
        AudioSource.PlayClipAtPoint(soundEffect, transform.position);

        foreach (Collider collider in colliders)
        {
            // 아이템을 판매
            GameObject.Find("GameDataManager").GetComponent<GameDataController>().AddMoney(collider.gameObject.GetComponent<ItemObject>().itemPrice);
            Destroy(collider.gameObject);
            collider.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
