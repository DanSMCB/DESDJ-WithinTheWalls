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

    //[HideInInspector]
    public bool canLook = true;

    private PlayerInputActions playerInputActions;

    private Alteruna.Avatar _avatar;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    void Start()
    {
        playerInputActions = new PlayerInputActions();
        _avatar = GetComponent<Alteruna.Avatar>();
        if (!_avatar.IsMe)
            return;

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
                Debug.Log("Picked up " + item.itemName);
                InventoryManager inv = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
                inv.AddItem(item.itemName, item.quantity, item.sprite);

                Destroy(item.gameObject);
                return;
            }

            // Interagir com puzzle do relogio
            ClockPuzzle clock = hit.collider.GetComponent<ClockPuzzle>();
            if (clock != null)
            {
                clock.Interact();
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
