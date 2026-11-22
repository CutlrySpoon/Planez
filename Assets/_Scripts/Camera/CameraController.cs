using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static MapManager;

// Camera controller for the player

// By OS

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float followSmoothTime;
    [SerializeField] int zoomIncrement;
    [SerializeField] int minZoom;
    [SerializeField] int maxZoom;
    [Space(5)]

    [Header("Input")]
    [SerializeField] InputActionReference zoomAction;
    [Space(5)]

    [Header("References")]
    [SerializeField] Transform camParent;
    [SerializeField] Camera cam;

    Vector3 _followVelocity;
    float _currentFollowSmooth;

    // Subscribe to input
    private void OnEnable()
    {
        zoomAction.action.Enable();
        mm.OnZoomChanged += HandleZoom;
    }

    private void OnDisable()
    {
        zoomAction.action.Disable();
        mm.OnZoomChanged -= HandleZoom;
    }

    private void Start()
    {
        mm.SetZoom(minZoom);
    }

    private void Update()
    {
        UpdateInput();
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    // Change the camera zoom based on player input
    void UpdateInput()
    {
        float input = zoomAction.action.ReadValue<float>();

        if (input < 0 && mm.CurrentZoom > minZoom)
        {
            mm.SetZoom(-zoomIncrement);
        }

        else if (input > 0 && mm.CurrentZoom < maxZoom)
        {
            mm.SetZoom(zoomIncrement);
        }

    }

    // Update the camera position based on zoom
    void FollowTarget()
    {
        GameObject player = GameManager.gm.Player;
        if (player == null) return;

        Vector3 p = player.transform.position;
        Vector3 desired = new Vector3(p.x, mm.CurrentZoom, p.z);

        camParent.position = Vector3.SmoothDamp(camParent.position, desired, ref _followVelocity, _currentFollowSmooth);
    }

    void HandleZoom(int currentZoom)
    {
        float smooth = (followSmoothTime / 100) * currentZoom;
        _currentFollowSmooth = smooth;
    }
}
