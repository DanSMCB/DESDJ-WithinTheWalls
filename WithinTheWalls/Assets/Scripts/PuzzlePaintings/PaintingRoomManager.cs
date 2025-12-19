using UnityEngine;
using Alteruna;
using System.Collections;

public class PaintingRoomManager : AttributesSync
{
    public static PaintingRoomManager Instance;

    [Header("Puzzle")]
    [SynchronizableField] public ushort affectedUserIndex = ushort.MaxValue;
    [SynchronizableField] public bool puzzleSolved = false;

    public GameObject PaintingsA;
    public GameObject PaintingsB;

    [Header("Code")]
    public string correctCode = "7162";

    [Header("Door")]
    public Door door;

    void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        UpdatePaintings();

        while (Multiplayer.Instance.GetUsers().Count <= 1)
            yield return null;

        if (Multiplayer.Instance.Me.IsHost)
            PickAffectedPlayer();
    }

    void PickAffectedPlayer()
    {
        var users = Multiplayer.Instance.GetUsers();
        affectedUserIndex = users[Random.Range(0, users.Count)].Index;
        Commit();
    }

    public bool LocalPlayerIsAffected()
    {
        var user = Multiplayer.Instance.GetUser();
        return user != null && user.Index == affectedUserIndex;
    }

    public void TrySubmitCode(string input)
    {
        if (puzzleSolved) return;

        if (input == correctCode)
        {
            puzzleSolved = true;
            Commit();

            door.Unlock();
            Debug.Log("Puzzle resolvido!");
        }
    }

    void UpdatePaintings()
    {
        if (PaintingRoomManager.Instance == null) return;

        bool affected = LocalPlayerIsAffected();

        PaintingsA.SetActive(affected);
        PaintingsB.SetActive(!affected);
    }
}
