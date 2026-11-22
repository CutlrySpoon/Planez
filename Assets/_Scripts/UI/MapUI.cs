using UnityEngine;
using static MapManager;

// Mini-Map sprite UI setter and controller

// By OS

public class MapUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int scaler;
    [SerializeField] bool overrideScaling = false; // Disables size scaling on camera zoom
    [SerializeField] float spriteSize;
    [Space(5)]

    [Header("References")]
    [SerializeField] Sprite sprite;

    Material _mat;
    Renderer _matRender;
    SpriteRenderer _spriteRender;

    void OnEnable() => mm.OnZoomChanged += HandleZoom;
    void OnDisable() => mm.OnZoomChanged -= HandleZoom;

    void Awake()
    {
        _matRender = GetComponentInChildren<Renderer>();
        _mat = _matRender.material;

        _spriteRender = GetComponentInChildren<SpriteRenderer>();
    }

    // Set the UI to the correct orientation and layer
    private void Start()
    {
        _spriteRender.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        _spriteRender.gameObject.layer = LayerMask.NameToLayer("MapUI");
        UpdateSprite();
        HandleZoom(mm.CurrentZoom);
    }

    // Scale the sprite based on current zoom if override is false
    void HandleZoom(int currentZoom)
    {
        if (overrideScaling) return;

        spriteSize = currentZoom * scaler;
        UpdateSprite();
    }

    // Set the sprite size
    void UpdateSprite()
    {
        _spriteRender.transform.localScale = new Vector3(spriteSize, spriteSize, spriteSize);
        _spriteRender.sprite = sprite;

        // Fix tiling for custom sprites 
        if (_spriteRender.drawMode == SpriteDrawMode.Tiled)
            _spriteRender.size = new Vector2(1.32f, 1.32f);
    }

    // External function for changing the sprite size
    public void SetSpriteSize(int size)
    {
        spriteSize = size;
        UpdateSprite();
    }

    public void SetColour(Color colour)
    {
        _spriteRender.color = colour;
    }

    public void SetMaterial(Color colour)
    {
        _mat.color = colour;
    }
}
