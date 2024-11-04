using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class EnvironmentalDamage : NetworkBehaviour
{
    public AudioClip environmentalSound;
    public int damage;
    public bool isEnter;
    public bool isStay;
    public bool isExit;
    private float lastTime = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (!isEnter
            || !other.CompareTag("Player"))
            return;

        if (NetworkClient.connection.identity != other.GetComponent<NetworkIdentity>())
            return;

        other.GetComponent<PlayerDataController>().CmdChangeHp(-damage);

        if (environmentalSound != null)
            AudioSource.PlayClipAtPoint(environmentalSound, other.transform.position);
    }

    void OnTriggerStay(Collider other)
    {
        if (!isStay
            || !other.CompareTag("Player"))
            return;

        if (NetworkClient.connection.identity != other.GetComponent<NetworkIdentity>())
            return;
        
        if (Time.time - lastTime < 1f)
            return;

        lastTime = Time.time;
        other.GetComponent<PlayerDataController>().CmdChangeHp(-damage);

        if (environmentalSound != null)
            AudioSource.PlayClipAtPoint(environmentalSound, other.transform.position);
    }

    void OnTriggerExit(Collider other)
    {
        if (!isExit
            || !other.CompareTag("Player"))
            return;

        if (NetworkClient.connection.identity != other.GetComponent<NetworkIdentity>())
            return;

        other.GetComponent<PlayerDataController>().CmdChangeHp(-damage);

        if (environmentalSound != null)
            AudioSource.PlayClipAtPoint(environmentalSound, other.transform.position);
    }

}
