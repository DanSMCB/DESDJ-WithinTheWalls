using UnityEngine;
using Alteruna;

public class MirrorEntryTrigger : AttributesSync
{
    public Door entryDoor;
    public GameObject player2Portal;

    [SynchronizableField]
    private bool portalActive;

    private void Update()
    {
        player2Portal.SetActive(portalActive);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Avatar")) return;

        portalActive = true;

        entryDoor.ToggleDoor();
        entryDoor.Lock();
        gameObject.GetComponent<Collider>().enabled = false;

    }
}
