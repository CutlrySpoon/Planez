using UnityEngine;
using static MapManager;

// Mini-Map sprite UI setter and controller

// By OS

public class MapUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool overrideScaling = false; // Disables size scaling on camera zoom
    [SerializeField] float spriteSize;
    [Space(5)]

    [Header("References")]
    [SerializeField] SpriteRenderer target;
    [SerializeField] Sprite sprite;

    void OnEnable() => mm.OnZoomChanged += HandleZoom;
    void OnDisable() => mm.OnZoomChanged -= HandleZoom;

    // Set the UI to the correct orientation and layer
    private void Start()
    {
        target.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        target.gameObject.layer = LayerMask.NameToLayer("MapUI");
        UpdateSprite();
    }

    // Scale the sprite based on current zoom if override is false
    void HandleZoom(int currentZoom)
    {
        if (overrideScaling) return;

        spriteSize = currentZoom * 10;
        UpdateSprite();
    }

    // Set the sprite size
    void UpdateSprite()
    {
        target.transform.localScale = new Vector3(spriteSize, spriteSize, spriteSize);
        target.sprite = sprite;

        // Fix tiling for custom sprites 
        if (target.drawMode == SpriteDrawMode.Tiled)
            target.size = new Vector2(1.32f, 1.32f);
    }

    // External function for changing the sprite size
    public void SetSpriteSize(int size)
    {
        spriteSize = size;
        UpdateSprite();
    }
}
