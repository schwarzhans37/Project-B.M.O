using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class WagonController : InteractableObject
{
    public Transform wagonPoint;
    public float radius;

    protected override void OnValidate()
    {
        base.OnValidate();
        
        guideText = "출발하기 : [V]";
        holdTime = 3f;
    }

    public override void InteractWithObject(GameObject player)
    {
        Collider[] items = Physics.OverlapSphere(transform.position, radius)
        .Where(collider => collider.CompareTag("ItemObject")).ToArray();

        Collider[] players = Physics.OverlapSphere(transform.position, radius)
        .Where(collider => collider.CompareTag("Player")).ToArray();

        // 플레이어와 아이템을 모두 웨건으로 이동
        foreach (Collider item in items)
        {
            item.transform.SetParent(transform, true);
            Vector3 localPosition = item.transform.localPosition;
            Quaternion localRotation = item.transform.localRotation;
            item.transform.SetParent(wagonPoint, true);
            item.transform.localPosition = localPosition;
            item.transform.localRotation = localRotation;
            item.transform.SetParent(null, true);
        }

        foreach (Collider playerCollider in players)
            MoveToWagon(playerCollider.GetComponent<NetworkIdentity>().connectionToClient, playerCollider.gameObject);
        
    }

    [TargetRpc]
    void MoveToWagon(NetworkConnectionToClient target, GameObject player)
    {
        player.transform.SetParent(transform, true);
        Vector3 localPosition = player.transform.localPosition;
        Quaternion localRotation = player.transform.localRotation;
        player.transform.SetParent(wagonPoint, true);
        player.transform.localPosition = localPosition;
        player.transform.localRotation = localRotation;
        player.transform.SetParent(null, true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
