using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DeathCamera : MonoBehaviour
{
    public TMP_Text viewingText;
    public List<Transform> alivePlayers = new List<Transform>();
    public Transform targetPlayer;
    private int currentPlayerIndex;


    void Update()
    {
        if (targetPlayer == null
            || targetPlayer.GetComponent<PlayerDataController>().isDead)
            SetUp();

        if (Input.GetMouseButtonDown(0)) // 좌클릭 감지
        {
            MoveToNextPlayer();
        }
    }

    void OnEnable()
    {
        viewingText.text = $"관전중 : {targetPlayer.GetComponent<PlayerDataController>().nickname}";
        transform.SetParent(targetPlayer);
        transform.localPosition = new Vector3(0, 2, -1);
        transform.localRotation = Quaternion.identity;
        transform.Rotate(10, 0, 0);
    }

    private void MoveToNextPlayer()
    {
        if (alivePlayers.Count == 0)
            return; // 살아있는 플레이어가 없는 경우

        currentPlayerIndex = (currentPlayerIndex + 1) % alivePlayers.Count; // 다음 플레이어로 이동
        targetPlayer = alivePlayers[currentPlayerIndex];
        viewingText.text = $"관전중 : {targetPlayer.GetComponent<PlayerDataController>().nickname}";
        transform.SetParent(targetPlayer);
        transform.localPosition = new Vector3(0, 2, -1);
        transform.localRotation = Quaternion.identity;
        transform.Rotate(10, 0, 0);
    }

    void SetUp()
    {
        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();

        alivePlayers.Clear();
        foreach (GameObject player in players)
        {
            if (!player.GetComponent<PlayerDataController>().isDead)
                alivePlayers.Add(player.transform);
        }

        if (alivePlayers.Count == 0)
            transform.SetParent(null, true);
        else
            MoveToNextPlayer();
    }
}
