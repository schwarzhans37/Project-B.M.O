using System.Collections;
using Mirror;
using Sydewa;
using UnityEngine;

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

        GameObject.Find("GameDataManager").GetComponent<LightingManager>().SunDirectionalLight.enabled =
            !GameObject.Find("GameDataManager").GetComponent<LightingManager>().SunDirectionalLight.enabled;

        RenderSettings.skybox = RenderSettings.skybox != GameObject.Find("GameDataManager").GetComponent<LightingManager>().dungeonSkyboxMat ?
            GameObject.Find("GameDataManager").GetComponent<LightingManager>().dungeonSkyboxMat : GameObject.Find("GameDataManager").GetComponent<LightingManager>().defultSkyboxMat;

        DynamicGI.UpdateEnvironment();
    }

    [ClientRpc]
    void OnSoundEffect()
    {
        AudioSource.PlayClipAtPoint(soundEffect, transform.position);
        AudioSource.PlayClipAtPoint(soundEffect, teleportPoint.position);
    }
}
