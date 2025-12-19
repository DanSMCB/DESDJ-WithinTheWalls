using UnityEngine;
using Alteruna;

public class MirrorEntryTrigger_2 : AttributesSync
{
    public Door entryDoor;
    public GameObject player2Portal;

    [SynchronizableField]
    private bool portalActive=true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Avatar")) return;

        portalActive = false;
        
        entryDoor.ToggleDoor();
        entryDoor.Lock();
        gameObject.GetComponent<Collider>().enabled = false;
    }

    private void Update()
    {
        player2Portal.SetActive(portalActive);
    }
}
