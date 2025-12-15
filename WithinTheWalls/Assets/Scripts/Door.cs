using Alteruna;
using UnityEngine;

public class Door : AttributesSync
{
    [SynchronizableField]
    public bool isOpen = false;
    public float openAngle = 90f;
    public float speed = 3f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    public bool isLocked = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    public void Interact()
    {
        if (isLocked)
        {
            Debug.Log("A porta está trancada.");
            return;
        }

        ToggleDoor();
    }

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
}
