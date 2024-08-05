using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public Transform MF_to_TR, TR_to_MF, MF_to_TR_Exit, TR_to_MF_Exit;
    GameObject Player;

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        Teleport();
    }

    private void Teleport()
    {

    }
}
