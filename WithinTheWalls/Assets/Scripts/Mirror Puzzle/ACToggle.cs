using UnityEngine;
using Alteruna;

public class ACToggle : AttributesSync, IMirrorState
{
    public string objectID;
    public GameObject ac_off;
    public GameObject ac_on;
    public AudioSource audioSource;
    [SynchronizableField]
    public bool isOn;
    private bool lastUpdate;
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
            if (isOn)
            {
                if (!audioSource.isPlaying)
                    audioSource.Play();
                ac_on.SetActive(true);
                ac_off.SetActive(false);
            }
            else
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();
                ac_on.SetActive(false);
                ac_off.SetActive(true);
            }
            lastUpdate = isOn;
        }
    }

    public int GetState()
    {
        return isOn ? 1 : 0;
    }
}
