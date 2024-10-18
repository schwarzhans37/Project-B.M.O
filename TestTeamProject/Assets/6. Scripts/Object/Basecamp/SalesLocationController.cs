using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalesLocationController : InteractableObject
{
    public ParticleSystem particle;
    protected override void OnValidate()
    {
        base.OnValidate();
        
        objectName = "Sales Location";
        guideText = "판매하기 : [F]";
        holdTime = 3f;
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    public override void InteractWithObject(GameObject player)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);

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

        GameObject.Find("EnemySpawner").GetComponent<TestSpawner>().SpawnEnemies();
    }
}
