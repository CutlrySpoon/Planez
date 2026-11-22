using System;
using UnityEngine;
using static ObjectFaction;
using static GameManager;

// Script attached to each tile

// By OS

public class TileController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform tileMesh;
    [SerializeField] MapUI tileMapUI;
    [SerializeField] BaseController basePrefab;

    public Faction Faction
    {
        get => _faction;
        set
        {
            if (_faction == value) return;
            _faction = value;
            OnFactionChanged?.Invoke(_faction);
        }
    }

    public event Action<Faction> OnFactionChanged;

    Faction _faction;
    Vector2 _position;
    float _worldSize;
    BaseController _base;


    void OnEnable() => OnFactionChanged += CheckFaction;
    void OnDisable() => OnFactionChanged -= CheckFaction;

    private void Awake()
    {
        _position = new Vector2(transform.position.x, transform.position.z);
    }

    // Change the tiles faction visuals
    void CheckFaction(Faction faction)
    {
        if (_base == null) return;

        if (faction == Faction.Player)
        {
            SetColour(gm.playerColour);
        }
        else if (faction == Faction.Friendly)
        {
            SetColour(gm.friendlyColour);
        }
        else if (faction == Faction.Enemy)
        {
            SetColour(gm.enemyColour);
        }
    }

    void SetColour(Color colour)
    {
        tileMapUI.SetColour(colour);
        if (_base != null) _base.SetFaction(colour);
    }

    // Set the tiles size based on map manager settings
    public void SetSize(int size)
    {
        tileMesh.transform.localScale = new Vector3(size, size, size);
        tileMapUI.SetSpriteSize(size * 10);
        _worldSize = size * 10f;
    }

    // Check if this tile can spawn a Base with random chance and set a faction
    public void CheckSpawnBase(float chance, int safeRad)
    {
        bool inSafe = _position.magnitude < safeRad;
        if (inSafe || UnityEngine.Random.value >= chance) return;

        SpawnBase(RollFaction(chance));
    }

    // Spawn a base at a random location in the tile
    public void SpawnBase(Faction faction, bool atCentre = false)
    {
        // Spawn a base at map origin as players first base
        float range = atCentre ? 0f : _worldSize * 0.35f;

        Vector3 offset = new Vector3(UnityEngine.Random.Range(-range, range), 0f, UnityEngine.Random.Range(-range, range));
        _base = Instantiate(basePrefab, transform.position + offset, Quaternion.identity, transform);
        _base.SetBase();

        Faction = faction;
    }

    // Pick a random faction
    Faction RollFaction(float friendlyChance)
    {
        return UnityEngine.Random.value < friendlyChance ? Faction.Friendly : Faction.Enemy;
    }
}
