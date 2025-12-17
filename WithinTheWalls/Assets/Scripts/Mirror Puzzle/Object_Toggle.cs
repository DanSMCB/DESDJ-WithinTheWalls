using UnityEngine;

public class Object_Toggle : MonoBehaviour, IMirrorState
{
    public string objectID;
    public GameObject[] states;
    private int currentIndex;

    public string ObjectID => objectID;

    void Start()
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].activeSelf)
            {
                currentIndex = i;
                break;
            }
        }
    }

    public void Interact()
    {
        currentIndex = (currentIndex + 1) % states.Length;

        for (int i = 0; i < states.Length; i++)
            states[i].SetActive(i == currentIndex);

        MirrorRoomManager.Instance.CheckRooms();
    }

    public int GetState()
    {
        return currentIndex;
    }
}
