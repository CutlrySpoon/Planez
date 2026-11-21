using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

// Plane flying physics controller

// By OS


public class PlaneController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxThrust; // Speed multiplier at full throttle
    [SerializeField] float turnSpeed; // Max turn rate
    [SerializeField] float turnGrip; // Strength of cancelling slide
    [SerializeField] float turnSensitivity; // Weight of turning
    [SerializeField] AnimationCurve steeringCurve; // Steering sensitivity relative to speed
    [Space(5)]

    [Header("Visuals")]
    [SerializeField] Transform mesh;
    [SerializeField] AnimationCurve rollCurve;
    [SerializeField] float maxRollAngle;
    [SerializeField] float rollSensitivity;
    [Space(5)]


    [Header("Input")]
    [SerializeField] InputActionReference steerAction;
    [SerializeField] InputActionReference throttleAction;

    Rigidbody rb;

    Vector3 _localVelocity;

    float _throttle;
    float _steerInput;
    float _localYaw;

    float _steeringPower;
    float _currentRoll;
    float _rollVelocity;

    private void OnEnable()
    {
        steerAction.action.Enable();
        throttleAction.action.Enable();
        gm.RegisterPlayer(this);
    }

    private void OnDisable()
    {
        steerAction.action.Disable();
        throttleAction.action.Disable();
        gm.UnregisterPlayer(this);
    }

    // Assign references from local components
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateInput();

        // ##### TEMP #####
        // To check top speed
        //Debug.Log(rb.linearVelocity.magnitude.ToString("F0"));
    }

    private void LateUpdate()
    {
        UpdateRoll();
    }

    private void FixedUpdate()
    {
        float time = Time.fixedDeltaTime;

        UpdateLocalState();
        UpdateThrust();
        UpdateSteer(time);
        UpdateGrip();
    }

    void UpdateInput()
    {
        _steerInput = steerAction.action.ReadValue<float>();
        float throttleInput = throttleAction.action.ReadValue<float>();

        _throttle = Mathf.Clamp(_throttle + throttleInput * Time.deltaTime, 0.15f, 1f);
    }

    // Converts world space velocity to local
    void UpdateLocalState()
    {
        Quaternion invRotation = Quaternion.Inverse(rb.rotation);

        // Forward velocity relative to where the plane is pointing
        _localVelocity = invRotation * rb.linearVelocity;

        // Sideways velocity drift
        _localYaw = rb.angularVelocity.y * Mathf.Rad2Deg;
    }

    // Pushes plane forward based on throttle input
    void UpdateThrust()
    {
        rb.AddRelativeForce(Vector3.forward * (_throttle * maxThrust));
    }

    // Pushes plane angular velocity to induce turning 
    void UpdateSteer(float time)
    {
        // Get speed and value from steering curve
        float speed = Mathf.Max(0, _localVelocity.z);
        float steeringPower = steeringCurve.Evaluate(speed);
        _steeringPower = steeringPower;

        // Set the target rotation based on input and settings with a min value
        float targetYaw = _steerInput * turnSpeed * steeringPower;
        float authority = Mathf.Max(steeringPower, 0.2f);
        float newYaw = Mathf.MoveTowards(_localYaw, targetYaw, turnSensitivity * authority * time);


        rb.angularVelocity = Vector3.up * (newYaw * Mathf.Deg2Rad);
    }

    // Pushes plane in opposite direction of angular velocity
    // This ensures the plane travels forward instead of drifting
    void UpdateGrip()
    {
        rb.AddRelativeForce(Vector3.right * (-_localVelocity.x * turnGrip), ForceMode.Acceleration);
    }

    // Visually moves plane mesh to simulate roll
    void UpdateRoll()
    {
        if (mesh == null) return;

        // Get speed and value from rolling curve
        float speed = Mathf.Max(0f, _localVelocity.z);
        float rollPower = rollCurve.Evaluate(speed);

        // Set max roll speed, angle and roll target
        float availableYaw = turnSpeed * Mathf.Max(_steeringPower, 0.01f);
        float turnAmount = Mathf.Clamp(_localYaw / availableYaw, -1f, 1f);
        float targetRoll = -turnAmount * maxRollAngle * rollPower;

        _currentRoll = Mathf.SmoothDampAngle(_currentRoll, targetRoll, ref _rollVelocity, rollSensitivity);
        mesh.localRotation = Quaternion.Euler(0f, 0f, _currentRoll);
    }
}
