using UnityEngine;
using UnityEngine.InputSystem;

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

    private PlayerController player;    

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        cam = GetComponentInChildren<Camera>();
        lightSource = transform.GetChild(3).gameObject;
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void OnEnable()
    {
        player.input.Player.Flashlight.performed += OnFlashlight;
    }

    private void OnDisable()
    {
        player.input.Player.Flashlight.performed -= OnFlashlight;
    }

    private void Start()
    {
        lightSource.gameObject.SetActive(false);
        offset = transform.position - cam.transform.position;
    }
    
    private void OnFlashlight(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (IsOn == false)
        {
            lightSource.gameObject.SetActive(true);
            IsOn = true;
            audioSource.PlayOneShot(onSFX);
        }
        else
        {
            lightSource.gameObject.SetActive(false);
            IsOn = false;
            audioSource.PlayOneShot(offSFX);
        }
    }

    private void Update()
    {
        transform.position = cam.transform.position + offset;
        transform.rotation = Quaternion.Slerp(transform.rotation, cam.transform.rotation, speed * Time.deltaTime);

        if (!IsEnabled)
        {
            lightSource.gameObject.SetActive(false);
            IsOn = false;
            return;
        }
    }

    public void PlayFlashlightOffSFX()
    {
        audioSource.PlayOneShot(onSFX);
        audioSource.PlayDelayed(2f);
        audioSource.PlayOneShot(offSFX);
    }
}
