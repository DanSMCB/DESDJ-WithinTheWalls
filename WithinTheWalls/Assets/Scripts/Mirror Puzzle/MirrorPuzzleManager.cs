using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class MirrorRoomManager : AttributesSync
{
    public static MirrorRoomManager Instance;

    public List<AttributesSync> roomAObjects;
    public List<AttributesSync> roomBObjects;

    private Dictionary<string, IMirrorState> roomADict;
    private Dictionary<string, IMirrorState> roomBDict;

    [SynchronizableField]
    public bool puzzleSolved;

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

        puzzleSolved=true;
    }

    public void Update()
    {
        if (puzzleSolved)
        {
            Debug.Log("Mirror puzzle solved!");
            // cutscene final
        }
        
    }
}