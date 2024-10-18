using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius)
        .Where(collider => collider.CompareTag("Player") || collider.CompareTag("ItemObject")).ToArray();

        // 플레이어와 아이템을 모두 웨건으로 이동
        foreach (Collider collider in colliders)
        {
            collider.transform.SetParent(transform, true);
            Vector3 localPosition = collider.transform.localPosition;
            Quaternion localRotation = collider.transform.localRotation;
            collider.transform.SetParent(wagonPoint, true);
            collider.transform.localPosition = localPosition;
            collider.transform.localRotation = localRotation;
            collider.transform.SetParent(null, true);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
