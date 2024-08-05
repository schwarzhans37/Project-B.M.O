using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class weepingAngel : MonoBehaviour
{
    public NavMeshAgent ai;
    private Transform player;
    private Vector3 dest;
    private Camera playerCam;
    public float aiSpeed;

    void Start()
    {

    }

    void Update()
    {
        Debug.Log("Start method called."); // Start 메서드 호출 확인

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            this.player = playerObj.transform;
            Debug.Log("Player found: " + player); // 플레이어 찾기 확인
        }
        else
        {
            Debug.LogWarning("Player not found."); // 플레이어 찾기 실패
        }

        GameObject camObj = GameObject.FindWithTag("PlayerCam");
        if (camObj != null)
        {
            this.playerCam = camObj.GetComponent<Camera>();
            Debug.Log("PlayerCam found: " + playerCam); // 카메라 찾기 확인
        }
        else
        {
            Debug.LogWarning("PlayerCam not found."); // 카메라 찾기 실패
        }

        // NavMeshAgent 초기 설정
        if (ai != null)
        {
            ai.speed = aiSpeed;
            Debug.Log("NavMeshAgent speed set to: " + aiSpeed); // NavMeshAgent 속도 설정 확인
        }
        else
        {
            Debug.LogWarning("NavMeshAgent not assigned."); // NavMeshAgent 설정 실패
        }
        Debug.Log("Update method called."); // Update 메서드 호출 확인

        if (player == null || playerCam == null || ai == null)
        {
            Debug.LogWarning("Player, PlayerCam, or AI is not set."); // 경고 메시지 출력
            return; // player, playerCam 또는 ai가 설정되지 않은 경우 업데이트를 종료합니다.
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCam);

        if (GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds))
        {
            ai.speed = 0;
            ai.SetDestination(transform.position); // 현재 위치로 설정하여 멈추도록 합니다.
            Debug.Log("AI stopped."); // AI 멈춤 확인
        }
        else
        {
            ai.speed = aiSpeed;
            dest = player.position;
            ai.SetDestination(dest); // 플레이어 위치로 이동합니다.

            // 디버그 메시지 출력
            Debug.Log("AI moving to: " + dest);

            // 경로를 시각적으로 확인하기 위해 라인을 그립니다.
            Debug.DrawLine(transform.position, dest, Color.red);
        }

        // NavMeshAgent 상태를 디버그 메시지로 출력
        Debug.Log("AI velocity: " + ai.velocity);
        Debug.Log("AI remaining distance: " + ai.remainingDistance);
        Debug.Log("AI has path: " + ai.hasPath);
        Debug.Log("AI path pending: " + ai.pathPending);
    }
}
