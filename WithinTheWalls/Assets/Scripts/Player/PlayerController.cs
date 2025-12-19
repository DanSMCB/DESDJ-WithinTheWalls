using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : PortalTraveller
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float gravity = 18f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 10f;
    public Vector2 pitchLimits = new Vector2(-40f, 85f);
    public float rotationSmoothTime = 0.08f;

    [Header("Cursor")]
    public bool lockCursor = true;

    [Header("Animation")]
    public Animator animator;

    CharacterController controller;
    Camera cam;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float yaw;
    private float pitch;
    private float smoothYaw;
    private float smoothPitch;
    private float yawV;
    private float pitchV;

    private float verticalVelocity;
    Vector3 velocity;

    public float interactDistance = 4f;

    [HideInInspector]
    public bool canLook = true;

    private PlayerInputActions playerInputActions;
    private PlayerInput playerInput;

    private Alteruna.Avatar _avatar;

    void Awake()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
    }

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        if (!_avatar.IsMe)
        {
            if (cam != null)
            {
                cam.enabled = false;
                cam.GetComponent<AudioListener>().enabled = false;
            }

            return;
        }

        playerInput.enabled = true;
        playerInputActions = new PlayerInputActions();

        controller = GetComponent<CharacterController>();

        // Primeiro apanhar a camera!
        cam = GetComponentInChildren<Camera>();
        if (cam == null)
        {
            Debug.LogError("PlayerController: NENHUMA camera encontrada no jogador!");
            return;
        }

        if (PortalManager.Instance != null)
            PortalManager.Instance.RegisterLocalCamera(cam);

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        pitch = cam.transform.localEulerAngles.x;
        if (pitch > 180f)
            pitch -= 360f;

        yaw = transform.eulerAngles.y;
        smoothPitch = pitch;
        smoothYaw = yaw;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void OnInteract()
    {
        RaycastHit hit;
        float radius = 0.5f;

        int layerMask = ~LayerMask.GetMask("Ignore Raycast");

        if (Physics.SphereCast(cam.transform.position, radius, cam.transform.forward, out hit, interactDistance, layerMask))
        {
            //Abrir porta
            Door door = hit.collider.GetComponent<Door>();

            if (door != null)
            {
                door.Interact();
            }

            // Apanhar items
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                InventoryManager inv = GameObject.Find("InventoryCanvas")
                    .GetComponent<InventoryManager>();

                inv.AddItem(item.itemName, item.quantity, item.sprite);

                item.RequestPickup();
                return;
            }

            // Interagir com puzzle do relogio
            ClockPuzzle clock = hit.collider.GetComponent<ClockPuzzle>();
            if (clock != null)
            {
                clock.Interact();
                return;
            }

            // ------------------- Puzzle da Cave --------------------
            // Interagir com válvula do boiler
            BoilerValveController valve = hit.collider.GetComponent<BoilerValveController>();
            if (valve != null && !BoilerRoomManager.Instance.LocalPlayerIsAffected())
            {
                valve.InteractValve();
                return;
            }

            // ------------------- Puzzle da Galeria --------------------
            KeypadController keypad = hit.collider.GetComponentInParent<KeypadController>();
            if (keypad != null)
            {
                keypad.Interact(this);
                return;
            }

            // ------------------- Puzzle do Espelho --------------------
            // Luz
            LightToggle lightToggle = hit.collider.GetComponent<LightToggle>();
            if (lightToggle != null)
            {
                lightToggle.Interact();
                return;
            }

            // Objetos
            Object_Toggle objectToggle = hit.collider.GetComponent<Object_Toggle>();
            if (objectToggle != null)
            {
                objectToggle.Interact();
                return;
            }

            // AC
            ACToggle aCToggle = hit.collider.GetComponent<ACToggle>();
            if (aCToggle != null)
            {
                aCToggle.Interact();
                return;
            }
        }
    }

    private void OnFlashlight()
    {
        FlashlightController flashlight = GetComponent<FlashlightController>();
        if (flashlight != null)
        {
            flashlight.ToggleFlashlight();
        }
    }

    private void OnInventory()
    {
        canLook = !canLook;
        InventoryManager inv = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        inv.ToggleInventory();
    }

    void Update()
    {
        if (!_avatar.IsMe)
            return;

        if (PortalManager.Instance != null && PortalManager.Instance.LocalPlayerCamera != cam)
        {
            PortalManager.Instance.RegisterLocalCamera(cam);
            Debug.Log($"PortalManager: Registered local player camera = {cam?.name}");
        }

        if (!canLook) 
            return;

        HandleMovement();
        ApplyRotation();
        HandleLook();
    }

    private void HandleMovement()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 horizontalVelocity = transform.TransformDirection(inputDir) * walkSpeed;

        animator.SetFloat("Speed", horizontalVelocity.magnitude);

        // Handle gravity
        if (controller.isGrounded)
            verticalVelocity = -1f;
        else
            verticalVelocity -= gravity * Time.deltaTime;

        // Combine
        velocity = horizontalVelocity;
        velocity.y = verticalVelocity;

        // Apply
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        yaw += lookInput.x * mouseSensitivity;
        pitch = Mathf.Clamp(pitch - lookInput.y * mouseSensitivity, pitchLimits.x, pitchLimits.y);

        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawV, rotationSmoothTime);
        smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchV, rotationSmoothTime);
    }

    private void ApplyRotation()
    {
        transform.eulerAngles = Vector3.up * smoothYaw;
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle(smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;
        velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));
        Physics.SyncTransforms();
    }
}
