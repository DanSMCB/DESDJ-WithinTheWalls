using UnityEngine;

public class PortalRenderManager : MonoBehaviour
{
    public Portal[] portals;

    Camera playerCam;

    void Awake()
    {
        playerCam = Camera.main;
    }

    void LateUpdate()
    {
        if (portals == null || portals.Length == 0) return;

        // Before rendering portals
        foreach (var p in portals)
            p.PrePortalRender();

        // Render portals
        foreach (var p in portals)
            p.Render();

        // After rendering portals
        foreach (var p in portals)
            p.PostPortalRender();
    }
}
