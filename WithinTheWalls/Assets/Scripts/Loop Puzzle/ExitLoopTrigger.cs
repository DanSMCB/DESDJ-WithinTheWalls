using UnityEngine;

public class ExitRoomTrigger : MonoBehaviour
{
    public LoopIterationManager loopManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Avatar"))
        {
            Debug.Log("Exit Room Triggered");
            loopManager.DeactivateLoop();
            this.gameObject.SetActive(false);
        }
    }
}