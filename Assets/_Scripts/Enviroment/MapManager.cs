using System;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static ObjectFaction;

// Map manager for generating the map based on player position

// By OS

// Ensure script executes before others to remove race conditions
[DefaultExecutionOrder(-100)]
public class MapManager : MonoBehaviour
{
    private static MapManager instance;

    // Check if instance exists already.
    public static MapManager mm
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("MapManager");
                instance = obj.AddComponent<MapManager>();
            }

            return instance;
        }
    }

    [Header("Settings")]
    [SerializeField] int tileSize;
    [SerializeField] int generationRadius; // How many tiles around the player should be generated (1 = 3x3)
    [Space(5)]

    [SerializeField] float baseSpawnChance; // 0 - 1 chance to spawn a base on a tile
    [SerializeField] int safeAreaRadius;
    [Space(10)]

    [Header("References")]
    [SerializeField] TileController tilePrefab;
    [SerializeField] Transform tileParent;
    [SerializeField] Camera mapCam;


    public event Action<int> OnZoomChanged;
    readonly Dictionary<Vector2Int, TileController> _tiles = new();

    Vector2Int? _currentChunk;
    int _currentZoom;

    public int CurrentZoom
    {
        get => _currentZoom;
        private set
        {
            if (_currentZoom == value) return;
            _currentZoom = value;
            OnZoomChanged?.Invoke(_currentZoom);
        }
    }

    void OnEnable() => OnZoomChanged += HandleZoom;
    void OnDisable() => OnZoomChanged -= HandleZoom;


    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        GameObject player = gm.Player;
        if (player == null) return;

        // Set the minimap camera to the players position
        mapCam.transform.localPosition = gm.Player.transform.position;

        // Convert Vector3 into Vector2Int
        Vector2Int chunk = WorldToChunk(player.transform.position);

        // Skip generating if the player is a chunk
        if (chunk == _currentChunk) return;

        _currentChunk = chunk;
        GenerateChunks(chunk);
    }

    // Convert Vector3 of the players position into Vector2Int based of the tile size
    Vector2Int WorldToChunk(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / tileSize + 0.5f),
            Mathf.FloorToInt(worldPos.z / tileSize + 0.5f));
    }

    // Generate chunks around the player
    void GenerateChunks(Vector2Int center)
    {
        for (int x = -generationRadius; x <= generationRadius; x++)
        {
            for (int y = -generationRadius; y <= generationRadius; y++)
            {
                Vector2Int pos = new Vector2Int(center.x + x, center.y + y);

                if (!_tiles.ContainsKey(pos))
                    PlaceTile(pos);
            }
        }
    }

    // Place a tile on the position
    void PlaceTile(Vector2Int coord)
    {
        TileController tile = Instantiate(tilePrefab, ChunkToWorld(coord), Quaternion.identity, tileParent);
        tile.SetSize(tileSize / 10);
        _tiles.Add(coord, tile);

        if (coord == Vector2Int.zero)
            tile.SpawnBase(Faction.Player, true);
        else
            tile.CheckSpawnBase(baseSpawnChance, safeAreaRadius);
    }

    // Return center of tile based on tile size
    Vector3 ChunkToWorld(Vector2Int chunk)
    {
        return new Vector3(chunk.x * tileSize, 0f, chunk.y * tileSize);
    }

    // Public setter for current zoom
    public void SetZoom(int zoom)
    {
        CurrentZoom += zoom;
    }

    // Zoom the mini map camera based on player camera zoom
    private void HandleZoom(int currentZoom)
    {
        mapCam.orthographicSize = currentZoom * 50;
    }
}
