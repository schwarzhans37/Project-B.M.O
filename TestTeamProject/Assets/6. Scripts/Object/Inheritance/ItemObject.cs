using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
public class ItemObject : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnItemNameChanged))]
    public string itemName; // 아이템 이름

    [SyncVar(hook = nameof(OnItemPriceChanged))]
    public int itemPrice; // 아이템 가격
    public Sprite ItemImage;
    
    protected override void OnValidate()
    {
        base.OnValidate();

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        GetComponent<NetworkTransformReliable>().syncPosition = true;
        GetComponent<NetworkTransformReliable>().syncRotation = false;
    }
    public virtual void OnItemNameChanged(string oldName, string newName) { }
    public virtual void OnItemPriceChanged(int oldPrice, int newPrice) { }
}
