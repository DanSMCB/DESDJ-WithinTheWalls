using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] AudioClip onSFX;
    [SerializeField] AudioClip offSFX;

    private Camera cam;
    private GameObject lightSource;
    private AudioSource audioSource;
    private Vector3 offset;

    public bool IsOn { get; private set; }
    private readonly float speed = 5f;

    public bool IsEnabled = true;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        lightSource = transform.GetComponentInChildren<Light>().gameObject;
        audioSource = GetComponentInChildren<AudioSource>();
            
        offset = lightSource.transform.position - cam.transform.position;

        lightSource.SetActive(false);
    }

    void LateUpdate()
    {
        lightSource.transform.position = cam.transform.position + offset;
        lightSource.transform.rotation = Quaternion.Slerp(lightSource.transform.rotation, cam.transform.rotation, speed * Time.deltaTime);
    }

    public void ToggleFlashlight()
    {
        if (IsEnabled)
        {
            if (IsOn)
            {
                FlashlightOff();
            }else
            {
                FlashlightOn();
            }
        }
    }

    public void FlashlightOff()
    {
        lightSource.SetActive(false);
        IsOn = false;
        audioSource.PlayOneShot(offSFX);
    }

    public void FlashlightOn()
    {
        lightSource.SetActive(true);
        IsOn = true;
        audioSource.PlayOneShot(onSFX);
    }
}