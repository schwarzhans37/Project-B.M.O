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
        if (!isEnter || !isServer)
            return;
        
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerDataController>().ChangeHp(-damage);
            PlaySound(other.transform.position);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!isStay || !isServer)
            return;
        
        if (Time.time - lastTime < 1f)
            return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerDataController>().ChangeHp(-damage);
            lastTime = Time.time;
            PlaySound(other.transform.position);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isExit || !isServer)
            return;
        
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerDataController>().ChangeHp(-damage);
            PlaySound(other.transform.position);
        }
    }

    [ClientRpc]
    void PlaySound(Vector3 other)
    {
        if (environmentalSound == null)
            return;
            
        AudioSource.PlayClipAtPoint(environmentalSound, other);
    }
}
