using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    private bool menuActivated;
    public PlayerController player;
    public ItemSlot[] itemSlot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && menuActivated) { 
            InventoryMenu.SetActive(false);
            menuActivated = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            player.canLook = true;
        }
        else if (Input.GetButtonDown("Inventory") && !menuActivated)
        {
            InventoryMenu.SetActive(true);
            menuActivated = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            player.canLook = false;
        }
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite) {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false) {
                itemSlot[i].AddItem(itemName, quantity, itemSprite);
                return;
            }
        }
    }
}
