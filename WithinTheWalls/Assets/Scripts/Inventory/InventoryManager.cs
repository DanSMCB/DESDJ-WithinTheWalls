using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    public ItemSlot[] itemSlot;

    private bool menuActivated;

    private PlayerController player;

    void Start()
    {
        // Começa a procurar o jogador até existir
        StartCoroutine(WaitForPlayer());
    }

    private System.Collections.IEnumerator WaitForPlayer()
    {
        while (player == null)
        {
            player = FindObjectOfType<PlayerController>();
            yield return null; // Espera 1 frame
        }

        // Quando encontrar, liga o evento
        player.input.Player.Inventory.performed += OnInventory;

        Debug.Log("InventoryManager: Player encontrado e input ligado!");
    }

    private void OnDestroy()
    {
        if (player != null)
            player.input.Player.Inventory.performed -= OnInventory;
    }

    private void OnInventory(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        menuActivated = !menuActivated;
        InventoryMenu.SetActive(menuActivated);

        if (menuActivated)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            player.canLook = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            player.canLook = true;
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
}
