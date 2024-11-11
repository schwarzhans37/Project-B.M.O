using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : NetworkBehaviour
{
    public GameObject inventoryUI;
    public List<bool> isItemActive;
    public List<GameObject> items;
    public int selectedItemIndex;
    [SyncVar]
    public float totalWeight;
    public float weightSlowdownFactor => 1 - (totalWeight / 1200 > 0.5f ? 0.5f : totalWeight / 1200);

    public Sprite normalSprite;
    public Sprite seletedSprite;

    protected override void OnValidate()
    {
        base.OnValidate();
        
        isItemActive = new List<bool>(new bool[6]);
        items = new List<GameObject>(new GameObject[6]);
        this.enabled = false;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // 권한이 있으면 상호작용 컨트롤러를 활성화
        this.enabled = true;
    }

    void Start()
    {
        if (inventoryUI == null)
        {
            inventoryUI = GameObject.Find("Inventory");
        }
    }

    void Update()
    {
        if (!isLocalPlayer
            || GetComponent<PlayerDataController>().isDead)
            return;

        if (GameUIController.IsPaused)
            return;

        for (int i = 0; i < items.Count; i++)
            inventoryUI.transform.GetChild(i).GetComponentInChildren<CanvasGroup>().alpha = isItemActive[i] ? 1 : 0;
        
        if (Input.GetKeyDown(KeyCode.Q) && isItemActive[selectedItemIndex])
        {
            CmdDropItem(selectedItemIndex);
            isItemActive[selectedItemIndex] = false;
        }
        
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollValue) > 0.07f) // 작은 움직임을 무시하는 조건
        {
            if (scrollValue < 0)
            {
                inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = normalSprite;
                selectedItemIndex = (selectedItemIndex + 1) % 6;
                inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = seletedSprite;
            }
            else
            {
                inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = normalSprite;
                selectedItemIndex = (selectedItemIndex + 5) % 6;
                inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = seletedSprite;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = normalSprite;
            selectedItemIndex = 0;
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = seletedSprite;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = normalSprite;
            selectedItemIndex = 1;
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = seletedSprite;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = normalSprite;
            selectedItemIndex = 2;
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = seletedSprite;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = normalSprite;
            selectedItemIndex = 3;
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = seletedSprite;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = normalSprite;
            selectedItemIndex = 4;
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = seletedSprite;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = normalSprite;
            selectedItemIndex = 5;
            inventoryUI.transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite = seletedSprite;
        }
    }

    public void PickUpItem(GameObject item)
    {
        if (items[selectedItemIndex] != null)
            return;

        CmdPickUpItem(item, selectedItemIndex);
        isItemActive[selectedItemIndex] = true;
    }

    [Command]
    public void CmdPickUpItem(GameObject item, int index)
    {
        items[index] = item;

        if (items[index] == null)
            return;

        totalWeight += items[index].GetComponent<ItemObject>().itemPrice;
        RpcItem(false, items[index]);
    }

    [Command]
    public void CmdDropItem(int index)
    {
        GameObject item = items[index];
        items[index] = null;

        if (items[index] != null)
            return;
            
        isItemActive[index] = false;
        totalWeight -= item.GetComponent<ItemObject>().itemPrice;
        item.transform.position = transform.position + transform.up;
        RpcItem(true, item);
        item.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Impulse);
    }

    [ClientRpc]
    public void RpcItem(bool isActive, GameObject item)
    {
        item.GetComponent<Collider>().enabled = isActive;
        item.GetComponent<Rigidbody>().isKinematic = !isActive;
        for (int i = 0; i < item.transform.childCount; i++)
            item.transform.GetChild(i).gameObject.SetActive(isActive);
    }

    public void ClearItems()
    {
        foreach (var item in items)
            if (item != null) 
                NetworkServer.Destroy(item);

        items = new List<GameObject>(new GameObject[6]);
        ClearItemsClient(connectionToClient);
        totalWeight = 0;
    }

    [TargetRpc]
    public void ClearItemsClient(NetworkConnectionToClient conn)
    {
        isItemActive = new List<bool>(new bool[6]);
    }
}
