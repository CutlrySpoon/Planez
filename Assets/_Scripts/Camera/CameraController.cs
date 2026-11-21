using UnityEngine;
using UnityEngine.InputSystem;
using static MapManager;

// Camera controller for the player

// By OS

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int zoomIncrement;
    [SerializeField] int minZoom;
    [SerializeField] int maxZoom;
    [Space(5)]

    [Header("Input")]
    [SerializeField] InputActionReference zoomAction;
    [Space(5)]

    [Header("References")]
    [SerializeField] Transform camParent;


    // Subscribe to input
    private void OnEnable()
    {
        zoomAction.action.Enable();
    }

    private void OnDisable()
    {
        zoomAction.action.Disable();
    }

    private void Start()
    {
        mm.SetZoom(minZoom);
        UpdateCameraPos();
    }

    private void Update()
    {
        UpdateInput();
    }

    // Change the camera zoom based on player input
    void UpdateInput()
    {
        float input = zoomAction.action.ReadValue<float>();

        if (input < 0 && mm.CurrentZoom > minZoom)
        {
            mm.SetZoom(-zoomIncrement);
            UpdateCameraPos();
        }

        else if (input > 0 && mm.CurrentZoom < maxZoom)
        {
            mm.SetZoom(zoomIncrement);
            UpdateCameraPos();
        }

    }

    // Update the camera position based on zoom
    void UpdateCameraPos()
    {
        Vector3 pos = camParent.localPosition;
        pos.y = mm.CurrentZoom;
        camParent.localPosition = pos;
    }
}
