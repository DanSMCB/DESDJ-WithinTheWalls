using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PortalCamera : MonoBehaviour
{
    public LayerMask portalVolumeLayer;   // A layer onde está o volume do portal
    public Transform portalVolumeCenter;  // Onde a câmara deve "sentir" o volume

    private Camera cam;
    private UniversalAdditionalCameraData camData;

    void Awake()
    {
        cam = GetComponent<Camera>();
        camData = GetComponent<UniversalAdditionalCameraData>();

        if (camData == null)
        {
            Debug.LogError("Esta camera não tem UniversalAdditionalCameraData!");
        }
    }

    void Start()
    {
        ApplyPortalVolumeSettings();
    }

    public void ApplyPortalVolumeSettings()
    {
        if (camData == null) return;

        // Define a layer mask correta para o volume do portal
        camData.volumeLayerMask = portalVolumeLayer;

        // Define o ponto onde a câmara avalia o volume
        if (portalVolumeCenter != null)
            camData.volumeTrigger = portalVolumeCenter;
        else
            camData.volumeTrigger = transform;

        Debug.Log("Portal Volume aplicado!");
    }
}
