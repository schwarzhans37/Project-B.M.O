using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class OutsideDoorController : InteractableObject
{
    public Transform teleportPoint;

    protected override void OnValidate()
    {
        base.OnValidate();
        
        guideText = "들어가기 : [V]";
        holdTime = 1.0f;
        isInteractable = true;
    }

    public override void InteractWithObject(GameObject player)
    {
        MoveToDoor(player.GetComponent<NetworkIdentity>().connectionToClient, player);
        OnSoundEffect();
    }

    [TargetRpc]
    void MoveToDoor(NetworkConnectionToClient target, GameObject player)
    {
        player.transform.position = teleportPoint.position + teleportPoint.forward * 2;
    }

    [ClientRpc]
    void OnSoundEffect()
    {
        AudioSource.PlayClipAtPoint(soundEffect, transform.position);
        AudioSource.PlayClipAtPoint(soundEffect, teleportPoint.position);
    }
}
