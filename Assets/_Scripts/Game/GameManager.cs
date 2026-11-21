using UnityEngine;

// Game Manager for storing global game data

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    // Check if instance exists already.
    public static GameManager gm
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("GameManager");
                instance = obj.AddComponent<GameManager>();
            }

            return instance;
        }
    }

    public GameObject Player { get; private set; }


    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }


    public void RegisterPlayer(PlaneController player) => Player = player.gameObject;

    public void UnregisterPlayer(PlaneController player)
    {
        if (Player == player) Player = null;
    }
}