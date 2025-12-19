using UnityEngine;
using Alteruna;

public class Item : AttributesSync
{
    public string itemName;
    public int quantity;
    public Sprite sprite;

    [SynchronizableField] public bool pickedUp = false;

    public void RequestPickup()
    {
        if (pickedUp) return;

        if (Multiplayer.Instance.Me.IsHost)
        {
            Pickup();
        }
        else
        {
            InvokeRemoteMethod(nameof(Pickup));
        }
    }

    [SynchronizableMethod]
    private void Pickup()
    {
        if (pickedUp) return;

        pickedUp = true;
        Commit();

        // desaparecer para todos
        gameObject.SetActive(false);
    }
}
