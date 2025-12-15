using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    private bool menuActivated = false;
    public ItemSlot[] itemSlot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    public void ToggleInventory()
    {
        if (menuActivated)
        {
            InventoryMenu.SetActive(false);
            menuActivated = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            InventoryMenu.SetActive(true);
            menuActivated = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite);
                return;
            }
        }
    }

    public void RemoveItem(string itemName)
    {
        foreach (ItemSlot slot in itemSlot)
        {
            if (slot.isFull && slot.itemName == itemName)
            {
                slot.isFull = false;
                slot.itemName = "";
                slot.quantity = 0;
                slot.itemSprite = null;
                slot.itemImage.enabled = false;
                
                slot.quantityText.text = "";
                slot.quantityText.enabled = false;
                return;
            }
        }
    }

    public bool HasItem(string itemName)
    {
        foreach (ItemSlot slot in itemSlot)
        {
            if (slot.isFull && slot.itemName == itemName)
                return true;
        }
        return false;
    }
}