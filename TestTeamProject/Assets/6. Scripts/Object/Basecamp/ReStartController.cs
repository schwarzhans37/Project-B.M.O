using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ReStartController : InteractableObject
{
    protected override void OnValidate()
    {
        base.OnValidate();
        
        guideText = "다시하기 : [E]";
        holdTime = 3f;
        isInteractable = true;
    }

    public override IEnumerator InteractWithObject(GameObject player)
    {
        GameObject.Find("GameDataManager").GetComponent<GameDataController>().SetIsInteractionLocked(true);
        GameObject.Find("GameDataManager").GetComponent<GameDataController>().SetIsMoveLocked(true);

        GameObject.Find("GameDataManager").GetComponent<GameDataView>().FadeOutBlackScreen();
        yield return StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().ConfirmClientsComplete(10f));

        int index = 0;
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null)
                continue;

            GameObject.Find("GameDataManager").GetComponent<GameDataController>().MoveToSpawnPoint(conn, conn.identity.gameObject, index);
            conn.identity.GetComponent<PlayerDataController>().CmdReportTaskWorking();
            yield return StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().ConfirmClientsComplete(10f));

            index++;
        }

        GameObject.Find("GameDataManager").GetComponent<GameDataController>().SetIsMoveLocked(false);

        GameObject.Find("GameDataManager").GetComponent<GameDataView>().FadeOutBlackScreen();
        yield return StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().ConfirmClientsComplete(10f));
        StartCoroutine(GameObject.Find("GameDataManager").GetComponent<GameDataController>().StartSetting());
    }
}
