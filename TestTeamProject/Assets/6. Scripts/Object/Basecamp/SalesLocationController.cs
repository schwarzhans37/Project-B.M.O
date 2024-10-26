using System.Collections;
using System.Linq;
using Mirror;
using UnityEngine;

public class SalesLocationController : InteractableObject
{
    public float radius;

    public ParticleSystem particle;
    protected override void OnValidate()
    {
        base.OnValidate();
        
        guideText = "판매하기 : [E]";
        holdTime = 3f;
        isInteractable = true;
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    public override IEnumerator InteractWithObject(GameObject player)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius)
            .Where(x => x.CompareTag("ItemObject"))
            .ToArray();

        if (colliders.Length == 0)
            yield break;
        
        isInteractable = false;
        PlayEffect();
        
        yield return new WaitForSeconds(particle.main.duration);

        int totalMoney = 0;
        foreach (Collider collider in colliders)
        {
            // 아이템을 판매하고 돈을 얻음
            totalMoney += collider.GetComponent<ItemObject>().itemPrice;
            NetworkServer.Destroy(collider.gameObject);
        }

        GameObject.Find("GameDataManager").GetComponent<GameDataController>().money += totalMoney;

        isInteractable = true;
    }

    [ClientRpc]
    public override void PlayEffect()
    {
        base.PlayEffect();
        particle.Play();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
