using System.Collections;
using Mirror;
using Sydewa;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class OutsideDoorController : InteractableObject
{
    public Transform teleportPoint;
    public Vector3 lookDirection;

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
        player.transform.position = teleportPoint.position;
        player.transform.rotation = Quaternion.LookRotation(teleportPoint.GetComponent<OutsideDoorController>().lookDirection);

        // GameObject.Find("GameDataManager").GetComponent<LightingManager>().SunDirectionalLight.enabled =
        //     !GameObject.Find("GameDataManager").GetComponent<LightingManager>().SunDirectionalLight.enabled;

        bool isDayCycleOn = !GameObject.Find("GameDataManager").GetComponent<LightingManager>().IsDayCycleOn;

        GameObject.Find("GameDataManager").GetComponent<LightingManager>().IsDayCycleOn = isDayCycleOn;

        GameObject.Find("GameDataManager").GetComponent<LightingManager>().TimeOfDay =
            isDayCycleOn ? GameObject.Find("GameDataManager").GetComponent<GameDataController>().time / 60f : 0;

        GameObject.Find("GameDataManager").GetComponent<LightingManager>().UpdateLighting(
            GameObject.Find("GameDataManager").GetComponent<LightingManager>().TimeOfDay / 24f);
    }

    [ClientRpc]
    void OnSoundEffect()
    {
        AudioSource.PlayClipAtPoint(soundEffect, transform.position);
        AudioSource.PlayClipAtPoint(soundEffect, teleportPoint.position);
    }
}
