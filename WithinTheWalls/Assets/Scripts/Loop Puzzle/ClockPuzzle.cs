using UnityEngine;
using UnityEngine.Events;

public class ClockPuzzle : MonoBehaviour
{
    public GameObject hourHandObject;
    public GameObject minuteHandObject;

    public GameObject hourHandObject2;
    public GameObject minuteHandObject2;

    public string hourHandItemName = "clock_hour_hand";
    public string minuteHandItemName = "clock_minute_hand";

    private bool foundMinuteHand = false;
    private bool foundHourHand = false;
    private bool solved = false;
    public UnityEvent onSolved;
    public GameObject portal;
    public GameObject exitPortal;
    public GameObject exitPortal2;
    public GameObject exitCollider;

    public void Interact()
    {
        if (solved) return;

        InventoryManager inv = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();

        if (inv.HasItem(hourHandItemName))
        {
            hourHandObject.SetActive(true);
            inv.RemoveItem(hourHandItemName);
            foundHourHand = true;
        }
        if (inv.HasItem(minuteHandItemName))
        {
            minuteHandObject.SetActive(true);
            inv.RemoveItem(minuteHandItemName);
            foundMinuteHand = true;
        }

        if(foundHourHand && foundMinuteHand)
        {
            solved = true;
            OnPuzzleSolved();
        }
    }

    private void OnPuzzleSolved()
    {
        minuteHandObject2.SetActive(true);
        hourHandObject2.SetActive(true);
        portal.SetActive(false);
        exitPortal.SetActive(true);
        exitPortal2.SetActive(true);
        exitCollider.SetActive(true);
        onSolved.Invoke();
    }
}