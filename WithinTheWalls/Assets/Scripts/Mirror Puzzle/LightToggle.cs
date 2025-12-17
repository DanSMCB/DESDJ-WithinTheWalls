using UnityEngine;

public class LightToggle : MonoBehaviour, IMirrorState
{
    public string objectID;
    public GameObject light;
    private bool isOn;

    public string ObjectID => objectID;

    void Start()
    {
        isOn = light.gameObject.activeSelf;
    }

    public void Interact()
    {
        isOn = !isOn;
        light.SetActive(isOn);

        MirrorRoomManager.Instance.CheckRooms();
    }

    public int GetState()
    {
        return isOn ? 1 : 0;
    }
}
