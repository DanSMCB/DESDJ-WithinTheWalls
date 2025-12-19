using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance { get; private set; }
    public Portal[] portals;

    // Camera local usada pelos portais (setada quando jogador local surge)
    public Camera LocalPlayerCamera { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // opcional: DontDestroyOnLoad(gameObject);
    }

    public void RegisterLocalCamera(Camera cam)
    {
        LocalPlayerCamera = cam;
    }

    void LateUpdate()
    {
        if (portals == null || portals.Length == 0) return;

        // Não processar portais até que exista a camera local
        if (LocalPlayerCamera == null) return;

        // Before rendering portals
        foreach (var p in portals)
            p.PrePortalRender();

        // Render portals
        foreach (var p in portals)
            p.Render();

        // After rendering portals
        foreach (var p in portals)
            p.PostPortalRender(LocalPlayerCamera);
    }
}
