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
        
        holdTime = 3f;
    }

    public override IEnumerator InteractWithObject(GameObject player)
    {
        Collider[] items = Physics.OverlapSphere(transform.position, radius)
        .Where(collider => collider.CompareTag("ItemObject")).ToArray();

        List<GameObject> boardedPlayers = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Player"))
            .Select(collider => collider.gameObject).ToList();

        if (boardedPlayers.Count != NetworkServer.connections.Count && gameObject.name == "BasecampWagon")
        {
            StartCoroutine(ShowMessage("모든 플레이어가 웨건에 탑승해야 합니다."));
            yield break;
        }

        if (gameObject.name == "ForestWagon")
        {
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (conn.identity == null)
                    continue;

                if (!boardedPlayers.Contains(conn.identity.gameObject))
                    conn.identity.GetComponent<PlayerDataController>().isDead = true;
            }
        }

        GameObject.Find("GameDataManager").GetComponent<GameDataController>().SetIsInteractionLocked(true);
        GameObject.Find("GameDataManager").GetComponent<GameDataController>().SetIsMoveLocked(true);

        GameObject.Find("GameDataManager").GetComponent<GameDataView>().FadeOutBlackScreen();
        yield return StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().ConfirmClientsComplete(10f));

        // 아이템을 모두 웨건으로 이동
        foreach (Collider item in items)
            item.transform.position += wagonPoint.position - transform.position;

        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null)
                continue;

            if (boardedPlayers.Contains(conn.identity.gameObject))
            {
                int count = 0;
                Vector3 targetPosition = conn.identity.transform.position + wagonPoint.position - transform.position;
                while (Vector3.Distance(conn.identity.transform.position, wagonPoint.position) > radius * 2
                    && conn.identity != null
                    && count < 10)
                {
                    MoveToWagon(conn, conn.identity.gameObject, targetPosition);
                    conn.identity.GetComponent<PlayerDataController>().CmdReportTaskWorking();
                    yield return StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().ConfirmClientsComplete(10f));
                    yield return new WaitForSeconds(0.1f);
                    count++;
                }
            }
        }

        if (gameObject.name == "BasecampWagon")
            StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().StartGame(boardedPlayers));
        else
            StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().EndGame());
    }

    [TargetRpc]
    void MoveToWagon(NetworkConnectionToClient target, GameObject player, Vector3 targetPosition)
    {
        player.transform.position = targetPosition;
        AudioSource.PlayClipAtPoint(soundEffect, wagonPoint.position, 0.2f);
        NetworkClient.localPlayer.GetComponent<PlayerDataController>().CmdReportTaskComplete();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
