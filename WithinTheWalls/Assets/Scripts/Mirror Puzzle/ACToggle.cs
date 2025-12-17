using UnityEngine;

public class ACToggle : MonoBehaviour, IMirrorState
{
    public string objectID;
    public GameObject ac_off;
    public GameObject ac_on;
    public AudioSource audioSource;
    private bool isOn;
    public string ObjectID => objectID;

    void Start()
    {
        if (ac_on.activeSelf)
        {
            isOn = true;
            audioSource.Play();
        }
        else
        {
            isOn = false;
            audioSource.Stop();
        }
    }

    public void Interact()
    {
        isOn = !isOn;

        ac_on.SetActive(isOn);
        ac_off.SetActive(!isOn);

        if (isOn)
            audioSource.Play();
        else
            audioSource.Stop();

        MirrorRoomManager.Instance.CheckRooms();
    }

    public int GetState()
    {
        return isOn ? 1 : 0;
    }
}