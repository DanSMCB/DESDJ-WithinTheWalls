using Alteruna;
using UnityEngine;

public class Door : AttributesSync
{
    [SynchronizableField]
    public bool isOpen = false;
    [SynchronizableField]
    public bool isLocked = false;

    public float openAngle = 90f;
    public float speed = 3f;

    public string requiredItem;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private InventoryManager inventory;


    void Start()
    {
        inventory = FindFirstObjectByType<InventoryManager>();
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    public void Interact()
    {
        if (isLocked)
        {
            if (requiredItem != null)
            {
                if (inventory.HasItem(requiredItem))
                {
                    inventory.RemoveItem(requiredItem);
                    Unlock();
                    ToggleDoor();
                    return;
                }
            }
            Debug.Log("A porta está trancada.");
            return;
        }

        ToggleDoor();
    }

    [SynchronizableMethod]
    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }

    void Update()
    {
        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * speed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * speed);
        }
    }

    public void Unlock()
    {
        Debug.Log("Porta destrancada.");
        isLocked = false;
    }

    public void Lock()
    {
        Debug.Log("Porta trancada.");
        isLocked = true;
    }
}
