using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : NetworkBehaviour
{
    public ItemObject Item; // 아이템
    public Image ItemImage; //아이템의 이미지
    
    private void SetColor(float _alpha)
    {
        Color color = ItemImage.color;
        color.a = _alpha;
        ItemImage.color = color;
    }
    
    public void AddItem(ItemObject _item)
    {
        Item = _item;
        
        SetColor(1);
    }
}
