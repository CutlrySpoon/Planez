using UnityEngine;

// Script attached to each tile

// By OS

public class TileController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform tileMesh;
    [SerializeField] MapUI tileMapUI;

    public Vector2 Position { get; private set; }

    private void Awake()
    {
        Position = new Vector2(transform.position.x, transform.position.z);
    }

    public void SetSize(int size)
    {
        tileMesh.transform.localScale = new Vector3(size, size, size);
        tileMapUI.SetSpriteSize(size * 10);
    }
}
