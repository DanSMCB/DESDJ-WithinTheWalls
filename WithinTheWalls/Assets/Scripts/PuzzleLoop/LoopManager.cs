using UnityEngine;
using System.Collections;

public class LoopIterationManager : MonoBehaviour
{
    public GameObject entryPortal;
    public GameObject exitDoor;
    public GameObject exitWall;
    public GameObject clock;
    private bool loopActivated = false;

    void Start()
    {
        entryPortal.SetActive(false);
    }

    public void ActivateLoop()
    {
        if (loopActivated) return;

        loopActivated = true;
        entryPortal.SetActive(true);
    }

    public void DeactivateLoop()
    {
        loopActivated = false;
        entryPortal.SetActive(false);
        exitDoor.SetActive(false);
        exitWall.SetActive(false);
        clock.SetActive(false);
    }
}