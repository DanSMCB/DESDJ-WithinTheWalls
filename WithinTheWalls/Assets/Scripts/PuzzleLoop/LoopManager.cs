using UnityEngine;
using System.Collections;

public class LoopIterationManager : MonoBehaviour
{
    public GameObject portalI;
    public GameObject portalJ;
    public GameObject portalK;
    public GameObject portalL;
    public GameObject portalM;
    public GameObject portalN;
    public GameObject portalO;
    public GameObject portalP;
    public GameObject wallClock;
    public GameObject doorClock;
    public GameObject clock;

    void Start()
    {
        portalI.SetActive(false);
    }

    public void ActivateLoop()
    {
        portalI.SetActive(true);
    }

    public void DeactivateLoop()
    {
        portalI.SetActive(false);
        portalJ.SetActive(false);
        portalK.SetActive(false);
        portalL.SetActive(false);
        portalM.SetActive(false);
        portalN.SetActive(false);
        portalO.SetActive(false);
        portalP.SetActive(false);
        wallClock.SetActive(false);
        doorClock.SetActive(false);
        clock.SetActive(false);
    }
}