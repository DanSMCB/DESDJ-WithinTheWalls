using System.Collections.Generic;
using UnityEngine;

public class MirrorRoomManager : MonoBehaviour
{
    public static MirrorRoomManager Instance;

    public List<MonoBehaviour> roomAObjects;
    public List<MonoBehaviour> roomBObjects;

    private Dictionary<string, IMirrorState> roomADict;
    private Dictionary<string, IMirrorState> roomBDict;

    void Awake()
    {
        Instance = this;

        roomADict = new Dictionary<string, IMirrorState>();
        roomBDict = new Dictionary<string, IMirrorState>();

        foreach (var obj in roomAObjects)
        {
            IMirrorState state = obj as IMirrorState;
            roomADict.Add(state.ObjectID, state);
        }

        foreach (var obj in roomBObjects)
        {
            IMirrorState state = obj as IMirrorState;
            roomBDict.Add(state.ObjectID, state);
        }
    }

    public void CheckRooms()
    {
        foreach (var key in roomADict.Keys)
        {
            Debug.Log("Mirror " + key + ": A=" + roomADict[key].GetState() + " | B=" + roomBDict[key].GetState());
            if (roomADict[key].GetState() != roomBDict[key].GetState())
                return;
        }

        OnPuzzleSolved();
    }

    void OnPuzzleSolved()
    {
        Debug.Log("Mirror puzzle solved!");
        // aqui: trocar vidro por espelho, trigger final, etc
    }
}