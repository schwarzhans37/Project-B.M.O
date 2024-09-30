using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
[RequireComponent(typeof(Rigidbody))]
public class ItemObject : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnItemNameChanged))]
    public string itemName; // 아이템 이름

    [SyncVar(hook = nameof(OnItemPriceChanged))]
    public int itemPrice; // 아이템 가격

    public virtual void OnItemNameChanged(string oldName, string newName) { }
    public virtual void OnItemPriceChanged(int oldPrice, int newPrice) { }
}
