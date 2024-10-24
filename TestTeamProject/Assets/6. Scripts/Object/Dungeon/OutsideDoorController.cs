using System.Collections;
using System.Collections.Generic;
using Mirror;
using Sydewa;
using UnityEngine;

public class OutsideDoorController : InteractableObject
{
    public Transform teleportPoint;

    protected override void OnValidate()
    {
        base.OnValidate();
        
        holdTime = 1.0f;
        isInteractable = true;
    }

    public override IEnumerator InteractWithObject(GameObject player)
    {
        MoveToDoor(player.GetComponent<NetworkIdentity>().connectionToClient, player);
        OnSoundEffect();
        yield return null;
    }

    [TargetRpc]
    void MoveToDoor(NetworkConnectionToClient target, GameObject player)
    {
        player.transform.position = teleportPoint.position + teleportPoint.forward;
        player.transform.rotation = Quaternion.LookRotation(teleportPoint.forward);
        GameObject.Find("GameDataManager").GetComponent<LightingManager>().SunDirectionalLight.enabled =
            !GameObject.Find("GameDataManager").GetComponent<LightingManager>().SunDirectionalLight.enabled;
    }

    [ClientRpc]
    void OnSoundEffect()
    {
        AudioSource.PlayClipAtPoint(soundEffect, transform.position);
        AudioSource.PlayClipAtPoint(soundEffect, teleportPoint.position);
    }
}
