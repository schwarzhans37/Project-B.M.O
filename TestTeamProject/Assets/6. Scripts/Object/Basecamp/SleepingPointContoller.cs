using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class SleepingPointContoller : InteractableObject
{
    public float radius;

    protected override void OnValidate()
    {
        base.OnValidate();
        
        guideText = "수면하기 : [E]";
        holdTime = 3f;
    }

    public override IEnumerator InteractWithObject(GameObject player)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Player"));

        int survivedPlayersCound = 0;
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null)
                continue;
            
            if (!conn.identity.GetComponent<PlayerDataController>().isDead)
                survivedPlayersCound++;
        }

        if (colliders.Length != survivedPlayersCound)
        {
            StartCoroutine(ShowMessage("모든 플레이어가 수면해야 합니다."));
            yield break;
        }

        GameObject.Find("GameDataManager").GetComponent<GameDataController>().SetIsInteractionLocked(true);
        GameObject.Find("GameDataManager").GetComponent<GameDataController>().SetIsMoveLocked(true);

        GameObject.Find("GameDataManager").GetComponent<GameDataView>().FadeOutBlackScreen();
        yield return StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().ConfirmClientsComplete(10f));

        StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().AdvanceDay());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
