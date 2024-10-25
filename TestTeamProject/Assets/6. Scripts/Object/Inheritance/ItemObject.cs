using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
public class ItemObject : NetworkBehaviour
{    
    [SyncVar]
    public int itemPrice; // 아이템 가격
    public string itemName; // 아이템 이름
    public int itemMinPrice; // 아이템 가격 범위
    public int itemMaxPrice; // 아이템 가격 범위
    public Sprite ItemImage;
}
