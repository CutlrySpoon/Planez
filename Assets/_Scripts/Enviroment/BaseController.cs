using UnityEngine;

// Base controller for setting and managing a base

// By OS

public class BaseController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float minSize;
    [SerializeField] float maxSize;
    [Space(10)]

    [Header("References")]
    [SerializeField] Transform mesh;
    [SerializeField] MapUI baseMapUI;

    // Sets a random size and rotation of a base for variety
    public void SetBase()
    {
        float sizeX = Random.Range(minSize, maxSize);
        float sizeY = sizeX * 2;

        mesh.localScale = new Vector3(sizeX, 1, sizeY);
        mesh.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
    }

    // Sets the base colours to match its faction.
    public void SetFaction(Color colour)
    {
        baseMapUI.SetColour(colour);
        baseMapUI.SetMaterial(colour);
    }
}
