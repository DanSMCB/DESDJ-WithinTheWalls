using UnityEngine;

public class EntryRoomTrigger : MonoBehaviour
{
    public LoopIterationManager loopManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Avatar"))
        {
            Debug.Log("Entry Room Triggered");
            loopManager.ActivateLoop();
            this.gameObject.SetActive(false);
        }
    }
}