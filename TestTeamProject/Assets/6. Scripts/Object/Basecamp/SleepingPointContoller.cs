using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

        int deathedPlayersCount = 0;
        foreach (GameObject playerObject in players)
        {
            if (playerObject == null)
            {
                players.Remove(playerObject);
                break;
            }

            if (playerObject.GetComponent<PlayerDataController>().isDead)
                deathedPlayersCount++;
            
            playerObject.GetComponent<PlayerDataController>().hp = 1000;
        }

        if (colliders.Length != players.Count - deathedPlayersCount)
        {
            StartCoroutine(ShowMessage("모든 플레이어가 수면해야 합니다."));
            yield break;
        }

        StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().AdvanceDay(players));
        yield return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
