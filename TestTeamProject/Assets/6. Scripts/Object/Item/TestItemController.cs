using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItemController : ItemObject
{
    protected override void OnValidate()
    {
        base.OnValidate();
        
        itemName = "Test Item";
        itemPrice = 100;
    }
}
