using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakingHostController : MonoBehaviour
{
    public async void CreateHost(string name, string password)
    {
        var manager = BMORoomManager.singleton;
        manager.StartHost();
    }
}
