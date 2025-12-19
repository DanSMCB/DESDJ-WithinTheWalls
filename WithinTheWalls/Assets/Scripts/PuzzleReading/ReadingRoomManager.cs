using UnityEngine;

public class ReadingRoomManager : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject doorObj;
    public string requiredKeyName;

    private bool unlocked = false;
    private Door door;
    void Awake()
    {
        door = doorObj.GetComponent<Door>();
        if (door == null)
            Debug.LogError("KeyDoorManager: Porta não atribuída.");

        door.isLocked = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (unlocked)
            return;

        // Só reage a jogadores
        if (!other.CompareTag("Avatar"))
            return;

        // Só o jogador LOCAL pode destrancar
        Alteruna.Avatar avatar = other.GetComponent<Alteruna.Avatar>();
        if (avatar == null || !avatar.IsMe)
            return;

        InventoryManager inventory = FindFirstObjectByType<InventoryManager>();
        ;
        if (inventory == null)
            return;

        if (inventory.HasItem(requiredKeyName))
        {
            door.Unlock();
            unlocked = true;
        }
    }
}