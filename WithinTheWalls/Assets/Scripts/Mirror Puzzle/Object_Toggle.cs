using UnityEngine;
using Alteruna;

public class Object_Toggle : AttributesSync, IMirrorState
{
    public string objectID;
    public GameObject[] states;
    private int currentIndex;

    [SynchronizableField]
    private int syncedIndex;
    public string ObjectID => objectID;

    void Start()
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].activeSelf)
            {
                currentIndex = i;
                syncedIndex = i;
                break;
            }
        }
    }

    public void Interact()
    {
        syncedIndex=(currentIndex + 1) % states.Length;

        MirrorRoomManager.Instance.CheckRooms();
    }

    public void Update()
    {
        if (syncedIndex != currentIndex)
        {
            currentIndex = syncedIndex;
            for (int i = 0; i < states.Length; i++)
                states[i].SetActive(i == currentIndex);
        }
    }

    public int GetState()
    {
        return currentIndex;
    }
}