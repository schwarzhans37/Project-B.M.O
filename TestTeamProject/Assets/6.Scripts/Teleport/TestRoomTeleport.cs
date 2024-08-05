using UnityEngine;

public class TestRoomTeleport : MonoBehaviour
{
    public GameObject portal;
    public GameObject portalEnd;

    private void Start()
    {
        // 각 포탈 오브젝트가 제대로 할당되었는지 확인합니다.
        if (portal == null || portalEnd == null)
        {
            Debug.LogError("Portals not found! Make sure you have GameObjects assigned to 'portal' and 'portalEnd'.");
        }
        else
        {
            Debug.Log($"PortalEnd position: {portalEnd.transform.position}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Triggered by {other.gameObject.name}");

        // Player 태그를 가진 오브젝트와 충돌하면 텔레포트합니다.
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the portal.");

            if (gameObject == portal)
            {
                Debug.Log("Teleporting player to portalEnd position.");
                Teleport(other.gameObject, portalEnd.transform.position);
            }
        }
    }

    private void Teleport(GameObject player, Vector3 destination)
    {
        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = destination;
            controller.enabled = true;
            Debug.Log($"Player teleported to {destination} using CharacterController");
        }
        else
        {
            player.transform.position = destination;
            Debug.Log($"Player teleported to {destination} using transform.position");
        }
    }
}
