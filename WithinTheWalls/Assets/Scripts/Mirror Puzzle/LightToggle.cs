using UnityEngine;
using Alteruna;

public class LightToggle : AttributesSync, IMirrorState
{
    public string objectID;
    public GameObject light;
    [SynchronizableField]
    public bool isOn;
    private bool lastUpdate;
    public string ObjectID => objectID;

    void Start()
    {
        isOn = light.activeSelf;
        lastUpdate = isOn;
    }

    public void Interact()
    {
        isOn = !isOn;

        MirrorRoomManager.Instance.CheckRooms();
    }

    public void Update()
    {
        if(isOn!=lastUpdate)
        {
            light.SetActive(isOn);
            lastUpdate = isOn;
        }
    }

    public int GetState()
    {
        return isOn ? 1 : 0;
    }
}
