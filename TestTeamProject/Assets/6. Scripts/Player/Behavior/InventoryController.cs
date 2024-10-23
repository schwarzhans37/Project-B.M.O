using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : NetworkBehaviour
{
    [SyncVar]
    public List<GameObject> items;
    [SyncVar]
    public float totalWeight;
    public float weightSlowdownFactor => 1 - (totalWeight / 600 > 1 ? 1 : totalWeight / 600);
    public GameObject inventoryUI;
    public int selectedItemIndex = 0;

    public Sprite normalSprite;
    public Sprite seletedSprite;

    protected override void OnValidate()
    {
        base.OnValidate();
        
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
            inventoryUI.transform.GetChild(i).GetComponentInChildren<CanvasGroup>().alpha = items[i] != null ? 1 : 0;
        
        if (Input.GetKeyDown(KeyCode.Q) && items[selectedItemIndex] != null)
            CmdDropItem(selectedItemIndex);
        
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollValue) > 0.07f) // 작은 움직임을 무시하는 조건
        {
            if (scrollValue > 0)
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
        {
            CmdDropItem(selectedItemIndex);
        }

        CmdPickUpItem(item, selectedItemIndex);
    }

    [Command]
    public void CmdPickUpItem(GameObject item, int index)
    {
        items[index] = item;
        totalWeight += item.GetComponent<ItemObject>().itemPrice;
        item.SetActive(false);
    }

    [Command]
    public void CmdDropItem(int index)
    {
        GameObject item = items[index];
        items[index] = null;
        totalWeight -= item.GetComponent<ItemObject>().itemPrice;
        item.transform.position = transform.position + transform.up;
        item.SetActive(true);
        item.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Impulse);
    }

    public void ClearItems()
    {
        items = new List<GameObject>(new GameObject[6]);
        totalWeight = 0;
    }
}
