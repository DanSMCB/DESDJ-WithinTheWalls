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
    private PlayerInputActions input;

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

    //private Alteruna.Avatar _avatar;
    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    void Start()
    {
        //_avatar = GetComponent<Alteruna.Avatar>();
        //if (!_avatar.IsMe)
        //    return;

        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();

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

    void Update()
    {
        //if (!_avatar.IsMe)
        //    return;

        HandleMovement();
        HandleLook();
    }

    void LateUpdate()
    {
        //if (!_avatar.IsMe)
        //    return;

        ApplyRotation();
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
